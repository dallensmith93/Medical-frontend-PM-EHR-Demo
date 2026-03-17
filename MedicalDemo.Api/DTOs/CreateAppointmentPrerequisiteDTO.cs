using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Api.DTOs;

public class CreateAppointmentPrerequisiteDTO
{
    [Required]
    [Range(1, int.MaxValue)]
    public int AppointmentId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int PatientId { get; set; }

    [Required]
    [RegularExpression("authorization|referral", ErrorMessage = "Kind must be authorization or referral.")]
    public string Kind { get; set; } = "authorization";

    [Required]
    [RegularExpression("needed|submitted|approved|denied", ErrorMessage = "Status must be needed, submitted, approved, or denied.")]
    public string Status { get; set; } = "needed";

    public DateOnly? DueDate { get; set; }

    public DateOnly? ExpiresOn { get; set; }

    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;
}
