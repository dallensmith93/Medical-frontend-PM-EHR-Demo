using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalDemo.Api.Controllers;

[ApiController]
public class AppointmentChargesController(IAppointmentChargeService appointmentChargeService) : ControllerBase
{
    [HttpGet("/api/appointments/{appointmentId:int}/charge")]
    [ProducesResponseType(typeof(AppointmentChargeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<AppointmentChargeDto> GetCharge(int appointmentId)
    {
        try
        {
            return Ok(appointmentChargeService.GetByAppointmentId(appointmentId));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }

    [HttpPost("/api/appointments/{appointmentId:int}/charge")]
    [ProducesResponseType(typeof(AppointmentChargeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<AppointmentChargeDto> CreateCharge(int appointmentId, [FromBody] CreateAppointmentChargeDTO request)
    {
        try
        {
            return Ok(appointmentChargeService.Create(appointmentId, request));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }

    [HttpPut("/api/appointments/{appointmentId:int}/charge")]
    [ProducesResponseType(typeof(AppointmentChargeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<AppointmentChargeDto> UpdateCharge(int appointmentId, [FromBody] UpdateAppointmentChargeDTO request)
    {
        try
        {
            return Ok(appointmentChargeService.Update(appointmentId, request));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }

    [HttpGet("/api/charges/review")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ChargeReviewItemDTO>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<ChargeReviewItemDTO>> GetReviewQueue() =>
        Ok(appointmentChargeService.GetReviewQueue());
}
