using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Repositories;
using MedicalDemo.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MedicalDemo.Api.Tests;

public class AppointmentPrerequisiteServiceTests
{
    private readonly InMemoryAppointmentRepository _appointmentRepository = new();
    private readonly InMemoryPatientRepository _patientRepository = new();
    private readonly InMemoryAppointmentPrerequisiteRepository _prerequisiteRepository = new();
    private readonly AppointmentPrerequisiteService _service;

    public AppointmentPrerequisiteServiceTests()
    {
        _service = new AppointmentPrerequisiteService(
            _prerequisiteRepository,
            _appointmentRepository,
            _patientRepository,
            NullLogger<AppointmentPrerequisiteService>.Instance);
    }

    [Fact]
    public void Create_CreatesAuthorizationAndReferralRecordsForTheAppointmentPatient()
    {
        var authorization = _service.Create(new CreateAppointmentPrerequisiteDTO
        {
            AppointmentId = 1,
            PatientId = 1,
            Kind = "authorization",
            Status = "needed",
            DueDate = new DateOnly(2026, 3, 18),
            ExpiresOn = new DateOnly(2026, 3, 20),
            Notes = "Obtain payer approval before the visit."
        });

        var referral = _service.Create(new CreateAppointmentPrerequisiteDTO
        {
            AppointmentId = 1,
            PatientId = 1,
            Kind = "referral",
            Status = "submitted",
            DueDate = new DateOnly(2026, 3, 17),
            Notes = "Referral packet sent to the PCP office."
        });

        Assert.Equal("authorization", authorization.Kind);
        Assert.Equal("needed", authorization.Status);
        Assert.Equal(new DateOnly(2026, 3, 18), authorization.DueDate);
        Assert.Equal(new DateOnly(2026, 3, 20), authorization.ExpiresOn);
        Assert.Equal("Obtain payer approval before the visit.", authorization.Notes);

        Assert.Equal("referral", referral.Kind);
        Assert.Equal("submitted", referral.Status);
        Assert.Equal(new DateOnly(2026, 3, 17), referral.DueDate);
        Assert.Null(referral.ExpiresOn);
        Assert.Equal("Referral packet sent to the PCP office.", referral.Notes);
    }

    [Fact]
    public void Create_RejectsMismatchedAppointmentAndPatientLinkage()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => _service.Create(new CreateAppointmentPrerequisiteDTO
        {
            AppointmentId = 1,
            PatientId = 2,
            Kind = "authorization",
            Status = "needed",
            Notes = "Wrong patient for the visit."
        }));

        Assert.Contains("patient", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("appointment", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(" NEEDED ", "needed")]
    [InlineData("Submitted", "submitted")]
    [InlineData("APPROVED", "approved")]
    [InlineData(" denied ", "denied")]
    public void Create_NormalizesStatusesToTheAllowedVocabulary(string rawStatus, string expectedStatus)
    {
        var created = _service.Create(new CreateAppointmentPrerequisiteDTO
        {
            AppointmentId = 2,
            PatientId = 2,
            Kind = "authorization",
            Status = rawStatus,
            DueDate = new DateOnly(2026, 3, 19),
            ExpiresOn = new DateOnly(2026, 3, 22),
            Notes = "Status normalization contract."
        });

        Assert.Equal(expectedStatus, created.Status);
    }

    [Fact]
    public void Update_PreservesDueDateExpiresOnAndNotesWhenStatusChanges()
    {
        var created = _service.Create(new CreateAppointmentPrerequisiteDTO
        {
            AppointmentId = 2,
            PatientId = 2,
            Kind = "referral",
            Status = "needed",
            DueDate = new DateOnly(2026, 3, 19),
            ExpiresOn = new DateOnly(2026, 3, 25),
            Notes = "Needs specialist referral before cardiology follow-up."
        });

        var updated = _service.Update(created.Id, new UpdateAppointmentPrerequisiteDTO
        {
            Status = "approved",
            DueDate = created.DueDate,
            ExpiresOn = created.ExpiresOn,
            Notes = created.Notes
        });

        Assert.Equal("approved", updated.Status);
        Assert.Equal(new DateOnly(2026, 3, 19), updated.DueDate);
        Assert.Equal(new DateOnly(2026, 3, 25), updated.ExpiresOn);
        Assert.Equal("Needs specialist referral before cardiology follow-up.", updated.Notes);
    }

    [Fact]
    public void GetAppointmentSummary_TreatsApprovedRecordsThatExpireBeforeTheVisitAsExpiredAndBlocking()
    {
        _service.Create(new CreateAppointmentPrerequisiteDTO
        {
            AppointmentId = 1,
            PatientId = 1,
            Kind = "authorization",
            Status = "approved",
            DueDate = new DateOnly(2026, 3, 18),
            ExpiresOn = DateOnly.FromDateTime(_appointmentRepository.GetById(1)!.AppointmentDate.AddDays(-1)),
            Notes = "Authorization lapsed before the rescheduled visit."
        });

        var summary = _service.GetAppointmentSummary(1, "authorization");

        Assert.Equal("expired", summary.Status);
        Assert.True(summary.IsBlocking);
        Assert.Equal("Authorization lapsed before the rescheduled visit.", summary.Notes);
    }
}
