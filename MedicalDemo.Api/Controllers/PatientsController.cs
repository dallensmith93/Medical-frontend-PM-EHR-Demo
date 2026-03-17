using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController(IPatientService patientService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PatientDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<PatientDto>> GetPatients() =>
        Ok(patientService.GetAll());

    [HttpPost]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<PatientDto> CreatePatient([FromBody] CreatePatientDTO request)
    {
        var patient = patientService.Create(request);
        return CreatedAtAction(nameof(GetPatients), new { id = patient.Id }, patient);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PatientDto> UpdatePatient(int id, [FromBody] CreatePatientDTO request)
    {
        try
        {
            return Ok(patientService.Update(id, request));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }
}
