using MedicalDemo.Api.DTOs;
using MedicalDemo.Api.Models;
using MedicalDemo.Api.Repositories;
using MedicalDemo.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MedicalDemo.Api.Tests;

public class AppointmentChargeServiceTests
{
    private readonly InMemoryAppointmentChargeRepository _chargeRepository = new();
    private readonly InMemoryAppointmentRepository _appointmentRepository = new();
    private readonly InMemoryPatientRepository _patientRepository = new();
    private readonly InMemoryAppointmentPrerequisiteRepository _prerequisiteRepository = new();
    private readonly AppointmentPrerequisiteService _prerequisiteService;
    private readonly AppointmentChargeService _service;

    public AppointmentChargeServiceTests()
    {
        _prerequisiteService = new AppointmentPrerequisiteService(
            _prerequisiteRepository,
            _appointmentRepository,
            _patientRepository,
            NullLogger<AppointmentPrerequisiteService>.Instance);

        _service = new AppointmentChargeService(
            _chargeRepository,
            _appointmentRepository,
            _patientRepository,
            new InMemoryProviderRepository(),
            _prerequisiteService,
            NullLogger<AppointmentChargeService>.Instance);
    }

    [Fact]
    public void UpsertByAppointment_CreatesChargeForAppointmentWithoutExistingRecord()
    {
        var saved = _chargeRepository.UpsertByAppointment(new AppointmentCharge
        {
            AppointmentId = 4,
            PatientId = 1,
            ProviderId = 1,
            DiagnosisCode = "Z00.00",
            ProcedureCode = "99395",
            Modifier = "25",
            Units = 1,
            Amount = 150m,
            Notes = "Preventive visit"
        });

        Assert.True(saved.Id > 0);
        Assert.Equal("Z00.00", _chargeRepository.GetByAppointmentId(4)?.DiagnosisCode);
    }

    [Fact]
    public void UpsertByAppointment_ReusesExistingRecordForSameAppointment()
    {
        var first = _chargeRepository.UpsertByAppointment(new AppointmentCharge
        {
            AppointmentId = 4,
            PatientId = 2,
            ProviderId = 2,
            DiagnosisCode = "I10",
            ProcedureCode = "99214",
            Units = 1,
            Amount = 215m,
            Notes = "Initial save"
        });

        var updated = _chargeRepository.UpsertByAppointment(new AppointmentCharge
        {
            AppointmentId = 4,
            PatientId = 2,
            ProviderId = 2,
            DiagnosisCode = "I11.9",
            ProcedureCode = "99214",
            Modifier = "25",
            Units = 2,
            Amount = 430m,
            Notes = "Updated save"
        });

        Assert.Equal(first.Id, updated.Id);
        Assert.Equal("I11.9", updated.DiagnosisCode);
        Assert.Equal(2, _chargeRepository.GetByAppointmentId(4)?.Units);
    }

    [Fact]
    public void GetByAppointmentId_ReturnsReadyToSubmitForCleanCharge()
    {
        var charge = _service.GetByAppointmentId(1);

        Assert.Equal("readyToSubmit", charge.Billing.Status);
        Assert.Empty(charge.Billing.Warnings);
    }

    [Fact]
    public void GetByAppointmentId_ReturnsReviewNeededWarningsForIncompleteChargeAndReadinessIssues()
    {
        var charge = _service.GetByAppointmentId(2);

        Assert.Equal("reviewNeeded", charge.Billing.Status);
        Assert.Contains(charge.Billing.Warnings, item => item.Code == "diagnosis");
        Assert.Contains(charge.Billing.Warnings, item => item.Code == "eligibility");
        Assert.Contains(charge.Billing.Warnings, item => item.Code == "intake");
        Assert.Contains(charge.Billing.Warnings, item => item.Code == "authorization");
        Assert.Contains(charge.Billing.Warnings, item => item.Code == "referral");
    }

    [Fact]
    public void Update_CreatesReadyToSubmitChargeForAppointmentWithoutExistingRecord()
    {
        var saved = _service.Update(3, new UpdateAppointmentChargeDTO
        {
            DiagnosisCode = "J06.9",
            ProcedureCode = "99213",
            Modifier = "25",
            Units = 1,
            Amount = 135m,
            Notes = "Urgent same-day visit"
        });

        Assert.Equal(3, saved.AppointmentId);
        Assert.Equal("reviewNeeded", saved.Billing.Status);
        Assert.Contains(saved.Billing.Warnings, item => item.Code == "eligibility");
        Assert.Contains(saved.Billing.Warnings, item => item.Code == "insurance");
    }

    [Fact]
    public void GetReviewQueue_PrioritizesNonReadyItemsBeforeReadyToSubmit()
    {
        var queue = _service.GetReviewQueue().ToList();

        Assert.Equal("reviewNeeded", queue.First().Status);
        Assert.Equal("readyToSubmit", queue.Last().Status);
        Assert.Contains(queue.First().Warnings, item => item.Code == "diagnosis");
    }
}
