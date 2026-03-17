using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Api.DTOs;

public class CreatePatientDTO
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public DateOnly DateOfBirth { get; set; }

    [StringLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [StringLength(150)]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [StringLength(50)]
    public string State { get; set; } = string.Empty;

    [StringLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    [StringLength(100)]
    public string PayerName { get; set; } = string.Empty;

    [StringLength(100)]
    public string MemberId { get; set; } = string.Empty;

    [StringLength(250)]
    public string InsuranceSummary { get; set; } = string.Empty;

    [Required]
    [RegularExpression("verified|pending|failed")]
    public string EligibilityStatus { get; set; } = "pending";

    public DateTime? LastEligibilityVerifiedAt { get; set; }

    [StringLength(500)]
    public string EligibilityNotes { get; set; } = string.Empty;

    [Required]
    [RegularExpression("notStarted|inProgress|complete")]
    public string IntakeStatus { get; set; } = "notStarted";

    [StringLength(1000)]
    public string IntakeNotes { get; set; } = string.Empty;
}
