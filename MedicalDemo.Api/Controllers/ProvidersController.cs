using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProvidersController(IProviderService providerService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProviderDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<ProviderDto>> GetProviders() =>
        Ok(providerService.GetAll());
}
