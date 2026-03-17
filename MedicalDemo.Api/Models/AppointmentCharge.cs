namespace MedicalDemo.Api.Models;

public class AppointmentCharge
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int ProviderId { get; set; }

    public string DiagnosisCode { get; set; } = string.Empty;

    public string ProcedureCode { get; set; } = string.Empty;

    public string Modifier { get; set; } = string.Empty;

    public int Units { get; set; } = 1;

    public decimal Amount { get; set; }

    public string Notes { get; set; } = string.Empty;
}
