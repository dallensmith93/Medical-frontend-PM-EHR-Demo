using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Models;
using MedicalDemo.Api.Repositories;
using Microsoft.Extensions.Logging;

namespace MedicalDemo.Api.Services;

public class AppointmentPrerequisiteService(
    IAppointmentPrerequisiteRepository prerequisiteRepository,
    IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository,
    ILogger<AppointmentPrerequisiteService> logger) : IAppointmentPrerequisiteService
{
    public IReadOnlyCollection<AppointmentPrerequisiteDto> GetByAppointmentId(int appointmentId)
    {
        EnsureAppointmentExists(appointmentId);

        return prerequisiteRepository.GetByAppointmentId(appointmentId)
            .OrderBy(item => item.Kind)
            .Select(MapToDto)
            .ToList()
            .AsReadOnly();
    }

    public AppointmentPrerequisiteDto Create(CreateAppointmentPrerequisiteDTO request)
    {
        var appointment = appointmentRepository.GetById(request.AppointmentId)
            ?? throw new KeyNotFoundException($"Appointment with id {request.AppointmentId} was not found.");

        var patient = patientRepository.GetById(request.PatientId)
            ?? throw new KeyNotFoundException($"Patient with id {request.PatientId} was not found.");

        if (appointment.PatientId != patient.Id)
        {
            throw new InvalidOperationException("The prerequisite patient must match the appointment patient.");
        }

        var prerequisite = new AppointmentPrerequisite
        {
            AppointmentId = appointment.Id,
            PatientId = patient.Id,
            Kind = NormalizeKind(request.Kind),
            Status = NormalizeStatus(request.Status),
            DueDate = request.DueDate,
            ExpiresOn = request.ExpiresOn,
            Notes = request.Notes.Trim()
        };

        var saved = prerequisiteRepository.UpsertActive(prerequisite);

        logger.LogInformation(
            "Prerequisite {Kind} upserted for appointment {AppointmentId} with status {Status}.",
            saved.Kind,
            saved.AppointmentId,
            saved.Status);

        return MapToDto(saved);
    }

    public AppointmentPrerequisiteDto Update(int id, UpdateAppointmentPrerequisiteDTO request)
    {
        var existing = prerequisiteRepository.GetById(id)
            ?? throw new KeyNotFoundException($"Prerequisite with id {id} was not found.");

        EnsureAppointmentPatientLink(existing.AppointmentId, existing.PatientId);

        existing.Status = NormalizeStatus(request.Status);
        existing.DueDate = request.DueDate;
        existing.ExpiresOn = request.ExpiresOn;
        existing.Notes = request.Notes.Trim();

        var updated = prerequisiteRepository.Update(existing);

        logger.LogInformation(
            "Prerequisite {PrerequisiteId} updated to {Status}.",
            updated.Id,
            updated.Status);

        return MapToDto(updated);
    }

    public AppointmentPrerequisiteSummaryDTO GetAppointmentSummary(int appointmentId, string kind)
    {
        var appointment = appointmentRepository.GetById(appointmentId)
            ?? throw new KeyNotFoundException($"Appointment with id {appointmentId} was not found.");

        var normalizedKind = NormalizeKind(kind);
        var record = prerequisiteRepository.GetByAppointmentId(appointmentId)
            .FirstOrDefault(item => item.Kind == normalizedKind);

        if (record is null)
        {
            return new AppointmentPrerequisiteSummaryDTO
            {
                Kind = normalizedKind,
                IsRequired = false,
                Status = "notRequired",
                IsBlocking = false
            };
        }

        var visitDate = DateOnly.FromDateTime(appointment.AppointmentDate);
        var summaryStatus = ResolveSummaryStatus(record, visitDate);

        return new AppointmentPrerequisiteSummaryDTO
        {
            Id = record.Id,
            Kind = normalizedKind,
            IsRequired = true,
            Status = summaryStatus,
            DueDate = record.DueDate,
            ExpiresOn = record.ExpiresOn,
            Notes = record.Notes,
            IsBlocking = IsBlockingStatus(summaryStatus)
        };
    }

    private void EnsureAppointmentExists(int appointmentId)
    {
        _ = appointmentRepository.GetById(appointmentId)
            ?? throw new KeyNotFoundException($"Appointment with id {appointmentId} was not found.");
    }

    private void EnsureAppointmentPatientLink(int appointmentId, int patientId)
    {
        var appointment = appointmentRepository.GetById(appointmentId)
            ?? throw new KeyNotFoundException($"Appointment with id {appointmentId} was not found.");

        if (appointment.PatientId != patientId)
        {
            throw new InvalidOperationException("The prerequisite patient must match the appointment patient.");
        }
    }

    private static AppointmentPrerequisiteDto MapToDto(AppointmentPrerequisite prerequisite) =>
        new()
        {
            Id = prerequisite.Id,
            AppointmentId = prerequisite.AppointmentId,
            PatientId = prerequisite.PatientId,
            Kind = prerequisite.Kind,
            Status = prerequisite.Status,
            DueDate = prerequisite.DueDate,
            ExpiresOn = prerequisite.ExpiresOn,
            Notes = prerequisite.Notes
        };

    private static string NormalizeKind(string kind)
    {
        var normalized = string.IsNullOrWhiteSpace(kind) ? "authorization" : kind.Trim().ToLowerInvariant();
        return normalized is "authorization" or "referral"
            ? normalized
            : throw new InvalidOperationException("Prerequisite kind must be authorization or referral.");
    }

    private static string NormalizeStatus(string status)
    {
        var normalized = string.IsNullOrWhiteSpace(status) ? "needed" : status.Trim().ToLowerInvariant();
        return normalized is "needed" or "submitted" or "approved" or "denied"
            ? normalized
            : throw new InvalidOperationException("Prerequisite status must be needed, submitted, approved, or denied.");
    }

    private static string ResolveSummaryStatus(AppointmentPrerequisite prerequisite, DateOnly visitDate) =>
        prerequisite.ExpiresOn is DateOnly expiresOn && expiresOn < visitDate
            ? "expired"
            : prerequisite.Status;

    private static bool IsBlockingStatus(string status) =>
        status is "needed" or "submitted" or "denied" or "expired";
}
