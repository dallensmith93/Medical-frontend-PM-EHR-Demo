using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController(IAppointmentService appointmentService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AppointmentResponseDTO>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<AppointmentResponseDTO>> GetAppointments() =>
        Ok(appointmentService.GetAll());

    [HttpPost]
    [ProducesResponseType(typeof(AppointmentResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<AppointmentResponseDTO> CreateAppointment([FromBody] CreateAppointmentDTO request)
    {
        try
        {
            var appointment = appointmentService.Create(request);
            return CreatedAtAction(nameof(GetAppointments), new { id = appointment.Id }, appointment);
        }
        catch (SchedulingConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
