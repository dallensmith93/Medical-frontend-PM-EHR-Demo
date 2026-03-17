namespace MedicalDemo.Api.DTOs;

public class PatientDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string AddressLine1 { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string PayerName { get; set; } = string.Empty;

    public string MemberId { get; set; } = string.Empty;

    public string InsuranceSummary { get; set; } = string.Empty;

    public string EligibilityStatus { get; set; } = "pending";

    public DateTime? LastEligibilityVerifiedAt { get; set; }

    public string EligibilityNotes { get; set; } = string.Empty;

    public string IntakeStatus { get; set; } = "notStarted";

    public string IntakeNotes { get; set; } = string.Empty;

    public IReadOnlyCollection<string> MissingIntakeItems { get; set; } = [];
}
