namespace MedicalDemo.Api.DTOs;

public class AppointmentResponseDTO
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int ProviderId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public int DurationMinutes { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string EligibilityStatus { get; set; } = "pending";

    public DateTime? EligibilityReviewedAt { get; set; }

    public string EligibilityNotes { get; set; } = string.Empty;

    public string IntakeStatus { get; set; } = "notStarted";

    public bool IsIntakeComplete { get; set; }

    public IReadOnlyCollection<string> MissingIntakeItems { get; set; } = [];

    public AppointmentPrerequisiteSummaryDTO Authorization { get; set; } = new()
    {
        Kind = "authorization"
    };

    public AppointmentPrerequisiteSummaryDTO Referral { get; set; } = new()
    {
        Kind = "referral"
    };

    public bool HasPrerequisiteBlocker { get; set; }

    public AppointmentBillingSummaryDTO Billing { get; set; } = new();
}
