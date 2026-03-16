using MedicalDemo.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController(ILogger<AppointmentsController> logger) : ControllerBase
{
    private static readonly List<Appointment> Appointments = [];

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<Appointment>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<Appointment>> GetAppointments() =>
        Ok(Appointments);

    [HttpPost]
    [ProducesResponseType(typeof(Appointment), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<Appointment> CreateAppointment([FromBody] Appointment appointment)
    {
        var requestedStart = appointment.AppointmentDate;
        var requestedEnd = appointment.AppointmentDate.AddMinutes(appointment.DurationMinutes);

        var conflictExists = Appointments.Any(existingAppointment =>
        {
            if (existingAppointment.ProviderId != appointment.ProviderId)
            {
                return false;
            }

            var existingAppointmentStart = existingAppointment.AppointmentDate;
            var existingAppointmentEnd = existingAppointment.AppointmentDate.AddMinutes(existingAppointment.DurationMinutes);

            return requestedStart < existingAppointmentEnd && existingAppointmentStart < requestedEnd;
        });

        if (conflictExists)
        {
            logger.LogWarning(
                "Scheduling conflict detected for provider {ProviderId}",
                appointment.ProviderId);
            return Conflict(new { message = "Provider already has an appointment scheduled during this time." });
        }

        if (appointment.Id == 0)
        {
            appointment.Id = Appointments.Count == 0 ? 1 : Appointments.Max(existing => existing.Id) + 1;
        }

        Appointments.Add(appointment);
        logger.LogInformation(
            "Appointment created for provider {ProviderId} at {AppointmentDate}",
            appointment.ProviderId,
            appointment.AppointmentDate);

        return CreatedAtAction(nameof(GetAppointments), new { id = appointment.Id }, appointment);
    }
}
