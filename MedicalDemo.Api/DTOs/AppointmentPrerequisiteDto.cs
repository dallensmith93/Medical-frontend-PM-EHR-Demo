namespace MedicalDemo.Api.DTOs;

public class AppointmentPrerequisiteDto
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public string Kind { get; set; } = "authorization";

    public string Status { get; set; } = "needed";

    public DateOnly? DueDate { get; set; }

    public DateOnly? ExpiresOn { get; set; }

    public string Notes { get; set; } = string.Empty;
}
