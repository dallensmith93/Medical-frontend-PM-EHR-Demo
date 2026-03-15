using MedicalDemo.Api.Models;

namespace MedicalDemo.Api.Repositories;

public interface IPatientRepository
{
    IReadOnlyCollection<Patient> GetAll();

    Patient Add(Patient patient);

    bool Exists(int id);
}
