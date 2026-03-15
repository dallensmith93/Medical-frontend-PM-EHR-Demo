using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Api.DTOs;

public class CreateAppointmentDTO
{
    [Required]
    [Range(1, int.MaxValue)]
    public int PatientId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int ProviderId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [Range(1, 1440)]
    public int DurationMinutes { get; set; }

    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;
}
