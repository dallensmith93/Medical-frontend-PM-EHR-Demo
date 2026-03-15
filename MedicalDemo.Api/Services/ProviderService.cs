using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Models;
using MedicalDemo.Api.Repositories;

namespace MedicalDemo.Api.Services;

public class ProviderService(IProviderRepository providerRepository) : IProviderService
{
    public IReadOnlyCollection<ProviderDto> GetAll() =>
        providerRepository.GetAll()
            .Select(MapToDto)
            .ToList()
            .AsReadOnly();

    private static ProviderDto MapToDto(Provider provider) =>
        new()
        {
            Id = provider.Id,
            Name = provider.Name,
            Specialty = provider.Specialty
        };
}
