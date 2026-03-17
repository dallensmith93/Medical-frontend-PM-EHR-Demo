namespace MedicalDemo.Api.DTOs;

public class AppointmentChargeDto
{
    public int? Id { get; set; }

    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int ProviderId { get; set; }

    public string DiagnosisCode { get; set; } = string.Empty;

    public string ProcedureCode { get; set; } = string.Empty;

    public string Modifier { get; set; } = string.Empty;

    public int Units { get; set; }

    public decimal Amount { get; set; }

    public string Notes { get; set; } = string.Empty;

    public AppointmentBillingSummaryDTO Billing { get; set; } = new();
}
