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
            DateOfBirth = request.DateOfBirth,
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = request.Email.Trim(),
            AddressLine1 = request.AddressLine1.Trim(),
            City = request.City.Trim(),
            State = request.State.Trim(),
            PostalCode = request.PostalCode.Trim(),
            PayerName = request.PayerName.Trim(),
            MemberId = request.MemberId.Trim(),
            InsuranceSummary = request.InsuranceSummary.Trim(),
            EligibilityStatus = NormalizeStatus(request.EligibilityStatus),
            LastEligibilityVerifiedAt = request.LastEligibilityVerifiedAt,
            EligibilityNotes = request.EligibilityNotes.Trim(),
            IntakeStatus = NormalizeIntakeStatus(request.IntakeStatus),
            IntakeNotes = request.IntakeNotes.Trim()
        };

        patient.IntakeStatus = ResolvePersistedIntakeStatus(patient, patient.IntakeStatus);

        return MapToDto(patientRepository.Add(patient));
    }

    public PatientDto Update(int id, CreatePatientDTO request)
    {
        var existingPatient = patientRepository.GetById(id)
            ?? throw new KeyNotFoundException($"Patient with id {id} was not found.");

        existingPatient.FirstName = request.FirstName.Trim();
        existingPatient.LastName = request.LastName.Trim();
        existingPatient.DateOfBirth = request.DateOfBirth;
        existingPatient.PhoneNumber = request.PhoneNumber.Trim();
        existingPatient.Email = request.Email.Trim();
        existingPatient.AddressLine1 = request.AddressLine1.Trim();
        existingPatient.City = request.City.Trim();
        existingPatient.State = request.State.Trim();
        existingPatient.PostalCode = request.PostalCode.Trim();
        existingPatient.PayerName = request.PayerName.Trim();
        existingPatient.MemberId = request.MemberId.Trim();
        existingPatient.InsuranceSummary = request.InsuranceSummary.Trim();
        existingPatient.EligibilityStatus = NormalizeStatus(request.EligibilityStatus);
        existingPatient.LastEligibilityVerifiedAt = request.LastEligibilityVerifiedAt;
        existingPatient.EligibilityNotes = request.EligibilityNotes.Trim();
        existingPatient.IntakeNotes = request.IntakeNotes.Trim();
        existingPatient.IntakeStatus = ResolvePersistedIntakeStatus(
            existingPatient,
            NormalizeIntakeStatus(request.IntakeStatus));

        return MapToDto(patientRepository.Update(existingPatient));
    }

    private static PatientDto MapToDto(Patient patient) =>
        new()
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            PhoneNumber = patient.PhoneNumber,
            Email = patient.Email,
            AddressLine1 = patient.AddressLine1,
            City = patient.City,
            State = patient.State,
            PostalCode = patient.PostalCode,
            PayerName = patient.PayerName,
            MemberId = patient.MemberId,
            InsuranceSummary = patient.InsuranceSummary,
            EligibilityStatus = patient.EligibilityStatus,
            LastEligibilityVerifiedAt = patient.LastEligibilityVerifiedAt,
            EligibilityNotes = patient.EligibilityNotes,
            IntakeStatus = patient.IntakeStatus,
            IntakeNotes = patient.IntakeNotes,
            MissingIntakeItems = GetMissingIntakeItems(patient)
        };

    private static string NormalizeStatus(string status) =>
        string.IsNullOrWhiteSpace(status) ? "pending" : status.Trim().ToLowerInvariant();

    public static string NormalizeIntakeStatus(string status) =>
        string.IsNullOrWhiteSpace(status) ? "notStarted" : status.Trim();

    public static IReadOnlyCollection<string> GetMissingIntakeItems(Patient patient)
    {
        var missingItems = new List<string>();

        if (string.IsNullOrWhiteSpace(patient.PhoneNumber))
        {
            missingItems.Add("phone");
        }

        if (string.IsNullOrWhiteSpace(patient.Email))
        {
            missingItems.Add("email");
        }

        if (string.IsNullOrWhiteSpace(patient.InsuranceSummary))
        {
            missingItems.Add("insurance summary");
        }

        if (string.IsNullOrWhiteSpace(patient.IntakeNotes))
        {
            missingItems.Add("intake notes");
        }

        return missingItems.AsReadOnly();
    }

    private static string ResolvePersistedIntakeStatus(Patient patient, string requestedStatus)
    {
        var normalizedStatus = requestedStatus switch
        {
            "complete" => "complete",
            "inProgress" => "inProgress",
            _ => "notStarted"
        };

        if (normalizedStatus == "complete" && GetMissingIntakeItems(patient).Count > 0)
        {
            return "inProgress";
        }

        return normalizedStatus;
    }
}
