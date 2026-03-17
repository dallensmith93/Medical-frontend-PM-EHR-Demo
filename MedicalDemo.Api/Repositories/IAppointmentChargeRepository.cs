using MedicalDemo.Api.Models;

namespace MedicalDemo.Api.Repositories;

public interface IAppointmentChargeRepository
{
    IReadOnlyCollection<AppointmentCharge> GetAll();

    AppointmentCharge? GetByAppointmentId(int appointmentId);

    AppointmentCharge? GetById(int id);

    AppointmentCharge UpsertByAppointment(AppointmentCharge charge);
}
