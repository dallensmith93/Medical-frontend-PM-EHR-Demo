using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Api.DTOs;

public class UpdateAppointmentChargeDTO
{
    [StringLength(20)]
    public string DiagnosisCode { get; set; } = string.Empty;

    [StringLength(20)]
    public string ProcedureCode { get; set; } = string.Empty;

    [StringLength(10)]
    public string Modifier { get; set; } = string.Empty;

    [Range(0, 99)]
    public int Units { get; set; }

    [Range(0, 999999)]
    public decimal Amount { get; set; }

    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;
}
