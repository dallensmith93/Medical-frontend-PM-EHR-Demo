using MedicalDemo.Api.Models;

namespace MedicalDemo.Api.Repositories;

public class InMemoryAppointmentRepository : IAppointmentRepository
{
    private readonly List<Appointment> _appointments =
    [
        new()
        {
            Id = 1,
            PatientId = 1,
            ProviderId = 1,
            AppointmentDate = DateTime.UtcNow.Date.AddDays(1).AddHours(9),
            DurationMinutes = 30,
            Reason = "Annual wellness visit",
            EligibilityStatus = "verified",
            EligibilityReviewedAt = DateTime.UtcNow.AddHours(-4),
            EligibilityNotes = "Eligibility confirmed this morning."
        },
        new()
        {
            Id = 2,
            PatientId = 2,
            ProviderId = 2,
            AppointmentDate = DateTime.UtcNow.Date.AddDays(1).AddHours(11),
            DurationMinutes = 30,
            Reason = "Cardiology follow-up",
            EligibilityStatus = "pending",
            EligibilityNotes = "Needs review before check-in."
        },
        new()
        {
            Id = 3,
            PatientId = 3,
            ProviderId = 3,
            AppointmentDate = DateTime.UtcNow.Date.AddDays(2).AddHours(10),
            DurationMinutes = 30,
            Reason = "Pediatric sick visit",
            EligibilityStatus = "failed",
            EligibilityReviewedAt = DateTime.UtcNow.AddDays(-1),
            EligibilityNotes = "Coverage check failed. Front desk follow-up required."
        }
    ];
    private readonly object _syncRoot = new();
    private int _nextId = 4;

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

    public Appointment? GetById(int id)
    {
        lock (_syncRoot)
        {
            var appointment = _appointments.FirstOrDefault(item => item.Id == id);
            return appointment is null ? null : Clone(appointment);
        }
    }

    public Appointment Update(Appointment appointment)
    {
        lock (_syncRoot)
        {
            var index = _appointments.FindIndex(existingAppointment => existingAppointment.Id == appointment.Id);
            if (index < 0)
            {
                throw new KeyNotFoundException($"Appointment with id {appointment.Id} was not found.");
            }

            var storedAppointment = Clone(appointment);
            _appointments[index] = storedAppointment;
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
            Reason = appointment.Reason,
            EligibilityStatus = appointment.EligibilityStatus,
            EligibilityReviewedAt = appointment.EligibilityReviewedAt,
            EligibilityNotes = appointment.EligibilityNotes
        };
}
