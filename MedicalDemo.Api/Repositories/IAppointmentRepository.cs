using MedicalDemo.Api.Models;

namespace MedicalDemo.Api.Repositories;

public interface IAppointmentRepository
{
    IReadOnlyCollection<Appointment> GetAll();

    Appointment? GetById(int id);

    Appointment Add(Appointment appointment);

    Appointment Update(Appointment appointment);
}
