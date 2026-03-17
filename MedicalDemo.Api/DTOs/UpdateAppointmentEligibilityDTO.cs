using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Api.DTOs;

public class UpdateAppointmentEligibilityDTO
{
    [Required]
    [RegularExpression("verified|pending|failed")]
    public string EligibilityStatus { get; set; } = "pending";

    public DateTime? EligibilityReviewedAt { get; set; }

    [StringLength(500)]
    public string EligibilityNotes { get; set; } = string.Empty;
}
