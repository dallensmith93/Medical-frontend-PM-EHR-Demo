import { TestBed } from "@angular/core/testing";
import { provideRouter, Router } from "@angular/router";
import { of } from "rxjs";
import { PmPageComponent } from "./pm-page.component";
import { AppointmentService } from "./services/appointment.service";
import { BillingService } from "./services/billing.service";
import { AppointmentChargeDto, ChargeReviewItemDto } from "./types/scheduler-api.types";

describe("PmPageComponent", () => {
  const reviewQueue: ChargeReviewItemDto[] = [
    {
      appointmentId: 102,
      chargeId: 302,
      patientId: 2,
      patientName: "Ethan Brooks",
      payerName: "Blue Cross",
      providerId: 1,
      providerName: "Dr. Sarah Chen",
      appointmentDate: "2026-03-18T09:30:00.000Z",
      reason: "Cardiology follow-up",
      status: "reviewNeeded",
      warningCount: 2,
      warnings: [
        { code: "diagnosis", message: "Diagnosis code is missing.", isBlocking: true },
        { code: "eligibility", message: "Eligibility is not verified for this visit.", isBlocking: true },
      ],
    },
    {
      appointmentId: 101,
      chargeId: 301,
      patientId: 1,
      patientName: "Maya Robinson",
      payerName: "Aetna",
      providerId: 1,
      providerName: "Dr. Sarah Chen",
      appointmentDate: "2026-03-18T09:00:00.000Z",
      reason: "Annual wellness visit",
      status: "readyToSubmit",
      warningCount: 0,
      warnings: [],
    },
  ];

  const activeCharge: AppointmentChargeDto = {
    id: 302,
    appointmentId: 102,
    patientId: 2,
    providerId: 1,
    diagnosisCode: "",
    procedureCode: "99214",
    modifier: "",
    units: 1,
    amount: 225,
    notes: "Initial billing review",
    billing: {
      chargeId: 302,
      status: "reviewNeeded",
      isReadyToSubmit: false,
      warningCount: 2,
      warnings: [
        { code: "diagnosis", message: "Diagnosis code is missing.", isBlocking: true },
        { code: "eligibility", message: "Eligibility is not verified for this visit.", isBlocking: true },
      ],
    },
  };

  let billingService: jasmine.SpyObj<BillingService>;
  let appointmentService: jasmine.SpyObj<AppointmentService>;

  beforeEach(async () => {
    billingService = jasmine.createSpyObj<BillingService>("BillingService", ["getReviewQueue"]);
    appointmentService = jasmine.createSpyObj<AppointmentService>("AppointmentService", [
      "getAppointmentCharge",
      "createAppointmentCharge",
      "updateAppointmentCharge",
    ]);

    billingService.getReviewQueue.and.returnValue(of(reviewQueue));
    appointmentService.getAppointmentCharge.and.returnValue(of(activeCharge));
    appointmentService.createAppointmentCharge.and.returnValue(of(activeCharge));
    appointmentService.updateAppointmentCharge.and.returnValue(of({
      ...activeCharge,
      diagnosisCode: "I10",
    }));

    await TestBed.configureTestingModule({
      imports: [PmPageComponent],
      providers: [
        provideRouter([]),
        { provide: BillingService, useValue: billingService },
        { provide: AppointmentService, useValue: appointmentService },
      ],
    }).compileComponents();
  });

  it("renders a warning-bearing claim queue item in the claims workspace", () => {
    const fixture = TestBed.createComponent(PmPageComponent);
    const component = fixture.componentInstance;
    spyOnProperty(component.router, "url", "get").and.returnValue("/pm/claims");
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent ?? "";

    expect(text).toContain("Ethan Brooks");
    expect(text).toContain("Review Needed");
    expect(text).toContain("Diagnosis code is missing.");
  });

  it("saves editable charge detail for the selected claim", () => {
    const fixture = TestBed.createComponent(PmPageComponent);
    const component = fixture.componentInstance;
    spyOnProperty(component.router, "url", "get").and.returnValue("/pm/claims");
    fixture.detectChanges();

    component.updateChargeField("diagnosisCode", "I10");
    component.saveActiveCharge();

    expect(appointmentService.updateAppointmentCharge).toHaveBeenCalledWith(102, {
      diagnosisCode: "I10",
      procedureCode: "99214",
      modifier: "",
      units: 1,
      amount: 225,
      notes: "Initial billing review",
    });
  });
});
