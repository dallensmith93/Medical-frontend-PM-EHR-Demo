using MedicalDemo.Api.Models;

namespace MedicalDemo.Api.Repositories;

public interface IPatientRepository
{
    IReadOnlyCollection<Patient> GetAll();

    Patient? GetById(int id);

    Patient Add(Patient patient);

    Patient Update(Patient patient);

    bool Exists(int id);
}
