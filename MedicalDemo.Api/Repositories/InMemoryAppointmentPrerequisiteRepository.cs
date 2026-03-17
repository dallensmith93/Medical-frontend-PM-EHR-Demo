using MedicalDemo.Api.Models;

namespace MedicalDemo.Api.Repositories;

public class InMemoryAppointmentPrerequisiteRepository : IAppointmentPrerequisiteRepository
{
    private readonly List<AppointmentPrerequisite> _prerequisites =
    [
        new()
        {
            Id = 1,
            AppointmentId = 1,
            PatientId = 1,
            Kind = "authorization",
            Status = "approved",
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
            ExpiresOn = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(10)),
            Notes = "Authorization approved for the annual wellness visit."
        },
        new()
        {
            Id = 2,
            AppointmentId = 2,
            PatientId = 2,
            Kind = "authorization",
            Status = "needed",
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)),
            Notes = "Submit prior auth packet before cardiology follow-up."
        },
        new()
        {
            Id = 3,
            AppointmentId = 2,
            PatientId = 2,
            Kind = "referral",
            Status = "submitted",
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
            Notes = "Referral request sent to PCP office."
        },
        new()
        {
            Id = 4,
            AppointmentId = 3,
            PatientId = 3,
            Kind = "referral",
            Status = "denied",
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)),
            Notes = "Referral denied until subscriber mismatch is resolved."
        }
    ];

    private readonly object _syncRoot = new();
    private int _nextId = 5;

    public IReadOnlyCollection<AppointmentPrerequisite> GetByAppointmentId(int appointmentId)
    {
        lock (_syncRoot)
        {
            return _prerequisites
                .Where(item => item.AppointmentId == appointmentId)
                .Select(Clone)
                .ToList()
                .AsReadOnly();
        }
    }

    public AppointmentPrerequisite? GetById(int id)
    {
        lock (_syncRoot)
        {
            var prerequisite = _prerequisites.FirstOrDefault(item => item.Id == id);
            return prerequisite is null ? null : Clone(prerequisite);
        }
    }

    public AppointmentPrerequisite UpsertActive(AppointmentPrerequisite prerequisite)
    {
        lock (_syncRoot)
        {
            var index = _prerequisites.FindIndex(item =>
                item.AppointmentId == prerequisite.AppointmentId &&
                item.Kind == prerequisite.Kind);

            if (index >= 0)
            {
                var storedExisting = Clone(prerequisite);
                storedExisting.Id = _prerequisites[index].Id;
                _prerequisites[index] = storedExisting;
                return Clone(storedExisting);
            }

            var storedPrerequisite = Clone(prerequisite);
            storedPrerequisite.Id = _nextId++;
            _prerequisites.Add(storedPrerequisite);
            return Clone(storedPrerequisite);
        }
    }

    public AppointmentPrerequisite Update(AppointmentPrerequisite prerequisite)
    {
        lock (_syncRoot)
        {
            var index = _prerequisites.FindIndex(item => item.Id == prerequisite.Id);
            if (index < 0)
            {
                throw new KeyNotFoundException($"Prerequisite with id {prerequisite.Id} was not found.");
            }

            var storedPrerequisite = Clone(prerequisite);
            _prerequisites[index] = storedPrerequisite;
            return Clone(storedPrerequisite);
        }
    }

    private static AppointmentPrerequisite Clone(AppointmentPrerequisite prerequisite) =>
        new()
        {
            Id = prerequisite.Id,
            AppointmentId = prerequisite.AppointmentId,
            PatientId = prerequisite.PatientId,
            Kind = prerequisite.Kind,
            Status = prerequisite.Status,
            DueDate = prerequisite.DueDate,
            ExpiresOn = prerequisite.ExpiresOn,
            Notes = prerequisite.Notes
        };
}
