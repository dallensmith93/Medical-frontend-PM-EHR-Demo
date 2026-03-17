using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Api.DTOs;

public class UpdateAppointmentPrerequisiteDTO
{
    [Required]
    [RegularExpression("needed|submitted|approved|denied", ErrorMessage = "Status must be needed, submitted, approved, or denied.")]
    public string Status { get; set; } = "needed";

    public DateOnly? DueDate { get; set; }

    public DateOnly? ExpiresOn { get; set; }

    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;
}
