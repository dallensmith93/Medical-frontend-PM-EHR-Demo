import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { finalize } from "rxjs";
import {
  CreatePatientDto,
  EligibilityStatus,
  IntakeStatus,
  PatientDto,
} from "./types/scheduler-api.types";
import { PatientService } from "./services/patient.service";

@Component({
  standalone: true,
  selector: "app-patient-form",
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: "./patient-form.component.html",
  styleUrl: "./entity-forms.css",
})
export class PatientFormComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly patientService = inject(PatientService);

  readonly patientForm = this.formBuilder.nonNullable.group({
    demographics: this.formBuilder.nonNullable.group({
      firstName: ["", [Validators.required, Validators.maxLength(100)]],
      lastName: ["", [Validators.required, Validators.maxLength(100)]],
      dateOfBirth: ["", Validators.required],
    }),
    contact: this.formBuilder.nonNullable.group({
      phoneNumber: ["", [Validators.maxLength(30)]],
      email: ["", [Validators.email, Validators.maxLength(150)]],
      addressLine1: ["", [Validators.maxLength(150)]],
      city: ["", [Validators.maxLength(100)]],
      state: ["", [Validators.maxLength(50)]],
      postalCode: ["", [Validators.maxLength(20)]],
    }),
    insurance: this.formBuilder.nonNullable.group({
      payerName: ["", [Validators.maxLength(100)]],
      memberId: ["", [Validators.maxLength(100)]],
      insuranceSummary: ["", [Validators.maxLength(250)]],
      eligibilityStatus: ["pending" as EligibilityStatus, Validators.required],
      lastEligibilityVerifiedAt: [""],
      eligibilityNotes: ["", [Validators.maxLength(500)]],
    }),
    intake: this.formBuilder.nonNullable.group({
      intakeStatus: ["notStarted" as IntakeStatus, Validators.required],
      intakeNotes: ["", [Validators.maxLength(1000)]],
    }),
  });
  readonly eligibilityStatuses: EligibilityStatus[] = ["verified", "pending", "failed"];
  readonly intakeStatuses: IntakeStatus[] = ["notStarted", "inProgress", "complete"];

  patients: PatientDto[] = [];
  successMessage = "";
  isSaving = false;
  selectedPatientId: number | null = null;

  constructor() {
    this.loadPatients();
  }

  get isEditing(): boolean {
    return this.selectedPatientId !== null;
  }

  savePatient(): void {
    if (this.patientForm.invalid) {
      this.patientForm.markAllAsTouched();
      return;
    }

    this.successMessage = "";
    this.isSaving = true;

    const patientRequest = this.buildRequest();
    const request$ = this.isEditing && this.selectedPatientId !== null
      ? this.patientService.updatePatient(this.selectedPatientId, patientRequest)
      : this.patientService.createPatient(patientRequest);

    request$
      .pipe(finalize(() => (this.isSaving = false)))
      .subscribe({
        next: (patient) => {
          this.patients = this.sortPatients([
            ...this.patients.filter((currentPatient) => currentPatient.id !== patient.id),
            patient,
          ]);
          this.successMessage = this.isEditing
            ? "Patient updated successfully."
            : "Patient saved successfully.";
          this.clearForm();
        },
      });
  }

  editPatient(patient: PatientDto): void {
    this.selectedPatientId = patient.id;
    this.successMessage = "";
    this.patientForm.reset({
      demographics: {
        firstName: patient.firstName,
        lastName: patient.lastName,
        dateOfBirth: patient.dateOfBirth,
      },
      contact: {
        phoneNumber: patient.phoneNumber,
        email: patient.email,
        addressLine1: patient.addressLine1,
        city: patient.city,
        state: patient.state,
        postalCode: patient.postalCode,
      },
      insurance: {
        payerName: patient.payerName,
        memberId: patient.memberId,
        insuranceSummary: patient.insuranceSummary,
        eligibilityStatus: patient.eligibilityStatus,
        lastEligibilityVerifiedAt: this.toDateTimeLocalValue(patient.lastEligibilityVerifiedAt),
        eligibilityNotes: patient.eligibilityNotes,
      },
      intake: {
        intakeStatus: patient.intakeStatus,
        intakeNotes: patient.intakeNotes,
      },
    });
  }

  clearForm(): void {
    this.selectedPatientId = null;
    this.patientForm.reset({
      demographics: {
        firstName: "",
        lastName: "",
        dateOfBirth: "",
      },
      contact: {
        phoneNumber: "",
        email: "",
        addressLine1: "",
        city: "",
        state: "",
        postalCode: "",
      },
      insurance: {
        payerName: "",
        memberId: "",
        insuranceSummary: "",
        eligibilityStatus: "pending",
        lastEligibilityVerifiedAt: "",
        eligibilityNotes: "",
      },
      intake: {
        intakeStatus: "notStarted",
        intakeNotes: "",
      },
    });
  }

  intakeStatusLabel(status: IntakeStatus): string {
    return status === "notStarted"
      ? "Not Started"
      : status === "inProgress"
        ? "In Progress"
        : "Complete";
  }

  private loadPatients(): void {
    this.patientService.getPatients().subscribe({
      next: (patients) => {
        this.patients = this.sortPatients(patients);
      },
    });
  }

  private buildRequest(): CreatePatientDto {
    const formValue = this.patientForm.getRawValue();

    return {
      ...formValue.demographics,
      ...formValue.contact,
      ...formValue.insurance,
      ...formValue.intake,
      phoneNumber: formValue.contact.phoneNumber.trim(),
      email: formValue.contact.email.trim(),
      addressLine1: formValue.contact.addressLine1.trim(),
      city: formValue.contact.city.trim(),
      state: formValue.contact.state.trim(),
      postalCode: formValue.contact.postalCode.trim(),
      payerName: formValue.insurance.payerName.trim(),
      memberId: formValue.insurance.memberId.trim(),
      insuranceSummary: formValue.insurance.insuranceSummary.trim(),
      eligibilityNotes: formValue.insurance.eligibilityNotes.trim(),
      lastEligibilityVerifiedAt: formValue.insurance.lastEligibilityVerifiedAt || null,
      intakeNotes: formValue.intake.intakeNotes.trim(),
    };
  }

  private sortPatients(patients: PatientDto[]): PatientDto[] {
    return [...patients].sort((left, right) => {
      return `${left.lastName}${left.firstName}`.localeCompare(`${right.lastName}${right.firstName}`);
    });
  }

  private toDateTimeLocalValue(value: string | null): string {
    if (!value) {
      return "";
    }

    return new Date(value).toISOString().slice(0, 16);
  }
}
