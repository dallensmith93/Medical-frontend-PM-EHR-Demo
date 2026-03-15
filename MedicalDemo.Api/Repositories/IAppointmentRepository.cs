using MedicalDemo.Api.Models;

namespace MedicalDemo.Api.Repositories;

public interface IAppointmentRepository
{
    IReadOnlyCollection<Appointment> GetAll();

    Appointment Add(Appointment appointment);
}
