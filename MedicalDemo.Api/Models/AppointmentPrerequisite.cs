namespace MedicalDemo.Api.Models;

public class AppointmentPrerequisite
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
