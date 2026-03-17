namespace MedicalDemo.Api.DTOs;

public class ClaimScrubWarningDTO
{
    public string Code { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsBlocking { get; set; } = true;
}
