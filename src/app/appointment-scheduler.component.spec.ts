import { TestBed } from "@angular/core/testing";
import { of } from "rxjs";
import { AppointmentSchedulerComponent } from "./appointment-scheduler.component";
import { AppointmentService } from "./services/appointment.service";
import { PatientService } from "./services/patient.service";
import { ProviderService } from "./services/provider.service";
import {
  AppointmentPrerequisiteDto,
  AppointmentResponseDto,
  PatientDto,
  ProviderDto,
} from "./types/scheduler-api.types";

describe("AppointmentSchedulerComponent", () => {
  const patients: PatientDto[] = [
    {
      id: 1,
      firstName: "Maya",
      lastName: "Robinson",
      dateOfBirth: "1989-04-12",
      phoneNumber: "555-0101",
      email: "maya@example.com",
      addressLine1: "10 Cherry Ln",
      city: "Denver",
      state: "CO",
      postalCode: "80203",
      payerName: "Aetna",
      memberId: "AET-884421",
      insuranceSummary: "Commercial PPO",
      eligibilityStatus: "verified",
      lastEligibilityVerifiedAt: "2026-03-16T13:00:00.000Z",
      eligibilityNotes: "Commercial plan active.",
      intakeStatus: "complete",
      intakeNotes: "Forms signed.",
      missingIntakeItems: [],
    },
    {
      id: 2,
      firstName: "Ethan",
      lastName: "Brooks",
      dateOfBirth: "1978-11-03",
      phoneNumber: "555-0102",
      email: "ethan@example.com",
      addressLine1: "25 Aspen Dr",
      city: "Lakewood",
      state: "CO",
      postalCode: "80226",
      payerName: "Blue Cross",
      memberId: "BCB-552190",
      insuranceSummary: "Needs updated card",
      eligibilityStatus: "pending",
      lastEligibilityVerifiedAt: null,
      eligibilityNotes: "Recheck needed before follow-up.",
      intakeStatus: "inProgress",
      intakeNotes: "Authorization not uploaded.",
      missingIntakeItems: ["Insurance card", "Signed consent"],
    },
    {
      id: 3,
      firstName: "Sophia",
      lastName: "Nguyen",
      dateOfBirth: "2016-07-21",
      phoneNumber: "555-0103",
      email: "guardian@example.com",
      addressLine1: "88 Pine Ave",
      city: "Aurora",
      state: "CO",
      postalCode: "80012",
      payerName: "Cigna",
      memberId: "CIG-104822",
      insuranceSummary: "Subscriber mismatch",
      eligibilityStatus: "failed",
      lastEligibilityVerifiedAt: "2026-03-15T13:00:00.000Z",
      eligibilityNotes: "Subscriber ID mismatch.",
      intakeStatus: "notStarted",
      intakeNotes: "No intake packet returned.",
      missingIntakeItems: ["Demographics", "Insurance card", "Medical history"],
    },
  ];

  const providers: ProviderDto[] = [
    { id: 1, name: "Dr. Sarah Chen", specialty: "Family Medicine" },
    { id: 2, name: "Dr. Michael Alvarez", specialty: "Cardiology" },
  ];

  const appointments: AppointmentResponseDto[] = [
    {
      id: 101,
      patientId: 1,
      providerId: 1,
      appointmentDate: "2026-03-18T09:00:00.000Z",
      durationMinutes: 30,
      reason: "Annual wellness visit",
      eligibilityStatus: "verified",
      eligibilityReviewedAt: "2026-03-16T13:00:00.000Z",
      eligibilityNotes: "Coverage verified and ready for the visit.",
      intakeStatus: "complete",
      isIntakeComplete: true,
      missingIntakeItems: [],
      authorization: {
        id: 10,
        kind: "authorization",
        isRequired: true,
        status: "approved",
        dueDate: "2026-03-17",
        expiresOn: "2026-03-30",
        notes: "Approved for the wellness visit.",
        isBlocking: false,
      },
      referral: {
        id: null,
        kind: "referral",
        isRequired: false,
        status: "notRequired",
        dueDate: null,
        expiresOn: null,
        notes: "",
        isBlocking: false,
      },
      hasPrerequisiteBlocker: false,
    },
    {
      id: 102,
      patientId: 2,
      providerId: 1,
      appointmentDate: "2026-03-18T09:30:00.000Z",
      durationMinutes: 30,
      reason: "Cardiology follow-up",
      eligibilityStatus: "pending",
      eligibilityReviewedAt: null,
      eligibilityNotes: "Eligibility review is still pending.",
      intakeStatus: "inProgress",
      isIntakeComplete: false,
      missingIntakeItems: ["Insurance card", "Signed consent"],
      authorization: {
        id: 20,
        kind: "authorization",
        isRequired: true,
        status: "needed",
        dueDate: "2026-03-17",
        expiresOn: null,
        notes: "Submit auth before the visit.",
        isBlocking: true,
      },
      referral: {
        id: 21,
        kind: "referral",
        isRequired: true,
        status: "submitted",
        dueDate: "2026-03-17",
        expiresOn: null,
        notes: "Referral request sent to PCP office.",
        isBlocking: true,
      },
      hasPrerequisiteBlocker: true,
    },
    {
      id: 103,
      patientId: 3,
      providerId: 2,
      appointmentDate: "2026-03-19T10:00:00.000Z",
      durationMinutes: 30,
      reason: "Pediatric sick visit",
      eligibilityStatus: "failed",
      eligibilityReviewedAt: "2026-03-15T13:00:00.000Z",
      eligibilityNotes: "Coverage issue found. Follow up with patient or payer.",
      intakeStatus: "notStarted",
      isIntakeComplete: false,
      missingIntakeItems: ["Demographics", "Insurance card", "Medical history"],
      authorization: {
        id: 30,
        kind: "authorization",
        isRequired: true,
        status: "expired",
        dueDate: "2026-03-18",
        expiresOn: "2026-03-18",
        notes: "Approval expired before the rescheduled visit.",
        isBlocking: true,
      },
      referral: {
        id: 31,
        kind: "referral",
        isRequired: true,
        status: "denied",
        dueDate: "2026-03-18",
        expiresOn: null,
        notes: "Referral denied until subscriber data is corrected.",
        isBlocking: true,
      },
      hasPrerequisiteBlocker: true,
    },
  ];

  const updatedPrerequisite: AppointmentPrerequisiteDto = {
    id: 20,
    appointmentId: 102,
    patientId: 2,
    kind: "authorization",
    status: "submitted",
    dueDate: "2026-03-17",
    expiresOn: null,
    notes: "Submitted to payer.",
  };

  let appointmentService: jasmine.SpyObj<AppointmentService>;
  let patientService: jasmine.SpyObj<PatientService>;
  let providerService: jasmine.SpyObj<ProviderService>;

  beforeEach(async () => {
    appointmentService = jasmine.createSpyObj<AppointmentService>("AppointmentService", [
      "getAppointments",
      "createAppointment",
      "updateEligibility",
      "getAppointmentPrerequisites",
      "createAppointmentPrerequisite",
      "updateAppointmentPrerequisite",
    ]);
    patientService = jasmine.createSpyObj<PatientService>("PatientService", ["getPatients"]);
    providerService = jasmine.createSpyObj<ProviderService>("ProviderService", ["getProviders"]);

    appointmentService.getAppointments.and.returnValue(of(appointments));
    appointmentService.createAppointment.and.returnValue(of(appointments[0]));
    appointmentService.updateEligibility.and.returnValue(of(appointments[0]));
    appointmentService.getAppointmentPrerequisites.and.returnValue(of([]));
    appointmentService.createAppointmentPrerequisite.and.returnValue(of(updatedPrerequisite));
    appointmentService.updateAppointmentPrerequisite.and.returnValue(of(updatedPrerequisite));
    patientService.getPatients.and.returnValue(of(patients));
    providerService.getProviders.and.returnValue(of(providers));

    await TestBed.configureTestingModule({
      imports: [AppointmentSchedulerComponent],
      providers: [
        { provide: AppointmentService, useValue: appointmentService },
        { provide: PatientService, useValue: patientService },
        { provide: ProviderService, useValue: providerService },
      ],
    }).compileComponents();
  });

  it("renders a complete intake appointment without a readiness warning", () => {
    const fixture = TestBed.createComponent(AppointmentSchedulerComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();
    component.appointments = appointments;
    const completeAppointment = component.appointments.find((appointment) => appointment.id === 101);

    expect(completeAppointment).toBeDefined();
    expect(component.intakeBadgeClass(completeAppointment!.intakeStatus)).toBe("is-complete");
    expect(component.shouldWarnForIntake(completeAppointment!)).toBeFalse();
    expect(component.intakeSummary(completeAppointment!)).toBe("Intake complete");
  });

  it("renders a prerequisite blocker warning in the week slot card", () => {
    const fixture = TestBed.createComponent(AppointmentSchedulerComponent);
    const component = fixture.componentInstance;

    component.currentView = "week";
    component.currentDate = new Date(2026, 2, 18);
    fixture.detectChanges();

    const warningElements = Array.from(
      fixture.nativeElement.querySelectorAll(".prerequisite-warning"),
    ).map((element) => ((element as Element).textContent ?? "").trim());

    expect(warningElements.some((text) => text.includes("Authorization needed"))).toBeTrue();
  });

  it("shows due-date and notes detail for prerequisite follow-up", () => {
    const fixture = TestBed.createComponent(AppointmentSchedulerComponent);
    fixture.detectChanges();

    const detailText = fixture.nativeElement.textContent ?? "";

    expect(detailText).toContain("Referral request sent to PCP office.");
    expect(detailText).toContain("Due");
  });

  it("updates an existing prerequisite through the appointment service", () => {
    const fixture = TestBed.createComponent(AppointmentSchedulerComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component.savePrerequisite(appointments[1], "authorization", "submitted");

    expect(appointmentService.updateAppointmentPrerequisite).toHaveBeenCalledWith(20, {
      status: "submitted",
      dueDate: "2026-03-17",
      expiresOn: null,
      notes: "Submit auth before the visit.",
    });
  });

  it("creates a prerequisite when the appointment has no existing record for that kind", () => {
    const fixture = TestBed.createComponent(AppointmentSchedulerComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component.savePrerequisite(appointments[0], "referral", "needed");

    expect(appointmentService.createAppointmentPrerequisite).toHaveBeenCalledWith(101, {
      patientId: 1,
      kind: "referral",
      status: "needed",
      dueDate: null,
      expiresOn: null,
      notes: "",
    });
  });
});
