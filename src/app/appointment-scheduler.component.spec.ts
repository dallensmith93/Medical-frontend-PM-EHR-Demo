import { TestBed } from "@angular/core/testing";
import { of } from "rxjs";
import { AppointmentSchedulerComponent } from "./appointment-scheduler.component";
import { AppointmentService } from "./services/appointment.service";
import { PatientService } from "./services/patient.service";
import { ProviderService } from "./services/provider.service";
import {
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
    },
  ];

  let appointmentService: jasmine.SpyObj<AppointmentService>;
  let patientService: jasmine.SpyObj<PatientService>;
  let providerService: jasmine.SpyObj<ProviderService>;

  beforeEach(async () => {
    appointmentService = jasmine.createSpyObj<AppointmentService>("AppointmentService", [
      "getAppointments",
      "createAppointment",
      "updateEligibility",
    ]);
    patientService = jasmine.createSpyObj<PatientService>("PatientService", ["getPatients"]);
    providerService = jasmine.createSpyObj<ProviderService>("ProviderService", ["getProviders"]);

    appointmentService.getAppointments.and.returnValue(of(appointments));
    appointmentService.createAppointment.and.returnValue(of(appointments[0]));
    appointmentService.updateEligibility.and.returnValue(of(appointments[0]));
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
    const completeAppointment = component.appointments.find((appointment) => appointment.id === 101);

    expect(completeAppointment).toBeDefined();
    expect(component.eligibilityBadgeClass(completeAppointment!.eligibilityStatus)).toBe("is-verified");
    expect(component.shouldWarnForEligibility(completeAppointment!)).toBeFalse();
  });

  it("renders an incomplete intake appointment with a visible pending warning", () => {
    const fixture = TestBed.createComponent(AppointmentSchedulerComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();
    const pendingAppointment = component.appointments.find((appointment) => appointment.id === 102);

    expect(pendingAppointment).toBeDefined();
    expect(component.eligibilityBadgeClass(pendingAppointment!.eligibilityStatus)).toBe("is-pending");
    expect(component.shouldWarnForEligibility(pendingAppointment!)).toBeTrue();
    expect(component.eligibilityWarningText(pendingAppointment!)).toBe(
      "Eligibility review is still pending before this appointment.",
    );
  });

  it("shows a failed-readiness warning in the week slot card", () => {
    const fixture = TestBed.createComponent(AppointmentSchedulerComponent);
    const component = fixture.componentInstance;

    component.currentView = "week";
    component.currentDate = new Date(2026, 2, 19);
    fixture.detectChanges();

    const warningElements = Array.from(
      fixture.nativeElement.querySelectorAll(".eligibility-warning"),
    ).map((element) => ((element as Element).textContent ?? "").trim());

    expect(warningElements).toContain(
      "Coverage issue found. Follow up with patient or payer before the visit.",
    );
  });
});
