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
}
