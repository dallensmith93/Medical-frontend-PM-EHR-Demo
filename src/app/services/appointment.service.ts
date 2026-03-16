import { Injectable, inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";
import {
  AppointmentResponseDto,
  CreateAppointmentDto,
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
}
