namespace MedicalDemo.Api.DTOs;

public class AppointmentResponseDTO
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int ProviderId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public int DurationMinutes { get; set; }

    public string Reason { get; set; } = string.Empty;
}
