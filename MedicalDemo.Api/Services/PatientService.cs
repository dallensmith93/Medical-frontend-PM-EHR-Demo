using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Models;
using MedicalDemo.Api.Repositories;

namespace MedicalDemo.Api.Services;

public class PatientService(IPatientRepository patientRepository) : IPatientService
{
    public IReadOnlyCollection<PatientDto> GetAll() =>
        patientRepository.GetAll()
            .Select(MapToDto)
            .ToList()
            .AsReadOnly();

    public PatientDto Create(CreatePatientDTO request)
    {
        var patient = new Patient
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            DateOfBirth = request.DateOfBirth
        };

        return MapToDto(patientRepository.Add(patient));
    }

    private static PatientDto MapToDto(Patient patient) =>
        new()
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth
        };
}
