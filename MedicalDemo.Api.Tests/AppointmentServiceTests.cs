using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Repositories;
using MedicalDemo.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MedicalDemo.Api.Tests;

public class AppointmentServiceTests
{
    [Fact]
    public void GetAll_ReturnsAppointmentsSortedWithExistingReadinessState()
    {
        var service = CreateService();

        var appointments = service.GetAll().ToList();

        Assert.True(appointments.Count >= 3);
        Assert.Equal(appointments.OrderBy(item => item.AppointmentDate).Select(item => item.Id), appointments.Select(item => item.Id));
        Assert.Contains(appointments, appointment => appointment.EligibilityStatus == "verified");
        Assert.Contains(appointments, appointment => appointment.EligibilityStatus == "pending");
        Assert.Contains(appointments, appointment => appointment.EligibilityStatus == "failed");
    }

    [Fact]
    public void Create_SetsPendingReadinessDefaultsForNewAppointments()
    {
        var service = CreateService();

        var created = service.Create(new CreateAppointmentDTO
        {
            PatientId = 1,
            ProviderId = 1,
            AppointmentDate = DateTime.UtcNow.Date.AddDays(5).AddHours(14),
            DurationMinutes = 30,
            Reason = "  Follow-up visit  "
        });

        Assert.Equal("pending", created.EligibilityStatus);
        Assert.Null(created.EligibilityReviewedAt);
        Assert.Equal("New appointment requires eligibility review.", created.EligibilityNotes);
        Assert.Equal("Follow-up visit", created.Reason);
    }

    [Fact]
    public void UpdateEligibility_NormalizesStatusAndProjectsReviewToAppointmentAndPatient()
    {
        var patientRepository = new InMemoryPatientRepository();
        var appointmentRepository = new InMemoryAppointmentRepository();
        var service = CreateService(patientRepository, appointmentRepository);
        var reviewedAt = new DateTime(2026, 3, 16, 18, 45, 0, DateTimeKind.Utc);

        var updated = service.UpdateEligibility(2, new UpdateAppointmentEligibilityDTO
        {
            EligibilityStatus = " FAILED ",
            EligibilityReviewedAt = reviewedAt,
            EligibilityNotes = "  Subscriber ID mismatch.  "
        });

        Assert.Equal("failed", updated.EligibilityStatus);
        Assert.Equal(reviewedAt, updated.EligibilityReviewedAt);
        Assert.Equal("Subscriber ID mismatch.", updated.EligibilityNotes);

        var patient = patientRepository.GetById(2);
        Assert.NotNull(patient);
        Assert.Equal("failed", patient!.EligibilityStatus);
        Assert.Equal(reviewedAt, patient.LastEligibilityVerifiedAt);
        Assert.Equal("Subscriber ID mismatch.", patient.EligibilityNotes);
    }

    private static AppointmentService CreateService(
        IPatientRepository? patientRepository = null,
        IAppointmentRepository? appointmentRepository = null)
    {
        return new AppointmentService(
            appointmentRepository ?? new InMemoryAppointmentRepository(),
            patientRepository ?? new InMemoryPatientRepository(),
            new InMemoryProviderRepository(),
            NullLogger<AppointmentService>.Instance);
    }
}
