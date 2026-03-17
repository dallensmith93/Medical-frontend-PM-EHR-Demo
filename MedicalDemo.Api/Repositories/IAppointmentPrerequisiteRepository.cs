using MedicalDemo.Api.Models;

namespace MedicalDemo.Api.Repositories;

public interface IAppointmentPrerequisiteRepository
{
    IReadOnlyCollection<AppointmentPrerequisite> GetByAppointmentId(int appointmentId);

    AppointmentPrerequisite? GetById(int id);

    AppointmentPrerequisite UpsertActive(AppointmentPrerequisite prerequisite);

    AppointmentPrerequisite Update(AppointmentPrerequisite prerequisite);
}
