using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalDemo.Api.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentPrerequisitesController(IAppointmentPrerequisiteService prerequisiteService) : ControllerBase
{
    [HttpGet("{appointmentId:int}/prerequisites")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AppointmentPrerequisiteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IReadOnlyCollection<AppointmentPrerequisiteDto>> GetByAppointment(int appointmentId)
    {
        try
        {
            return Ok(prerequisiteService.GetByAppointmentId(appointmentId));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }

    [HttpPost("{appointmentId:int}/prerequisites")]
    [ProducesResponseType(typeof(AppointmentPrerequisiteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<AppointmentPrerequisiteDto> Create(int appointmentId, [FromBody] CreateAppointmentPrerequisiteDTO request)
    {
        try
        {
            request.AppointmentId = appointmentId;
            var prerequisite = prerequisiteService.Create(request);
            return CreatedAtAction(nameof(GetByAppointment), new { appointmentId }, prerequisite);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("prerequisites/{prerequisiteId:int}")]
    [ProducesResponseType(typeof(AppointmentPrerequisiteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<AppointmentPrerequisiteDto> Update(int prerequisiteId, [FromBody] UpdateAppointmentPrerequisiteDTO request)
    {
        try
        {
            return Ok(prerequisiteService.Update(prerequisiteId, request));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}
