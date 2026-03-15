using MedicalDemo.Api.Models;

namespace MedicalDemo.Api.Repositories;

public interface IProviderRepository
{
    IReadOnlyCollection<Provider> GetAll();

    bool Exists(int id);
}
