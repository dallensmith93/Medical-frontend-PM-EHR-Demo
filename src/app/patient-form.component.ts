import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { finalize } from "rxjs";
import { PatientDto } from "./types/scheduler-api.types";
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
    firstName: ["", [Validators.required, Validators.maxLength(100)]],
    lastName: ["", [Validators.required, Validators.maxLength(100)]],
    dateOfBirth: ["", Validators.required],
  });

  patients: PatientDto[] = [];
  successMessage = "";
  isSaving = false;

  constructor() {
    this.loadPatients();
  }

  createPatient(): void {
    if (this.patientForm.invalid) {
      this.patientForm.markAllAsTouched();
      return;
    }

    this.successMessage = "";
    this.isSaving = true;

    this.patientService
      .createPatient(this.patientForm.getRawValue())
      .pipe(finalize(() => (this.isSaving = false)))
      .subscribe({
        next: (patient) => {
          this.patients = [...this.patients, patient];
          this.successMessage = "Patient saved successfully.";
          this.patientForm.reset({
            firstName: "",
            lastName: "",
            dateOfBirth: "",
          });
        },
      });
  }

  private loadPatients(): void {
    this.patientService.getPatients().subscribe({
      next: (patients) => {
        this.patients = patients;
      },
    });
  }
}
