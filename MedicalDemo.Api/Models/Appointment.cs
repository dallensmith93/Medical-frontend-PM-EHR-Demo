namespace MedicalDemo.Api.Models;

public class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int ProviderId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public int DurationMinutes { get; set; }

    public string Reason { get; set; } = string.Empty;
}
