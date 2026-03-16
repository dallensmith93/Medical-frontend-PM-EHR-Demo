using MedicalDemo.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProvidersController : ControllerBase
{
    private static readonly List<Provider> Providers = [];

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<Provider>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<Provider>> GetProviders() =>
        Ok(Providers);

    [HttpPost]
    [ProducesResponseType(typeof(Provider), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Provider> CreateProvider([FromBody] Provider provider)
    {
        if (provider.Id == 0)
        {
            provider.Id = Providers.Count == 0 ? 1 : Providers.Max(existing => existing.Id) + 1;
        }

        Providers.Add(provider);

        return CreatedAtAction(nameof(GetProviders), new { id = provider.Id }, provider);
    }
}
