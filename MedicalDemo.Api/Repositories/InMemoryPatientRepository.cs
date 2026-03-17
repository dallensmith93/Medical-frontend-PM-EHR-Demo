using MedicalDemo.Api.Models;

namespace MedicalDemo.Api.Repositories;

public class InMemoryPatientRepository : IPatientRepository
{
    private readonly List<Patient> _patients =
    [
        new()
        {
            Id = 1,
            FirstName = "Maya",
            LastName = "Robinson",
            DateOfBirth = new DateOnly(1989, 4, 12),
            PhoneNumber = "303-555-0182",
            Email = "maya.robinson@example.com",
            AddressLine1 = "1024 Grant St",
            City = "Denver",
            State = "CO",
            PostalCode = "80203",
            PayerName = "Aetna",
            MemberId = "AET-884421",
            InsuranceSummary = "Aetna PPO",
            EligibilityStatus = "verified",
            LastEligibilityVerifiedAt = DateTime.UtcNow.AddHours(-4),
            EligibilityNotes = "Commercial plan active through end of month.",
            IntakeStatus = "complete",
            IntakeNotes = "Demographics and insurance confirmed."
        },
        new()
        {
            Id = 2,
            FirstName = "Ethan",
            LastName = "Brooks",
            DateOfBirth = new DateOnly(1978, 11, 3),
            PhoneNumber = "720-555-0198",
            Email = "ethan.brooks@example.com",
            AddressLine1 = "4480 W 29th Ave",
            City = "Denver",
            State = "CO",
            PostalCode = "80212",
            PayerName = "Blue Cross",
            MemberId = "BCB-552190",
            InsuranceSummary = "Blue Cross HMO",
            EligibilityStatus = "pending",
            EligibilityNotes = "Recheck needed before cardiology follow-up.",
            IntakeStatus = "inProgress",
            IntakeNotes = ""
        },
        new()
        {
            Id = 3,
            FirstName = "Sophia",
            LastName = "Nguyen",
            DateOfBirth = new DateOnly(2016, 7, 21),
            PhoneNumber = "",
            Email = "",
            AddressLine1 = "91 Elm Ct",
            City = "Aurora",
            State = "CO",
            PostalCode = "80011",
            PayerName = "Cigna",
            MemberId = "CIG-104822",
            InsuranceSummary = "",
            EligibilityStatus = "failed",
            LastEligibilityVerifiedAt = DateTime.UtcNow.AddDays(-1),
            EligibilityNotes = "Subscriber ID mismatch reported by payer portal.",
            IntakeStatus = "notStarted",
            IntakeNotes = ""
        }
    ];
    private readonly object _syncRoot = new();
    private int _nextId = 4;

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

    public Patient? GetById(int id)
    {
        lock (_syncRoot)
        {
            var patient = _patients.FirstOrDefault(item => item.Id == id);
            return patient is null ? null : Clone(patient);
        }
    }

    public Patient Update(Patient patient)
    {
        lock (_syncRoot)
        {
            var index = _patients.FindIndex(existingPatient => existingPatient.Id == patient.Id);
            if (index < 0)
            {
                throw new KeyNotFoundException($"Patient with id {patient.Id} was not found.");
            }

            var storedPatient = Clone(patient);
            _patients[index] = storedPatient;
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
            IntakeNotes = patient.IntakeNotes
        };
}
