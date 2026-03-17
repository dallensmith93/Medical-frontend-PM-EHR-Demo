using MedicalDemo.Api.Models;

namespace MedicalDemo.Api.Repositories;

public class InMemoryAppointmentChargeRepository : IAppointmentChargeRepository
{
    private readonly List<AppointmentCharge> _charges =
    [
        new()
        {
            Id = 1,
            AppointmentId = 1,
            PatientId = 1,
            ProviderId = 1,
            DiagnosisCode = "Z00.00",
            ProcedureCode = "99395",
            Modifier = "25",
            Units = 1,
            Amount = 185m,
            Notes = "Annual preventive visit"
        },
        new()
        {
            Id = 2,
            AppointmentId = 2,
            PatientId = 2,
            ProviderId = 2,
            DiagnosisCode = "",
            ProcedureCode = "99214",
            Modifier = "",
            Units = 1,
            Amount = 225m,
            Notes = "Follow-up charge started but diagnosis not assigned"
        }
    ];

    private readonly object _syncRoot = new();
    private int _nextId = 3;

    public IReadOnlyCollection<AppointmentCharge> GetAll()
    {
        lock (_syncRoot)
        {
            return _charges
                .Select(Clone)
                .ToList()
                .AsReadOnly();
        }
    }

    public AppointmentCharge? GetByAppointmentId(int appointmentId)
    {
        lock (_syncRoot)
        {
            var charge = _charges.FirstOrDefault(item => item.AppointmentId == appointmentId);
            return charge is null ? null : Clone(charge);
        }
    }

    public AppointmentCharge? GetById(int id)
    {
        lock (_syncRoot)
        {
            var charge = _charges.FirstOrDefault(item => item.Id == id);
            return charge is null ? null : Clone(charge);
        }
    }

    public AppointmentCharge UpsertByAppointment(AppointmentCharge charge)
    {
        lock (_syncRoot)
        {
            var index = _charges.FindIndex(item => item.AppointmentId == charge.AppointmentId);

            if (index >= 0)
            {
                var storedExisting = Clone(charge);
                storedExisting.Id = _charges[index].Id;
                _charges[index] = storedExisting;
                return Clone(storedExisting);
            }

            var storedCharge = Clone(charge);
            storedCharge.Id = _nextId++;
            _charges.Add(storedCharge);
            return Clone(storedCharge);
        }
    }

    private static AppointmentCharge Clone(AppointmentCharge charge) =>
        new()
        {
            Id = charge.Id,
            AppointmentId = charge.AppointmentId,
            PatientId = charge.PatientId,
            ProviderId = charge.ProviderId,
            DiagnosisCode = charge.DiagnosisCode,
            ProcedureCode = charge.ProcedureCode,
            Modifier = charge.Modifier,
            Units = charge.Units,
            Amount = charge.Amount,
            Notes = charge.Notes
        };
}
