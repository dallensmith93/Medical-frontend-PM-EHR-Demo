using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Repositories;
using MedicalDemo.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MedicalDemo.Api.Tests;

public class AppointmentServiceTests
{
    private readonly InMemoryAppointmentRepository _appointmentRepository = new();
    private readonly InMemoryPatientRepository _patientRepository = new();
    private readonly InMemoryAppointmentPrerequisiteRepository _prerequisiteRepository = new();
    private readonly AppointmentPrerequisiteService _prerequisiteService;
    private readonly AppointmentService _service;

    public AppointmentServiceTests()
    {
        _prerequisiteService = new AppointmentPrerequisiteService(
            _prerequisiteRepository,
            _appointmentRepository,
            _patientRepository,
            NullLogger<AppointmentPrerequisiteService>.Instance);

        _service = new AppointmentService(
            _appointmentRepository,
            _patientRepository,
            new InMemoryProviderRepository(),
            _prerequisiteService,
            NullLogger<AppointmentService>.Instance);
    }

    [Fact]
    public void GetAll_ProjectsCompletePatientIntakeToAppointments()
    {
        var appointment = _service.GetAll().First(item => item.PatientId == 1);

        Assert.Equal("complete", appointment.IntakeStatus);
        Assert.True(appointment.IsIntakeComplete);
        Assert.Empty(appointment.MissingIntakeItems);
    }

    [Fact]
    public void GetAll_ProjectsMissingItemsForIncompletePatientIntake()
    {
        _service.Create(new CreateAppointmentDTO
        {
            PatientId = 2,
            ProviderId = 1,
            AppointmentDate = DateTime.UtcNow.Date.AddDays(3).AddHours(9),
            DurationMinutes = 30,
            Reason = "Intake review"
        });

        var appointment = _service.GetAll()
            .Where(item => item.PatientId == 2)
            .OrderByDescending(item => item.AppointmentDate)
            .First();

        Assert.Equal("inProgress", appointment.IntakeStatus);
        Assert.False(appointment.IsIntakeComplete);
        Assert.Contains("intake notes", appointment.MissingIntakeItems);
    }

    [Fact]
    public void GetAll_ReturnsNotRequiredWhenNoPrerequisiteRecordExistsForAKind()
    {
        var appointment = _service.GetAll().First(item => item.Id == 1);

        Assert.Equal("approved", appointment.Authorization.Status);
        Assert.Equal("notRequired", appointment.Referral.Status);
        Assert.False(appointment.Referral.IsBlocking);
    }

    [Fact]
    public void GetAll_ProjectsBlockingPrerequisiteSummariesAndDueDates()
    {
        var appointment = _service.GetAll().First(item => item.Id == 2);

        Assert.Equal("needed", appointment.Authorization.Status);
        Assert.True(appointment.Authorization.IsBlocking);
        Assert.Equal("submitted", appointment.Referral.Status);
        Assert.True(appointment.Referral.IsBlocking);
        Assert.True(appointment.HasPrerequisiteBlocker);
        Assert.NotNull(appointment.Authorization.DueDate);
        Assert.Contains("Referral request", appointment.Referral.Notes);
    }

    [Fact]
    public void GetAll_ProjectsExpiredSummaryWhenApprovalExpiresBeforeVisit()
    {
        _prerequisiteService.Create(new CreateAppointmentPrerequisiteDTO
        {
            AppointmentId = 3,
            PatientId = 3,
            Kind = "authorization",
            Status = "approved",
            ExpiresOn = DateOnly.FromDateTime(_appointmentRepository.GetById(3)!.AppointmentDate.AddDays(-1)),
            Notes = "Approval expired before the scheduled visit."
        });

        var appointment = _service.GetAll().First(item => item.Id == 3);

        Assert.Equal("expired", appointment.Authorization.Status);
        Assert.True(appointment.Authorization.IsBlocking);
        Assert.True(appointment.HasPrerequisiteBlocker);
    }
}
