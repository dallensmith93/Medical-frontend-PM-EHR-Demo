import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { forkJoin } from "rxjs";
import { AppointmentService } from "./services/appointment.service";
import { PatientService } from "./services/patient.service";
import { ProviderService } from "./services/provider.service";
import {
  AppointmentChargeDto,
  AppointmentPrerequisiteSummaryDto,
  AppointmentResponseDto,
  BillingStatus,
  CreateAppointmentChargeDto,
  CreateAppointmentPrerequisiteDto,
  EligibilityStatus,
  IntakeStatus,
  PatientDto,
  PrerequisiteKind,
  PrerequisiteStatus,
  ProviderDto,
} from "./types/scheduler-api.types";

type CalendarView = "month" | "week" | "day";
type SlotState = "open" | "occupied" | "selected";
type DraftField = "dueDate" | "expiresOn" | "notes";

interface CalendarDay {
  date: Date;
  inCurrentMonth: boolean;
  appointments: AppointmentResponseDto[];
}

interface TimeSlot {
  label: string;
  hour: number;
  minute: number;
}

interface PrerequisiteDraft {
  dueDate: string;
  expiresOn: string;
  notes: string;
}

interface ChargeDraft {
  diagnosisCode: string;
  procedureCode: string;
  modifier: string;
  units: number;
  amount: number;
  notes: string;
}

@Component({
  standalone: true,
  selector: "app-appointment-scheduler",
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: "./appointment-scheduler.component.html",
  styleUrl: "./entity-forms.css",
})
export class AppointmentSchedulerComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly patientService = inject(PatientService);
  private readonly providerService = inject(ProviderService);
  private readonly appointmentService = inject(AppointmentService);

  readonly appointmentForm = this.formBuilder.nonNullable.group({
    patientId: [0, [Validators.required, Validators.min(1)]],
    providerId: [0, [Validators.required, Validators.min(1)]],
  });

  readonly weekdayLabels = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
  readonly prerequisiteKinds: PrerequisiteKind[] = ["authorization", "referral"];
  readonly timeSlots: TimeSlot[] = Array.from({ length: 20 }, (_, index) => {
    const totalMinutes = 8 * 60 + index * 30;
    const hour = Math.floor(totalMinutes / 60);
    const minute = totalMinutes % 60;
    const slotDate = new Date(2024, 0, 1, hour, minute);
    return {
      label: slotDate.toLocaleTimeString([], {
        hour: "numeric",
        minute: "2-digit",
      }),
      hour,
      minute,
    };
  });

  patients: PatientDto[] = [];
  providers: ProviderDto[] = [];
  appointments: AppointmentResponseDto[] = [];
  currentView: CalendarView = "week";
  currentDate = this.startOfDay(new Date());
  errorMessage = "";
  isSaving = false;
  eligibilityUpdatingId: number | null = null;
  selectedSlotKey = "";
  prerequisiteSavingKey: string | null = null;
  chargeSavingId: number | null = null;

  private readonly prerequisiteDrafts: Record<string, PrerequisiteDraft> = {};
  private readonly chargeDrafts: Record<number, ChargeDraft> = {};

  constructor() {
    this.loadData();
  }

  setView(view: CalendarView): void {
    this.currentView = view;
  }

  navigateCalendar(direction: -1 | 1): void {
    if (this.currentView === "month") {
      this.currentDate = new Date(
        this.currentDate.getFullYear(),
        this.currentDate.getMonth() + direction,
        1,
      );
      return;
    }

    if (this.currentView === "week") {
      this.currentDate = this.addDays(this.currentDate, direction * 7);
      return;
    }

    this.currentDate = this.addDays(this.currentDate, direction);
  }

  jumpToToday(): void {
    this.currentDate = this.startOfDay(new Date());
  }

  scheduleSlot(date: Date, slot: TimeSlot): void {
    if (this.appointmentForm.invalid) {
      this.errorMessage = "Select a patient and provider before choosing a time slot.";
      this.appointmentForm.markAllAsTouched();
      return;
    }

    this.errorMessage = "";
    this.isSaving = true;
    this.selectedSlotKey = this.slotKey(date, slot);

    const formValue = this.appointmentForm.getRawValue();
    const appointmentDate = new Date(
      date.getFullYear(),
      date.getMonth(),
      date.getDate(),
      slot.hour,
      slot.minute,
    ).toISOString();

    this.appointmentService.createAppointment({
      patientId: formValue.patientId,
      providerId: formValue.providerId,
      appointmentDate,
      durationMinutes: 30,
      reason: "Scheduled appointment",
    }).subscribe({
      next: (appointment) => {
        this.currentDate = this.startOfDay(this.parseAppointmentDate(appointment));
        this.loadAppointments(() => {
          this.selectedSlotKey = "";
          this.isSaving = false;
        });
      },
      error: (error) => {
        this.errorMessage =
          error?.error?.message
            ? `${error.error.message} Try a different slot for ${this.selectedProviderName}.`
            : "Unable to schedule the appointment.";
        this.selectedSlotKey = "";
        this.isSaving = false;
      },
    });
  }

  get currentRangeLabel(): string {
    if (this.currentView === "month") {
      return this.currentDate.toLocaleDateString(undefined, {
        month: "long",
        year: "numeric",
      });
    }

    if (this.currentView === "week") {
      const start = this.startOfWeek(this.currentDate);
      const end = this.addDays(start, 6);
      return `${this.formatDateLabel(start, "short")} - ${this.formatDateLabel(end, "short")}`;
    }

    return this.formatDateLabel(this.currentDate, "long");
  }

  get selectedPatientName(): string {
    const patientId = this.appointmentForm.controls.patientId.value;
    return patientId > 0 ? this.patientName(patientId) : "No patient selected";
  }

  get selectedProviderName(): string {
    const providerId = this.appointmentForm.controls.providerId.value;
    return providerId > 0 ? this.providerName(providerId) : "No provider selected";
  }

  get selectedProviderAppointmentCount(): number {
    return this.filteredAppointments.length;
  }

  get selectedPatientEligibilityLabel(): string {
    const patientId = this.appointmentForm.controls.patientId.value;
    if (patientId <= 0) {
      return "Select a patient to review eligibility";
    }

    const patient = this.patients.find((item) => item.id === patientId);
    if (!patient) {
      return "Patient record unavailable";
    }

    const payerLabel = patient.payerName || "No payer on file";
    const statusLabel = this.formatEligibilityLabel(patient.eligibilityStatus);
    return `${statusLabel} • ${payerLabel}`;
  }

  get openSlotCountThisWeek(): number {
    return this.weekDays.reduce((total, day) => {
      return (
        total +
        this.timeSlots.filter((slot) => this.slotState(day.date, slot) === "open").length
      );
    }, 0);
  }

  get nextAppointmentLabel(): string {
    const next = this.filteredAppointments.find((appointment) => {
      return this.parseAppointmentDate(appointment).getTime() >= Date.now();
    });

    if (!next) {
      return "No upcoming visits";
    }

    return `${this.patientName(next.patientId)} • ${this.formatTime(next.appointmentDate)}`;
  }

  get appointmentsRequiringReviewCount(): number {
    return this.upcomingAppointments.filter((appointment) => {
      return appointment.eligibilityStatus !== "verified";
    }).length;
  }

  get appointmentsMissingIntakeCount(): number {
    return this.upcomingAppointments.filter((appointment) => !appointment.isIntakeComplete).length;
  }

  get appointmentsWithPrerequisiteBlockersCount(): number {
    return this.upcomingAppointments.filter((appointment) => appointment.hasPrerequisiteBlocker).length;
  }

  get appointmentsWithBillingReviewCount(): number {
    return this.upcomingAppointments.filter((appointment) => appointment.billing.status !== "readyToSubmit").length;
  }

  get upcomingAppointments(): AppointmentResponseDto[] {
    const now = Date.now();
    return this.filteredAppointments
      .filter((appointment) => this.parseAppointmentDate(appointment).getTime() >= now)
      .slice(0, 6);
  }

  get weekDays(): CalendarDay[] {
    const start = this.startOfWeek(this.currentDate);
    return Array.from({ length: 7 }, (_, index) => {
      const date = this.addDays(start, index);
      return {
        date,
        inCurrentMonth: true,
        appointments: this.appointmentsForDay(date),
      };
    });
  }

  get monthDays(): CalendarDay[] {
    const firstOfMonth = new Date(
      this.currentDate.getFullYear(),
      this.currentDate.getMonth(),
      1,
    );
    const gridStart = this.startOfWeek(firstOfMonth);

    return Array.from({ length: 42 }, (_, index) => {
      const date = this.addDays(gridStart, index);
      return {
        date,
        inCurrentMonth: date.getMonth() === this.currentDate.getMonth(),
        appointments: this.appointmentsForDay(date),
      };
    });
  }

  isToday(date: Date): boolean {
    const today = this.startOfDay(new Date());
    return date.getTime() === today.getTime();
  }

  isCurrentDate(date: Date): boolean {
    return date.getTime() === this.startOfDay(this.currentDate).getTime();
  }

  selectDate(date: Date): void {
    this.currentDate = this.startOfDay(date);
    if (this.currentView === "month") {
      this.currentView = "day";
    }
  }

  slotAppointments(date: Date, slot: TimeSlot): AppointmentResponseDto[] {
    return this.filteredAppointments.filter((appointment) => {
      const appointmentDate = this.parseAppointmentDate(appointment);
      return (
        this.startOfDay(appointmentDate).getTime() === this.startOfDay(date).getTime() &&
        appointmentDate.getHours() === slot.hour &&
        appointmentDate.getMinutes() === slot.minute
      );
    });
  }

  slotSelected(date: Date, slot: TimeSlot): boolean {
    return this.selectedSlotKey === this.slotKey(date, slot);
  }

  slotState(date: Date, slot: TimeSlot): SlotState {
    if (this.slotSelected(date, slot)) {
      return "selected";
    }

    return this.slotAppointments(date, slot).length > 0 ? "occupied" : "open";
  }

  slotHelperText(date: Date, slot: TimeSlot): string {
    const appointments = this.slotAppointments(date, slot);
    if (appointments.length === 0) {
      return "Open slot";
    }

    if (appointments.length === 1) {
      return this.patientName(appointments[0].patientId);
    }

    return `${appointments.length} appointments`;
  }

  formatTime(value: string): string {
    return new Date(value).toLocaleTimeString([], {
      hour: "numeric",
      minute: "2-digit",
    });
  }

  formatEligibilityLabel(status: EligibilityStatus): string {
    return `${status.charAt(0).toUpperCase()}${status.slice(1)}`;
  }

  formatIntakeLabel(status: IntakeStatus): string {
    return status === "notStarted"
      ? "Not Started"
      : status === "inProgress"
        ? "In Progress"
        : "Complete";
  }

  formatPrerequisiteLabel(status: PrerequisiteStatus): string {
    switch (status) {
      case "notRequired":
        return "Not Required";
      case "needed":
        return "Needed";
      case "submitted":
        return "Submitted";
      case "approved":
        return "Approved";
      case "denied":
        return "Denied";
      default:
        return "Expired";
    }
  }

  eligibilityBadgeClass(status: EligibilityStatus): string {
    return `is-${status}`;
  }

  intakeBadgeClass(status: IntakeStatus): string {
    return `is-${status}`;
  }

  prerequisiteBadgeClass(status: PrerequisiteStatus): string {
    return `is-${status}`;
  }

  billingBadgeClass(status: BillingStatus): string {
    return `is-${status}`;
  }

  shouldWarnForEligibility(appointment: AppointmentResponseDto): boolean {
    return appointment.eligibilityStatus !== "verified";
  }

  eligibilityWarningText(appointment: AppointmentResponseDto): string {
    if (appointment.eligibilityStatus === "failed") {
      return "Coverage issue found. Follow up with patient or payer before the visit.";
    }

    return "Eligibility review is still pending before this appointment.";
  }

  shouldWarnForIntake(appointment: AppointmentResponseDto): boolean {
    return !appointment.isIntakeComplete;
  }

  intakeSummary(appointment: AppointmentResponseDto): string {
    return appointment.missingIntakeItems.length > 0
      ? `Missing: ${appointment.missingIntakeItems.join(", ")}`
      : "Intake complete";
  }

  getSummary(
    appointment: AppointmentResponseDto,
    kind: PrerequisiteKind,
  ): AppointmentPrerequisiteSummaryDto {
    return kind === "authorization" ? appointment.authorization : appointment.referral;
  }

  prerequisiteWarningText(appointment: AppointmentResponseDto): string {
    const blockingKinds = (["authorization", "referral"] as const)
      .filter((kind) => this.getSummary(appointment, kind).isBlocking)
      .map((kind) => {
        const label = kind === "authorization" ? "Authorization" : "Referral";
        return `${label} ${this.formatPrerequisiteLabel(this.getSummary(appointment, kind).status).toLowerCase()}`;
      });

    return blockingKinds.length > 0
      ? `${blockingKinds.join(" and ")} before this visit.`
      : "No prerequisite blockers.";
  }

  prerequisiteDetailText(summary: AppointmentPrerequisiteSummaryDto): string {
    const parts: string[] = [];

    if (summary.dueDate) {
      parts.push(`Due ${this.formatOptionalDate(summary.dueDate)}`);
    }

    if (summary.expiresOn) {
      parts.push(`Expires ${this.formatOptionalDate(summary.expiresOn)}`);
    }

    if (summary.notes) {
      parts.push(summary.notes);
    }

    return parts.length > 0 ? parts.join(" • ") : "No follow-up details yet.";
  }

  billingStatusLabel(status: BillingStatus): string {
    switch (status) {
      case "readyToSubmit":
        return "Ready to Submit";
      case "reviewNeeded":
        return "Review Needed";
      default:
        return "Draft";
    }
  }

  billingWarningSummary(appointment: AppointmentResponseDto): string {
    if (appointment.billing.warningCount === 0) {
      return "Charge is clean and ready to submit.";
    }

    return appointment.billing.warnings
      .slice(0, 2)
      .map((warning) => warning.message)
      .join(" ");
  }

  updateDraft(
    appointment: AppointmentResponseDto,
    kind: PrerequisiteKind,
    field: DraftField,
    value: string,
  ): void {
    this.getDraft(appointment, kind)[field] = value;
  }

  draftValue(
    appointment: AppointmentResponseDto,
    kind: PrerequisiteKind,
    field: DraftField,
  ): string {
    return this.getDraft(appointment, kind)[field];
  }

  chargeDraftValue(
    appointment: AppointmentResponseDto,
    field: keyof ChargeDraft,
  ): string | number {
    return this.getChargeDraft(appointment)[field];
  }

  updateChargeDraft(
    appointment: AppointmentResponseDto,
    field: keyof ChargeDraft,
    value: string,
  ): void {
    const draft = this.getChargeDraft(appointment);

    if (field === "units" || field === "amount") {
      draft[field] = Number(value);
      return;
    }

    draft[field] = value;
  }

  savePrerequisite(
    appointment: AppointmentResponseDto,
    kind: PrerequisiteKind,
    status: Exclude<PrerequisiteStatus, "notRequired" | "expired">,
  ): void {
    this.errorMessage = "";
    const summary = this.getSummary(appointment, kind);
    const draft = this.getDraft(appointment, kind);
    const savingKey = `${appointment.id}-${kind}`;
    this.prerequisiteSavingKey = savingKey;

    const payload: CreateAppointmentPrerequisiteDto = {
      patientId: appointment.patientId,
      kind,
      status,
      dueDate: draft.dueDate || null,
      expiresOn: draft.expiresOn || null,
      notes: draft.notes.trim(),
    };

    const request$ = summary.id
      ? this.appointmentService.updateAppointmentPrerequisite(summary.id, {
          status,
          dueDate: payload.dueDate,
          expiresOn: payload.expiresOn,
          notes: payload.notes,
        })
      : this.appointmentService.createAppointmentPrerequisite(appointment.id, payload);

    request$.subscribe({
      next: () => {
        this.loadData(() => {
          this.prerequisiteSavingKey = null;
        });
      },
      error: () => {
        this.errorMessage = `Unable to update ${kind} details for this appointment.`;
        this.prerequisiteSavingKey = null;
      },
    });
  }

  isSavingPrerequisite(appointmentId: number, kind: PrerequisiteKind): boolean {
    return this.prerequisiteSavingKey === `${appointmentId}-${kind}`;
  }

  saveCharge(appointment: AppointmentResponseDto): void {
    this.errorMessage = "";
    this.chargeSavingId = appointment.id;
    const draft = this.getChargeDraft(appointment);

    const payload: CreateAppointmentChargeDto = {
      diagnosisCode: draft.diagnosisCode.trim(),
      procedureCode: draft.procedureCode.trim(),
      modifier: draft.modifier.trim(),
      units: Number.isFinite(draft.units) ? draft.units : 0,
      amount: Number.isFinite(draft.amount) ? draft.amount : 0,
      notes: draft.notes.trim(),
    };

    const request$ = appointment.billing.chargeId
      ? this.appointmentService.updateAppointmentCharge(appointment.id, payload)
      : this.appointmentService.createAppointmentCharge(appointment.id, payload);

    request$.subscribe({
      next: (charge) => {
        this.chargeDrafts[appointment.id] = this.mapChargeToDraft(charge);
        this.loadData(() => {
          this.chargeSavingId = null;
        });
      },
      error: () => {
        this.errorMessage = "Unable to update charge details for this appointment.";
        this.chargeSavingId = null;
      },
    });
  }

  isSavingCharge(appointmentId: number): boolean {
    return this.chargeSavingId === appointmentId;
  }

  simulateEligibilityCheck(
    appointment: AppointmentResponseDto,
    eligibilityStatus: EligibilityStatus,
  ): void {
    this.errorMessage = "";
    this.eligibilityUpdatingId = appointment.id;

    this.appointmentService.updateEligibility(appointment.id, {
      eligibilityStatus,
      eligibilityNotes: this.defaultEligibilityNote(eligibilityStatus),
      eligibilityReviewedAt: new Date().toISOString(),
    }).subscribe({
      next: () => {
        this.loadData(() => {
          this.eligibilityUpdatingId = null;
        });
      },
      error: () => {
        this.errorMessage = "Unable to update appointment eligibility.";
        this.eligibilityUpdatingId = null;
      },
    });
  }

  patientName(patientId: number): string {
    const patient = this.patients.find((item) => item.id === patientId);
    return patient ? `${patient.firstName} ${patient.lastName}` : `Patient #${patientId}`;
  }

  providerName(providerId: number): string {
    const provider = this.providers.find((item) => item.id === providerId);
    return provider?.name ?? `Provider #${providerId}`;
  }

  private loadData(onComplete?: () => void): void {
    forkJoin({
      patients: this.patientService.getPatients(),
      providers: this.providerService.getProviders(),
      appointments: this.appointmentService.getAppointments(),
    }).subscribe({
      next: ({ patients, providers, appointments }) => {
        this.patients = patients;
        this.providers = providers;
        this.appointments = this.sortAppointments(appointments);
        onComplete?.();
      },
      error: () => {
        this.errorMessage = "Unable to load scheduler data.";
        onComplete?.();
      },
    });
  }

  private loadAppointments(onComplete?: () => void): void {
    this.appointmentService.getAppointments().subscribe({
      next: (appointments) => {
        this.appointments = this.sortAppointments(appointments);
        onComplete?.();
      },
      error: () => {
        this.errorMessage = "Unable to refresh appointments after scheduling.";
        onComplete?.();
      },
    });
  }

  private getDraft(
    appointment: AppointmentResponseDto,
    kind: PrerequisiteKind,
  ): PrerequisiteDraft {
    const key = `${appointment.id}-${kind}`;
    const summary = this.getSummary(appointment, kind);

    this.prerequisiteDrafts[key] ??= {
      dueDate: summary.dueDate ?? "",
      expiresOn: summary.expiresOn ?? "",
      notes: summary.notes ?? "",
    };

    return this.prerequisiteDrafts[key];
  }

  private getChargeDraft(appointment: AppointmentResponseDto): ChargeDraft {
    const existingDraft = this.chargeDrafts[appointment.id];
    if (existingDraft) {
      return existingDraft;
    }

    this.chargeDrafts[appointment.id] = {
      diagnosisCode: "",
      procedureCode: "",
      modifier: "",
      units: 1,
      amount: 0,
      notes: "",
    };

    if (appointment.billing.chargeId) {
      this.appointmentService.getAppointmentCharge(appointment.id).subscribe({
        next: (charge) => {
          this.chargeDrafts[appointment.id] = this.mapChargeToDraft(charge);
        },
      });
    }

    return this.chargeDrafts[appointment.id];
  }

  private appointmentsForDay(date: Date): AppointmentResponseDto[] {
    const target = this.startOfDay(date).getTime();
    return this.filteredAppointments.filter((appointment) => {
      return this.startOfDay(this.parseAppointmentDate(appointment)).getTime() === target;
    });
  }

  private get filteredAppointments(): AppointmentResponseDto[] {
    const providerId = this.appointmentForm.controls.providerId.value;
    if (providerId <= 0) {
      return this.appointments;
    }

    return this.appointments.filter((appointment) => appointment.providerId === providerId);
  }

  private sortAppointments(appointments: AppointmentResponseDto[]): AppointmentResponseDto[] {
    return [...appointments].sort((left, right) => {
      return this.parseAppointmentDate(left).getTime() - this.parseAppointmentDate(right).getTime();
    });
  }

  private mapChargeToDraft(charge: AppointmentChargeDto): ChargeDraft {
    return {
      diagnosisCode: charge.diagnosisCode ?? "",
      procedureCode: charge.procedureCode ?? "",
      modifier: charge.modifier ?? "",
      units: charge.units || 0,
      amount: charge.amount || 0,
      notes: charge.notes ?? "",
    };
  }

  private parseAppointmentDate(appointment: AppointmentResponseDto): Date {
    return new Date(appointment.appointmentDate);
  }

  private startOfDay(date: Date): Date {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate());
  }

  private startOfWeek(date: Date): Date {
    const normalized = this.startOfDay(date);
    return this.addDays(normalized, -normalized.getDay());
  }

  private addDays(date: Date, amount: number): Date {
    const next = new Date(date);
    next.setDate(next.getDate() + amount);
    return this.startOfDay(next);
  }

  private formatDateLabel(date: Date, format: "short" | "long"): string {
    return date.toLocaleDateString(undefined, {
      month: format === "long" ? "long" : "short",
      day: "numeric",
      year: format === "long" ? "numeric" : undefined,
    });
  }

  private formatOptionalDate(value: string): string {
    return new Date(value).toLocaleDateString(undefined, {
      month: "short",
      day: "numeric",
    });
  }

  private slotKey(date: Date, slot: TimeSlot): string {
    return `${this.startOfDay(date).toISOString()}-${slot.hour}-${slot.minute}`;
  }

  private defaultEligibilityNote(status: EligibilityStatus): string {
    switch (status) {
      case "verified":
        return "Coverage verified for the scheduled visit.";
      case "failed":
        return "Coverage issue found. Follow up before check-in.";
      default:
        return "Eligibility review is still pending.";
    }
  }
}
