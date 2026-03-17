namespace MedicalDemo.Api.DTOs;

public class AppointmentPrerequisiteSummaryDTO
{
    public int? Id { get; set; }

    public string Kind { get; set; } = "authorization";

    public bool IsRequired { get; set; }

    public string Status { get; set; } = "notRequired";

    public DateOnly? DueDate { get; set; }

    public DateOnly? ExpiresOn { get; set; }

    public string Notes { get; set; } = string.Empty;

    public bool IsBlocking { get; set; }
}
