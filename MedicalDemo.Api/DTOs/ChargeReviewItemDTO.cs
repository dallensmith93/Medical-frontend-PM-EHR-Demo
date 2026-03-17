namespace MedicalDemo.Api.DTOs;

public class ChargeReviewItemDTO
{
    public int AppointmentId { get; set; }

    public int? ChargeId { get; set; }

    public int PatientId { get; set; }

    public string PatientName { get; set; } = string.Empty;

    public string PayerName { get; set; } = string.Empty;

    public int ProviderId { get; set; }

    public string ProviderName { get; set; } = string.Empty;

    public DateTime AppointmentDate { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string Status { get; set; } = "draft";

    public int WarningCount { get; set; }

    public IReadOnlyCollection<ClaimScrubWarningDTO> Warnings { get; set; } = [];
}
