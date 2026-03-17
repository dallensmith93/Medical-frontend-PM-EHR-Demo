using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Models;
using MedicalDemo.Api.Repositories;
using Microsoft.Extensions.Logging;

namespace MedicalDemo.Api.Services;

public class AppointmentChargeService(
    IAppointmentChargeRepository chargeRepository,
    IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository,
    IProviderRepository providerRepository,
    IAppointmentPrerequisiteService prerequisiteService,
    ILogger<AppointmentChargeService> logger) : IAppointmentChargeService
{
    public AppointmentChargeDto GetByAppointmentId(int appointmentId)
    {
        var appointment = GetAppointment(appointmentId);
        var charge = chargeRepository.GetByAppointmentId(appointmentId);
        var billing = BuildBillingSummary(appointment, charge);

        return MapToDto(appointment, charge, billing);
    }

    public AppointmentChargeDto Create(int appointmentId, CreateAppointmentChargeDTO request)
    {
        var appointment = GetAppointment(appointmentId);
        var saved = SaveCharge(appointment, request.DiagnosisCode, request.ProcedureCode, request.Modifier, request.Units, request.Amount, request.Notes);
        var billing = BuildBillingSummary(appointment, saved);
        return MapToDto(appointment, saved, billing);
    }

    public AppointmentChargeDto Update(int appointmentId, UpdateAppointmentChargeDTO request)
    {
        var appointment = GetAppointment(appointmentId);
        var saved = SaveCharge(appointment, request.DiagnosisCode, request.ProcedureCode, request.Modifier, request.Units, request.Amount, request.Notes);
        var billing = BuildBillingSummary(appointment, saved);
        return MapToDto(appointment, saved, billing);
    }

    public AppointmentBillingSummaryDTO GetAppointmentBillingSummary(int appointmentId)
    {
        var appointment = GetAppointment(appointmentId);
        return BuildBillingSummary(appointment, chargeRepository.GetByAppointmentId(appointmentId));
    }

    public IReadOnlyCollection<ChargeReviewItemDTO> GetReviewQueue() =>
        appointmentRepository.GetAll()
            .Select(appointment =>
            {
                var billing = BuildBillingSummary(appointment, chargeRepository.GetByAppointmentId(appointment.Id));
                var patient = patientRepository.GetById(appointment.PatientId)
                    ?? throw new KeyNotFoundException($"Patient with id {appointment.PatientId} was not found.");
                var provider = providerRepository.GetAll().FirstOrDefault(item => item.Id == appointment.ProviderId);

                return new ChargeReviewItemDTO
                {
                    AppointmentId = appointment.Id,
                    ChargeId = billing.ChargeId,
                    PatientId = patient.Id,
                    PatientName = $"{patient.FirstName} {patient.LastName}",
                    PayerName = patient.PayerName,
                    ProviderId = appointment.ProviderId,
                    ProviderName = provider?.Name ?? $"Provider #{appointment.ProviderId}",
                    AppointmentDate = appointment.AppointmentDate,
                    Reason = appointment.Reason,
                    Status = billing.Status,
                    WarningCount = billing.WarningCount,
                    Warnings = billing.Warnings
                };
            })
            .OrderBy(item => item.Status == "readyToSubmit")
            .ThenBy(item => item.AppointmentDate)
            .ToList()
            .AsReadOnly();

    private AppointmentCharge SaveCharge(
        Appointment appointment,
        string diagnosisCode,
        string procedureCode,
        string modifier,
        int units,
        decimal amount,
        string notes)
    {
        var patient = patientRepository.GetById(appointment.PatientId)
            ?? throw new KeyNotFoundException($"Patient with id {appointment.PatientId} was not found.");

        var charge = new AppointmentCharge
        {
            AppointmentId = appointment.Id,
            PatientId = patient.Id,
            ProviderId = appointment.ProviderId,
            DiagnosisCode = diagnosisCode.Trim(),
            ProcedureCode = procedureCode.Trim(),
            Modifier = modifier.Trim(),
            Units = units,
            Amount = amount,
            Notes = notes.Trim()
        };

        var saved = chargeRepository.UpsertByAppointment(charge);

        logger.LogInformation(
            "Charge {ChargeId} upserted for appointment {AppointmentId} with status {Status}.",
            saved.Id,
            appointment.Id,
            BuildBillingSummary(appointment, saved).Status);

        return saved;
    }

    private AppointmentBillingSummaryDTO BuildBillingSummary(Appointment appointment, AppointmentCharge? charge)
    {
        var warnings = BuildWarnings(appointment, charge);
        var hasChargeData = charge is not null && HasChargeData(charge);
        var status = !hasChargeData
            ? "draft"
            : warnings.Count > 0
                ? "reviewNeeded"
                : "readyToSubmit";

        return new AppointmentBillingSummaryDTO
        {
            ChargeId = charge?.Id,
            Status = status,
            IsReadyToSubmit = status == "readyToSubmit",
            WarningCount = warnings.Count,
            Warnings = warnings
        };
    }

    private List<ClaimScrubWarningDTO> BuildWarnings(Appointment appointment, AppointmentCharge? charge)
    {
        var warnings = new List<ClaimScrubWarningDTO>();
        var patient = patientRepository.GetById(appointment.PatientId)
            ?? throw new KeyNotFoundException($"Patient with id {appointment.PatientId} was not found.");

        if (charge is null || string.IsNullOrWhiteSpace(charge.DiagnosisCode))
        {
            warnings.Add(new ClaimScrubWarningDTO
            {
                Code = "diagnosis",
                Message = "Diagnosis code is missing.",
                IsBlocking = true
            });
        }

        if (charge is null || string.IsNullOrWhiteSpace(charge.ProcedureCode))
        {
            warnings.Add(new ClaimScrubWarningDTO
            {
                Code = "procedure",
                Message = "Procedure code is missing.",
                IsBlocking = true
            });
        }

        if (charge is null || charge.Units <= 0)
        {
            warnings.Add(new ClaimScrubWarningDTO
            {
                Code = "units",
                Message = "Units must be greater than zero.",
                IsBlocking = true
            });
        }

        if (charge is null || charge.Amount <= 0)
        {
            warnings.Add(new ClaimScrubWarningDTO
            {
                Code = "amount",
                Message = "Charge amount must be greater than zero.",
                IsBlocking = true
            });
        }

        if (appointment.EligibilityStatus != "verified")
        {
            warnings.Add(new ClaimScrubWarningDTO
            {
                Code = "eligibility",
                Message = "Eligibility is not verified for this visit.",
                IsBlocking = true
            });
        }

        if (string.IsNullOrWhiteSpace(patient.PayerName) ||
            string.IsNullOrWhiteSpace(patient.MemberId) ||
            string.IsNullOrWhiteSpace(patient.InsuranceSummary))
        {
            warnings.Add(new ClaimScrubWarningDTO
            {
                Code = "insurance",
                Message = "Insurance information is incomplete for billing.",
                IsBlocking = true
            });
        }

        var missingIntakeItems = PatientService.GetMissingIntakeItems(patient);
        if (patient.IntakeStatus != "complete" || missingIntakeItems.Count > 0)
        {
            warnings.Add(new ClaimScrubWarningDTO
            {
                Code = "intake",
                Message = missingIntakeItems.Count > 0
                    ? $"Intake is incomplete: {string.Join(", ", missingIntakeItems)}."
                    : "Intake is incomplete.",
                IsBlocking = true
            });
        }

        var authorization = prerequisiteService.GetAppointmentSummary(appointment.Id, "authorization");
        if (authorization.IsBlocking)
        {
            warnings.Add(new ClaimScrubWarningDTO
            {
                Code = "authorization",
                Message = $"Authorization is {authorization.Status}.",
                IsBlocking = true
            });
        }

        var referral = prerequisiteService.GetAppointmentSummary(appointment.Id, "referral");
        if (referral.IsBlocking)
        {
            warnings.Add(new ClaimScrubWarningDTO
            {
                Code = "referral",
                Message = $"Referral is {referral.Status}.",
                IsBlocking = true
            });
        }

        return warnings;
    }

    private static bool HasChargeData(AppointmentCharge charge) =>
        !string.IsNullOrWhiteSpace(charge.DiagnosisCode) ||
        !string.IsNullOrWhiteSpace(charge.ProcedureCode) ||
        !string.IsNullOrWhiteSpace(charge.Modifier) ||
        charge.Units > 0 ||
        charge.Amount > 0 ||
        !string.IsNullOrWhiteSpace(charge.Notes);

    private Appointment GetAppointment(int appointmentId) =>
        appointmentRepository.GetById(appointmentId)
            ?? throw new KeyNotFoundException($"Appointment with id {appointmentId} was not found.");

    private static AppointmentChargeDto MapToDto(
        Appointment appointment,
        AppointmentCharge? charge,
        AppointmentBillingSummaryDTO billing) =>
        new()
        {
            Id = charge?.Id,
            AppointmentId = appointment.Id,
            PatientId = appointment.PatientId,
            ProviderId = appointment.ProviderId,
            DiagnosisCode = charge?.DiagnosisCode ?? string.Empty,
            ProcedureCode = charge?.ProcedureCode ?? string.Empty,
            Modifier = charge?.Modifier ?? string.Empty,
            Units = charge?.Units ?? 0,
            Amount = charge?.Amount ?? 0,
            Notes = charge?.Notes ?? string.Empty,
            Billing = billing
        };
}
