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
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<AppointmentResponseDTO> CreateAppointment([FromBody] CreateAppointmentDTO request)
    {
        try
        {
            var appointment = appointmentService.Create(request);
            return CreatedAtAction(nameof(GetAppointments), new { id = appointment.Id }, appointment);
        }
        catch (SchedulingConflictException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }

    [HttpPut("{id:int}/eligibility")]
    [ProducesResponseType(typeof(AppointmentResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<AppointmentResponseDTO> UpdateEligibility(int id, [FromBody] UpdateAppointmentEligibilityDTO request)
    {
        try
        {
            return Ok(appointmentService.UpdateEligibility(id, request));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }
}
