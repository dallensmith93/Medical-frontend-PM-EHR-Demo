using MedicalDemo.Api.Models;

namespace MedicalDemo.Api.Repositories;

public class InMemoryPatientRepository : IPatientRepository
{
    private readonly List<Patient> _patients = [];
    private readonly object _syncRoot = new();
    private int _nextId = 1;

    public IReadOnlyCollection<Patient> GetAll()
    {
        lock (_syncRoot)
        {
            return _patients
                .Select(Clone)
                .ToList()
                .AsReadOnly();
        }
    }

    public Patient Add(Patient patient)
    {
        lock (_syncRoot)
        {
            var storedPatient = Clone(patient);
            storedPatient.Id = _nextId++;
            _patients.Add(storedPatient);

            return Clone(storedPatient);
        }
    }

    public bool Exists(int id)
    {
        lock (_syncRoot)
        {
            return _patients.Any(patient => patient.Id == id);
        }
    }

    private static Patient Clone(Patient patient) =>
        new()
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth
        };
}
