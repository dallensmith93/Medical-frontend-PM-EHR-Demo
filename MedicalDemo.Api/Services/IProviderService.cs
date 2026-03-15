using MedicalDemo.Api.DTOs;

namespace MedicalDemo.Api.Services;

public interface IProviderService
{
    IReadOnlyCollection<ProviderDto> GetAll();
}
