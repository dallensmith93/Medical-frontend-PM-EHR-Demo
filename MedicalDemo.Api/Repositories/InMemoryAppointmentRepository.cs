using MedicalDemo.Api.Models;

namespace MedicalDemo.Api.Repositories;

public class InMemoryAppointmentRepository : IAppointmentRepository
{
    private readonly List<Appointment> _appointments = [];
    private readonly object _syncRoot = new();
    private int _nextId = 1;

    public IReadOnlyCollection<Appointment> GetAll()
    {
        lock (_syncRoot)
        {
            return _appointments
                .Select(Clone)
                .ToList()
                .AsReadOnly();
        }
    }

    public Appointment Add(Appointment appointment)
    {
        lock (_syncRoot)
        {
            var storedAppointment = Clone(appointment);
            storedAppointment.Id = _nextId++;
            _appointments.Add(storedAppointment);

            return Clone(storedAppointment);
        }
    }

    private static Appointment Clone(Appointment appointment) =>
        new()
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            ProviderId = appointment.ProviderId,
            AppointmentDate = appointment.AppointmentDate,
            DurationMinutes = appointment.DurationMinutes,
            Reason = appointment.Reason
        };
}
