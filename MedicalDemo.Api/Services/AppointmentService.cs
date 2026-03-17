using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Models;
using MedicalDemo.Api.Repositories;
using Microsoft.Extensions.Logging;

namespace MedicalDemo.Api.Services;

public class AppointmentService(
    IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository,
    IProviderRepository providerRepository,
    IAppointmentPrerequisiteService prerequisiteService,
    IAppointmentChargeService appointmentChargeService,
    ILogger<AppointmentService> logger) : IAppointmentService
{
    public IReadOnlyCollection<AppointmentResponseDTO> GetAll() =>
        appointmentRepository.GetAll()
            .OrderBy(appointment => appointment.AppointmentDate)
            .Select(MapToDto)
            .ToList()
            .AsReadOnly();

    public AppointmentResponseDTO Create(CreateAppointmentDTO request)
    {
        if (!patientRepository.Exists(request.PatientId))
        {
            throw new KeyNotFoundException($"Patient with id {request.PatientId} was not found.");
        }

        if (!providerRepository.Exists(request.ProviderId))
        {
            throw new KeyNotFoundException($"Provider with id {request.ProviderId} was not found.");
        }

        var appointmentStart = request.AppointmentDate;
        var appointmentEnd = request.AppointmentDate.AddMinutes(request.DurationMinutes);

        var conflictExists = appointmentRepository.GetAll()
            .Where(appointment => appointment.ProviderId == request.ProviderId)
            .Any(existingAppointment =>
            {
                var existingStart = existingAppointment.AppointmentDate;
                var existingEnd = existingAppointment.AppointmentDate.AddMinutes(existingAppointment.DurationMinutes);
                return appointmentStart < existingEnd && existingStart < appointmentEnd;
            });

        if (conflictExists)
        {
            logger.LogWarning(
                "Scheduling conflict for provider {ProviderId} at {AppointmentDate} for {DurationMinutes} minutes.",
                request.ProviderId,
                request.AppointmentDate,
                request.DurationMinutes);
            throw new SchedulingConflictException("Provider already has an appointment during this time.");
        }

        var appointment = new Appointment
        {
            PatientId = request.PatientId,
            ProviderId = request.ProviderId,
            AppointmentDate = request.AppointmentDate,
            DurationMinutes = request.DurationMinutes,
            Reason = request.Reason.Trim(),
            EligibilityStatus = "pending",
            EligibilityNotes = "New appointment requires eligibility review."
        };

        var createdAppointment = appointmentRepository.Add(appointment);

        logger.LogInformation(
            "Appointment {AppointmentId} created for patient {PatientId} with provider {ProviderId} at {AppointmentDate} for {DurationMinutes} minutes.",
            createdAppointment.Id,
            createdAppointment.PatientId,
            createdAppointment.ProviderId,
            createdAppointment.AppointmentDate,
            createdAppointment.DurationMinutes);

        return MapToDto(createdAppointment);
    }

    public AppointmentResponseDTO UpdateEligibility(int id, UpdateAppointmentEligibilityDTO request)
    {
        var appointment = appointmentRepository.GetById(id)
            ?? throw new KeyNotFoundException($"Appointment with id {id} was not found.");

        var patient = patientRepository.GetById(appointment.PatientId)
            ?? throw new KeyNotFoundException($"Patient with id {appointment.PatientId} was not found.");

        var normalizedStatus = NormalizeStatus(request.EligibilityStatus);
        var reviewedAt = request.EligibilityReviewedAt ?? DateTime.UtcNow;
        var notes = string.IsNullOrWhiteSpace(request.EligibilityNotes)
            ? DefaultEligibilityNote(normalizedStatus)
            : request.EligibilityNotes.Trim();

        appointment.EligibilityStatus = normalizedStatus;
        appointment.EligibilityReviewedAt = reviewedAt;
        appointment.EligibilityNotes = notes;

        patient.EligibilityStatus = normalizedStatus;
        patient.LastEligibilityVerifiedAt = reviewedAt;
        patient.EligibilityNotes = notes;

        appointmentRepository.Update(appointment);
        patientRepository.Update(patient);

        logger.LogInformation(
            "Eligibility updated for appointment {AppointmentId} to {EligibilityStatus} at {EligibilityReviewedAt}.",
            appointment.Id,
            appointment.EligibilityStatus,
            appointment.EligibilityReviewedAt);

        return MapToDto(appointment);
    }

    private AppointmentResponseDTO MapToDto(Appointment appointment)
    {
        var patient = patientRepository.GetById(appointment.PatientId);
        var missingIntakeItems = patient is null
            ? Array.Empty<string>()
            : PatientService.GetMissingIntakeItems(patient).ToArray();
        var authorization = prerequisiteService.GetAppointmentSummary(appointment.Id, "authorization");
        var referral = prerequisiteService.GetAppointmentSummary(appointment.Id, "referral");
        var billing = appointmentChargeService.GetAppointmentBillingSummary(appointment.Id);

        return new()
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            ProviderId = appointment.ProviderId,
            AppointmentDate = appointment.AppointmentDate,
            DurationMinutes = appointment.DurationMinutes,
            Reason = appointment.Reason,
            EligibilityStatus = appointment.EligibilityStatus,
            EligibilityReviewedAt = appointment.EligibilityReviewedAt,
            EligibilityNotes = appointment.EligibilityNotes,
            IntakeStatus = patient?.IntakeStatus ?? "notStarted",
            IsIntakeComplete = patient is not null && patient.IntakeStatus == "complete" && missingIntakeItems.Length == 0,
            MissingIntakeItems = missingIntakeItems,
            Authorization = authorization,
            Referral = referral,
            HasPrerequisiteBlocker = authorization.IsBlocking || referral.IsBlocking,
            Billing = billing
        };
    }

    private static string NormalizeStatus(string status) =>
        string.IsNullOrWhiteSpace(status) ? "pending" : status.Trim().ToLowerInvariant();

    private static string DefaultEligibilityNote(string status) =>
        status switch
        {
            "verified" => "Coverage verified and ready for the visit.",
            "failed" => "Coverage issue found. Follow up with patient or payer.",
            _ => "Eligibility review is still pending."
        };
}
