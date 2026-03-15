namespace MedicalDemo.Api.Services;

public class SchedulingConflictException(string message) : Exception(message);
