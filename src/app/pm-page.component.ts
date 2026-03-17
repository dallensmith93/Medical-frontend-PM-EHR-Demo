import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import {
  masterFiles,
  Patient,
  patients,
  pmMetrics,
  reports,
  schedule,
  userSettings,
} from "./mock-data";
import { AppointmentService } from "./services/appointment.service";
import { BillingService } from "./services/billing.service";
import { AppointmentChargeDto, ChargeReviewItemDto } from "./types/scheduler-api.types";

@Component({
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: "./pm-page.component.html",
})
export class PmPageComponent {
  private readonly billingService = inject(BillingService);
  private readonly appointmentService = inject(AppointmentService);

  readonly metrics = pmMetrics;
  readonly schedule = schedule;
  readonly masterFiles = masterFiles;
  readonly userSettings = userSettings;
  readonly reports = reports;
  readonly patients = patients;

  activePatient: Patient = this.patients[0];
  claimQueue: ChargeReviewItemDto[] = [];
  activeClaim: ChargeReviewItemDto | null = null;
  activeCharge: AppointmentChargeDto | null = null;
  claimErrorMessage = "";
  claimSaving = false;

  constructor(public router: Router) {
    this.loadClaimQueue();
  }

  get currentSection(): string {
    if (this.router.url.includes("/claims")) {
      return "claims";
    }
    if (this.router.url.includes("/master-files")) {
      return "master-files";
    }
    if (this.router.url.includes("/user-settings")) {
      return "user-settings";
    }
    if (this.router.url.includes("/reports")) {
      return "reports";
    }
    return "schedule";
  }

  selectPatient(patient: Patient): void {
    this.activePatient = patient;
  }

  selectClaim(claim: ChargeReviewItemDto): void {
    this.activeClaim = claim;
    this.loadCharge(claim.appointmentId);
  }

  updateChargeField(
    field: "diagnosisCode" | "procedureCode" | "modifier" | "units" | "amount" | "notes",
    value: string,
  ): void {
    if (!this.activeCharge) {
      return;
    }

    if (field === "units" || field === "amount") {
      this.activeCharge[field] = Number(value) as never;
      return;
    }

    this.activeCharge[field] = value as never;
  }

  saveActiveCharge(): void {
    if (!this.activeClaim || !this.activeCharge) {
      return;
    }

    this.claimErrorMessage = "";
    this.claimSaving = true;

    const payload = {
      diagnosisCode: this.activeCharge.diagnosisCode.trim(),
      procedureCode: this.activeCharge.procedureCode.trim(),
      modifier: this.activeCharge.modifier.trim(),
      units: Number.isFinite(this.activeCharge.units) ? this.activeCharge.units : 0,
      amount: Number.isFinite(this.activeCharge.amount) ? this.activeCharge.amount : 0,
      notes: this.activeCharge.notes.trim(),
    };

    const request$ = this.activeCharge.id
      ? this.appointmentService.updateAppointmentCharge(this.activeClaim.appointmentId, payload)
      : this.appointmentService.createAppointmentCharge(this.activeClaim.appointmentId, payload);

    request$.subscribe({
      next: (charge) => {
        this.activeCharge = charge;
        this.loadClaimQueue(this.activeClaim?.appointmentId ?? charge.appointmentId);
        this.claimSaving = false;
      },
      error: () => {
        this.claimErrorMessage = "Unable to save charge details.";
        this.claimSaving = false;
      },
    });
  }

  billingStatusLabel(status: string): string {
    switch (status) {
      case "readyToSubmit":
        return "Ready to Submit";
      case "reviewNeeded":
        return "Review Needed";
      default:
        return "Draft";
    }
  }

  private loadClaimQueue(preferredAppointmentId?: number): void {
    this.billingService.getReviewQueue().subscribe({
      next: (claims) => {
        this.claimQueue = claims;
        const nextActive =
          claims.find((claim) => claim.appointmentId === preferredAppointmentId) ??
          claims[0] ??
          null;
        this.activeClaim = nextActive;

        if (nextActive) {
          this.loadCharge(nextActive.appointmentId);
        }
      },
      error: () => {
        this.claimErrorMessage = "Unable to load claim review queue.";
      },
    });
  }

  private loadCharge(appointmentId: number): void {
    this.appointmentService.getAppointmentCharge(appointmentId).subscribe({
      next: (charge) => {
        this.activeCharge = charge;
      },
      error: () => {
        this.claimErrorMessage = "Unable to load charge details.";
      },
    });
  }
}
