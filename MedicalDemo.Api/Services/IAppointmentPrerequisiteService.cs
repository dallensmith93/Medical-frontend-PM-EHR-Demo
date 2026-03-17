using MedicalDemo.Api.DTOs;

namespace MedicalDemo.Api.Services;

public interface IAppointmentPrerequisiteService
{
    IReadOnlyCollection<AppointmentPrerequisiteDto> GetByAppointmentId(int appointmentId);

    AppointmentPrerequisiteDto Create(CreateAppointmentPrerequisiteDTO request);

    AppointmentPrerequisiteDto Update(int id, UpdateAppointmentPrerequisiteDTO request);

    AppointmentPrerequisiteSummaryDTO GetAppointmentSummary(int appointmentId, string kind);
}
