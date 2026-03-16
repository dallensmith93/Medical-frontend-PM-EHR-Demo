import { TestBed } from "@angular/core/testing";
import { of } from "rxjs";
import { PatientFormComponent } from "./patient-form.component";
import { PatientService } from "./services/patient.service";
import { CreatePatientDto, PatientDto } from "./types/scheduler-api.types";

describe("PatientFormComponent", () => {
  const existingPatient: PatientDto = {
    id: 7,
    firstName: "Avery",
    lastName: "Stone",
    dateOfBirth: "1985-03-01",
    phoneNumber: "555-0100",
    email: "avery@example.com",
    addressLine1: "123 Main St",
    city: "Denver",
    state: "CO",
    postalCode: "80202",
    payerName: "Aetna",
    memberId: "AET-44",
    insuranceSummary: "Commercial PPO",
    eligibilityStatus: "verified",
    lastEligibilityVerifiedAt: "2026-03-10T15:00:00.000Z",
    eligibilityNotes: "Verified on portal.",
    intakeStatus: "complete",
    intakeNotes: "Forms received.",
    missingIntakeItems: [],
  };

  let patientService: jasmine.SpyObj<PatientService>;

  beforeEach(async () => {
    patientService = jasmine.createSpyObj<PatientService>("PatientService", [
      "getPatients",
      "createPatient",
      "updatePatient",
    ]);
    patientService.getPatients.and.returnValue(of([existingPatient]));
    patientService.createPatient.and.callFake((request: CreatePatientDto) => {
      return of({
        ...request,
        id: 8,
        missingIntakeItems: [],
      });
    });
    patientService.updatePatient.and.callFake((id: number, request: CreatePatientDto) => {
      return of({
        ...request,
        id,
        missingIntakeItems: [],
      });
    });

    await TestBed.configureTestingModule({
      imports: [PatientFormComponent],
      providers: [
        { provide: PatientService, useValue: patientService },
      ],
    }).compileComponents();
  });

  it("creates a patient in add mode and clears the form after save", () => {
    const fixture = TestBed.createComponent(PatientFormComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component.patientForm.setValue({
      demographics: {
        firstName: "Jordan",
        lastName: "Lee",
        dateOfBirth: "1992-04-15",
      },
      contact: {
        phoneNumber: " 555-0199 ",
        email: "jordan@example.com",
        addressLine1: " 45 Elm St ",
        city: " Denver ",
        state: " CO ",
        postalCode: " 80211 ",
      },
      insurance: {
        payerName: "  Blue Cross  ",
        memberId: "  BC-1002  ",
        insuranceSummary: "  PPO on file  ",
        eligibilityStatus: "pending",
        lastEligibilityVerifiedAt: "",
        eligibilityNotes: "  Needs portal review.  ",
      },
      intake: {
        intakeStatus: "inProgress",
        intakeNotes: "  Waiting on signed consents.  ",
      },
    });

    component.savePatient();

    expect(patientService.createPatient).toHaveBeenCalledOnceWith({
      firstName: "Jordan",
      lastName: "Lee",
      dateOfBirth: "1992-04-15",
      phoneNumber: "555-0199",
      email: "jordan@example.com",
      addressLine1: "45 Elm St",
      city: "Denver",
      state: "CO",
      postalCode: "80211",
      payerName: "Blue Cross",
      memberId: "BC-1002",
      insuranceSummary: "PPO on file",
      eligibilityStatus: "pending",
      lastEligibilityVerifiedAt: null,
      eligibilityNotes: "Needs portal review.",
      intakeStatus: "inProgress",
      intakeNotes: "Waiting on signed consents.",
    });
    expect(component.successMessage).toBe("Patient saved successfully.");
    expect(component.isEditing).toBeFalse();
    expect(component.patientForm.getRawValue().demographics.firstName).toBe("");
  });

  it("updates the selected patient in edit mode", () => {
    const fixture = TestBed.createComponent(PatientFormComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component.editPatient(existingPatient);
    component.patientForm.patchValue({
      insurance: {
        payerName: "Cigna",
        memberId: existingPatient.memberId,
        insuranceSummary: "Updated commercial summary",
        eligibilityStatus: "failed",
        lastEligibilityVerifiedAt: "2026-03-10T15:00",
        eligibilityNotes: "  ID mismatch  ",
      },
      intake: {
        intakeStatus: "inProgress",
        intakeNotes: "  Missing authorization form  ",
      },
    });

    component.savePatient();

    expect(patientService.updatePatient).toHaveBeenCalledOnceWith(existingPatient.id, {
      firstName: "Avery",
      lastName: "Stone",
      dateOfBirth: "1985-03-01",
      phoneNumber: "555-0100",
      email: "avery@example.com",
      addressLine1: "123 Main St",
      city: "Denver",
      state: "CO",
      postalCode: "80202",
      payerName: "Cigna",
      memberId: "AET-44",
      insuranceSummary: "Updated commercial summary",
      eligibilityStatus: "failed",
      lastEligibilityVerifiedAt: "2026-03-10T15:00",
      eligibilityNotes: "ID mismatch",
      intakeStatus: "inProgress",
      intakeNotes: "Missing authorization form",
    });
    expect(component.successMessage).toBe("Patient updated successfully.");
    expect(component.selectedPatientId).toBeNull();
  });

  it("blocks save and marks controls touched when the form is invalid", () => {
    const fixture = TestBed.createComponent(PatientFormComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component.patientForm.patchValue({
      demographics: {
        firstName: "",
        lastName: "",
        dateOfBirth: "",
      },
    });

    component.savePatient();

    expect(patientService.createPatient).not.toHaveBeenCalled();
    expect(patientService.updatePatient).not.toHaveBeenCalled();
    expect(component.patientForm.controls.demographics.controls.firstName.touched).toBeTrue();
    expect(component.patientForm.controls.demographics.controls.lastName.touched).toBeTrue();
    expect(component.patientForm.controls.demographics.controls.dateOfBirth.touched).toBeTrue();
  });
});
