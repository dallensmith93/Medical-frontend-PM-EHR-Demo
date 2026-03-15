using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Models;
using MedicalDemo.Api.Repositories;
using Microsoft.Extensions.Logging;

namespace MedicalDemo.Api.Services;

public class AppointmentService(
    IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository,
    IProviderRepository providerRepository,
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
            Reason = request.Reason.Trim()
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

    private static AppointmentResponseDTO MapToDto(Appointment appointment) =>
        new()
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            ProviderId = appointment.ProviderId,
            AppointmentDate = appointment.AppointmentDate,
            DurationMinutes = appointment.DurationMinutes,
            Reason = appointment.Reason
        };
}
