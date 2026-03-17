using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Repositories;
using MedicalDemo.Api.Services;
using Xunit;

namespace MedicalDemo.Api.Tests;

public class PatientServiceTests
{
    private readonly PatientService _service = new(new InMemoryPatientRepository());

    [Fact]
    public void Update_DemotesCompleteIntakeWhenRequiredFieldsAreMissing()
    {
        var updatedPatient = _service.Update(2, new CreatePatientDTO
        {
            FirstName = "Ethan",
            LastName = "Brooks",
            DateOfBirth = new DateOnly(1978, 11, 3),
            PhoneNumber = "",
            Email = "ethan@example.com",
            AddressLine1 = "4480 W 29th Ave",
            City = "Denver",
            State = "CO",
            PostalCode = "80212",
            PayerName = "Blue Cross",
            MemberId = "BCB-552190",
            InsuranceSummary = "",
            EligibilityStatus = "pending",
            EligibilityNotes = "Needs recheck",
            IntakeStatus = "complete",
            IntakeNotes = ""
        });

        Assert.Equal("inProgress", updatedPatient.IntakeStatus);
        Assert.Contains("phone", updatedPatient.MissingIntakeItems);
        Assert.Contains("insurance summary", updatedPatient.MissingIntakeItems);
        Assert.Contains("intake notes", updatedPatient.MissingIntakeItems);
    }

    [Fact]
    public void Update_KeepsCompleteStatusWhenRequiredFieldsArePresent()
    {
        var updatedPatient = _service.Update(3, new CreatePatientDTO
        {
            FirstName = "Sophia",
            LastName = "Nguyen",
            DateOfBirth = new DateOnly(2016, 7, 21),
            PhoneNumber = "303-555-0001",
            Email = "sophia@example.com",
            AddressLine1 = "91 Elm Ct",
            City = "Aurora",
            State = "CO",
            PostalCode = "80011",
            PayerName = "Cigna",
            MemberId = "CIG-104822",
            InsuranceSummary = "Cigna PPO",
            EligibilityStatus = "verified",
            EligibilityNotes = "Cleared",
            IntakeStatus = "complete",
            IntakeNotes = "Guardian forms received"
        });

        Assert.Equal("complete", updatedPatient.IntakeStatus);
        Assert.Empty(updatedPatient.MissingIntakeItems);
    }
}
