import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { forkJoin } from "rxjs";
import { AppointmentService } from "./services/appointment.service";
import { PatientService } from "./services/patient.service";
import { ProviderService } from "./services/provider.service";
import {
  AppointmentResponseDto,
  PatientDto,
  ProviderDto,
} from "./types/scheduler-api.types";

type CalendarView = "month" | "week" | "day";
type SlotState = "open" | "occupied" | "selected";

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
  selectedSlotKey = "";

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

  patientName(patientId: number): string {
    const patient = this.patients.find((item) => item.id === patientId);
    return patient ? `${patient.firstName} ${patient.lastName}` : `Patient #${patientId}`;
  }

  providerName(providerId: number): string {
    const provider = this.providers.find((item) => item.id === providerId);
    return provider?.name ?? `Provider #${providerId}`;
  }

  private loadData(): void {
    forkJoin({
      patients: this.patientService.getPatients(),
      providers: this.providerService.getProviders(),
      appointments: this.appointmentService.getAppointments(),
    }).subscribe({
      next: ({ patients, providers, appointments }) => {
        this.patients = patients;
        this.providers = providers;
        this.appointments = this.sortAppointments(appointments);
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

  private slotKey(date: Date, slot: TimeSlot): string {
    return `${this.startOfDay(date).toISOString()}-${slot.hour}-${slot.minute}`;
  }
}
