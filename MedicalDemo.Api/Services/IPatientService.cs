using MedicalDemo.Api.DTOs;

namespace MedicalDemo.Api.Services;

public interface IPatientService
{
    IReadOnlyCollection<PatientDto> GetAll();

    PatientDto Create(CreatePatientDTO request);

    PatientDto Update(int id, CreatePatientDTO request);
}
