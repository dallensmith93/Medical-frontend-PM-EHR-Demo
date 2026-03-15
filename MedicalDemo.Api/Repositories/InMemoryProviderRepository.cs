using MedicalDemo.Api.Models;

namespace MedicalDemo.Api.Repositories;

public class InMemoryProviderRepository : IProviderRepository
{
    private static readonly IReadOnlyCollection<Provider> Providers =
    [
        new Provider { Id = 1, Name = "Dr. Sarah Chen", Specialty = "Family Medicine" },
        new Provider { Id = 2, Name = "Dr. Michael Alvarez", Specialty = "Cardiology" },
        new Provider { Id = 3, Name = "Dr. Priya Patel", Specialty = "Pediatrics" }
    ];

    public IReadOnlyCollection<Provider> GetAll() =>
        Providers
            .Select(provider => new Provider
            {
                Id = provider.Id,
                Name = provider.Name,
                Specialty = provider.Specialty
            })
            .ToList()
            .AsReadOnly();

    public bool Exists(int id) => Providers.Any(provider => provider.Id == id);
}
