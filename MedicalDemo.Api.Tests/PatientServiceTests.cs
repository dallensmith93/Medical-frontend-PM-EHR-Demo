using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Repositories;
using MedicalDemo.Api.Services;
using Xunit;

namespace MedicalDemo.Api.Tests;

public class PatientServiceTests
{
    [Fact]
    public void Create_NormalizesEligibilityStatusAndTrimsIntakeFields()
    {
        var repository = new InMemoryPatientRepository();
        var service = new PatientService(repository);

        var created = service.Create(new CreatePatientDTO
        {
            FirstName = "  Jordan ",
            LastName = " Lee  ",
            DateOfBirth = new DateOnly(1992, 4, 15),
            PayerName = "  Blue Cross  ",
            MemberId = "  BC-1002  ",
            EligibilityStatus = " VERIFIED ",
            EligibilityNotes = "  Portal confirmed active coverage.  "
        });

        Assert.Equal("Jordan", created.FirstName);
        Assert.Equal("Lee", created.LastName);
        Assert.Equal("Blue Cross", created.PayerName);
        Assert.Equal("BC-1002", created.MemberId);
        Assert.Equal("verified", created.EligibilityStatus);
        Assert.Equal("Portal confirmed active coverage.", created.EligibilityNotes);
    }

    [Fact]
    public void Update_ReplacesStoredPatientAndDefaultsBlankEligibilityToPending()
    {
        var repository = new InMemoryPatientRepository();
        var service = new PatientService(repository);

        var updated = service.Update(2, new CreatePatientDTO
        {
            FirstName = "  Ethan ",
            LastName = " Brooks  ",
            DateOfBirth = new DateOnly(1978, 11, 3),
            PayerName = "  United Healthcare ",
            MemberId = "  UHC-7788 ",
            EligibilityStatus = "   ",
            LastEligibilityVerifiedAt = new DateTime(2026, 3, 16, 15, 30, 0, DateTimeKind.Utc),
            EligibilityNotes = "  Waiting on updated card.  "
        });

        Assert.Equal(2, updated.Id);
        Assert.Equal("United Healthcare", updated.PayerName);
        Assert.Equal("UHC-7788", updated.MemberId);
        Assert.Equal("pending", updated.EligibilityStatus);
        Assert.Equal("Waiting on updated card.", updated.EligibilityNotes);
        Assert.Equal(new DateTime(2026, 3, 16, 15, 30, 0, DateTimeKind.Utc), updated.LastEligibilityVerifiedAt);
    }

    [Fact]
    public void Update_ThrowsWhenPatientDoesNotExist()
    {
        var repository = new InMemoryPatientRepository();
        var service = new PatientService(repository);

        var exception = Assert.Throws<KeyNotFoundException>(() => service.Update(99, new CreatePatientDTO
        {
            FirstName = "Test",
            LastName = "Patient",
            DateOfBirth = new DateOnly(2000, 1, 1),
            EligibilityStatus = "pending"
        }));

        Assert.Contains("99", exception.Message);
    }
}
