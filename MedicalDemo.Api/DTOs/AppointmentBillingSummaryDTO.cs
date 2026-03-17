namespace MedicalDemo.Api.DTOs;

public class AppointmentBillingSummaryDTO
{
    public int? ChargeId { get; set; }

    public string Status { get; set; } = "draft";

    public bool IsReadyToSubmit { get; set; }

    public int WarningCount { get; set; }

    public IReadOnlyCollection<ClaimScrubWarningDTO> Warnings { get; set; } = [];
}
