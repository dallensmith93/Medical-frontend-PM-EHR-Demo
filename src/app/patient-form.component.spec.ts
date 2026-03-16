import { TestBed } from "@angular/core/testing";
import { of } from "rxjs";
import { PatientFormComponent } from "./patient-form.component";
import { PatientService } from "./services/patient.service";
import { PatientDto } from "./types/scheduler-api.types";

describe("PatientFormComponent", () => {
  const existingPatient: PatientDto = {
    id: 7,
    firstName: "Avery",
    lastName: "Stone",
    dateOfBirth: "1985-03-01",
    payerName: "Aetna",
    memberId: "AET-44",
    eligibilityStatus: "verified",
    lastEligibilityVerifiedAt: "2026-03-10T15:00:00.000Z",
    eligibilityNotes: "Verified on portal.",
  };

  let patientService: jasmine.SpyObj<PatientService>;

  beforeEach(async () => {
    patientService = jasmine.createSpyObj<PatientService>("PatientService", [
      "getPatients",
      "createPatient",
      "updatePatient",
    ]);
    patientService.getPatients.and.returnValue(of([existingPatient]));
    patientService.createPatient.and.callFake((request) => {
      return of({
        ...request,
        id: 8,
      });
    });
    patientService.updatePatient.and.callFake((id, request) => {
      return of({
        ...request,
        id,
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
      firstName: "Jordan",
      lastName: "Lee",
      dateOfBirth: "1992-04-15",
      payerName: "  Blue Cross  ",
      memberId: "  BC-1002  ",
      eligibilityStatus: "pending",
      lastEligibilityVerifiedAt: "",
      eligibilityNotes: "  Needs portal review.  ",
    });

    component.savePatient();

    expect(patientService.createPatient).toHaveBeenCalledOnceWith({
      firstName: "Jordan",
      lastName: "Lee",
      dateOfBirth: "1992-04-15",
      payerName: "Blue Cross",
      memberId: "BC-1002",
      eligibilityStatus: "pending",
      lastEligibilityVerifiedAt: null,
      eligibilityNotes: "Needs portal review.",
    });
    expect(component.successMessage).toBe("Patient saved successfully.");
    expect(component.isEditing).toBeFalse();
    expect(component.patientForm.getRawValue().firstName).toBe("");
  });

  it("updates the selected patient in edit mode", () => {
    const fixture = TestBed.createComponent(PatientFormComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component.editPatient(existingPatient);
    component.patientForm.patchValue({
      payerName: "Cigna",
      eligibilityStatus: "failed",
      eligibilityNotes: "  ID mismatch  ",
    });

    component.savePatient();

    expect(patientService.updatePatient).toHaveBeenCalledOnceWith(existingPatient.id, {
      firstName: "Avery",
      lastName: "Stone",
      dateOfBirth: "1985-03-01",
      payerName: "Cigna",
      memberId: "AET-44",
      eligibilityStatus: "failed",
      lastEligibilityVerifiedAt: "2026-03-10T15:00",
      eligibilityNotes: "ID mismatch",
    });
    expect(component.successMessage).toBe("Patient updated successfully.");
    expect(component.selectedPatientId).toBeNull();
  });

  it("blocks save and marks controls touched when the form is invalid", () => {
    const fixture = TestBed.createComponent(PatientFormComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component.patientForm.patchValue({
      firstName: "",
      lastName: "",
      dateOfBirth: "",
    });

    component.savePatient();

    expect(patientService.createPatient).not.toHaveBeenCalled();
    expect(patientService.updatePatient).not.toHaveBeenCalled();
    expect(component.patientForm.controls.firstName.touched).toBeTrue();
    expect(component.patientForm.controls.lastName.touched).toBeTrue();
    expect(component.patientForm.controls.dateOfBirth.touched).toBeTrue();
  });
});
