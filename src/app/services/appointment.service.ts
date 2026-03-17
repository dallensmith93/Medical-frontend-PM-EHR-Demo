import { Injectable, inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";
import {
  AppointmentPrerequisiteDto,
  AppointmentChargeDto,
  AppointmentResponseDto,
  CreateAppointmentPrerequisiteDto,
  CreateAppointmentChargeDto,
  CreateAppointmentDto,
  UpdateAppointmentChargeDto,
  UpdateAppointmentPrerequisiteDto,
  UpdateAppointmentEligibilityDto,
} from "../types/scheduler-api.types";

@Injectable({
  providedIn: "root",
})
export class AppointmentService {
  private readonly http = inject(HttpClient);
  private readonly appointmentsUrl = `${environment.apiUrl}/appointments`;

  getAppointments(): Observable<AppointmentResponseDto[]> {
    return this.http.get<AppointmentResponseDto[]>(this.appointmentsUrl);
  }

  createAppointment(
    appointment: CreateAppointmentDto,
  ): Observable<AppointmentResponseDto> {
    return this.http.post<AppointmentResponseDto>(
      this.appointmentsUrl,
      appointment,
    );
  }

  updateEligibility(
    appointmentId: number,
    update: UpdateAppointmentEligibilityDto,
  ): Observable<AppointmentResponseDto> {
    return this.http.put<AppointmentResponseDto>(
      `${this.appointmentsUrl}/${appointmentId}/eligibility`,
      update,
    );
  }

  getAppointmentPrerequisites(appointmentId: number): Observable<AppointmentPrerequisiteDto[]> {
    return this.http.get<AppointmentPrerequisiteDto[]>(
      `${this.appointmentsUrl}/${appointmentId}/prerequisites`,
    );
  }

  createAppointmentPrerequisite(
    appointmentId: number,
    prerequisite: CreateAppointmentPrerequisiteDto,
  ): Observable<AppointmentPrerequisiteDto> {
    return this.http.post<AppointmentPrerequisiteDto>(
      `${this.appointmentsUrl}/${appointmentId}/prerequisites`,
      prerequisite,
    );
  }

  updateAppointmentPrerequisite(
    prerequisiteId: number,
    update: UpdateAppointmentPrerequisiteDto,
  ): Observable<AppointmentPrerequisiteDto> {
    return this.http.put<AppointmentPrerequisiteDto>(
      `${this.appointmentsUrl}/prerequisites/${prerequisiteId}`,
      update,
    );
  }

  getAppointmentCharge(appointmentId: number): Observable<AppointmentChargeDto> {
    return this.http.get<AppointmentChargeDto>(
      `${this.appointmentsUrl}/${appointmentId}/charge`,
    );
  }

  createAppointmentCharge(
    appointmentId: number,
    charge: CreateAppointmentChargeDto,
  ): Observable<AppointmentChargeDto> {
    return this.http.post<AppointmentChargeDto>(
      `${this.appointmentsUrl}/${appointmentId}/charge`,
      charge,
    );
  }

  updateAppointmentCharge(
    appointmentId: number,
    charge: UpdateAppointmentChargeDto,
  ): Observable<AppointmentChargeDto> {
    return this.http.put<AppointmentChargeDto>(
      `${this.appointmentsUrl}/${appointmentId}/charge`,
      charge,
    );
  }
}
