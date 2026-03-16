import { Injectable, inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";
import { CreatePatientDto, PatientDto } from "../types/scheduler-api.types";

@Injectable({
  providedIn: "root",
})
export class PatientService {
  private readonly http = inject(HttpClient);
  private readonly patientsUrl = `${environment.apiUrl}/patients`;

  getPatients(): Observable<PatientDto[]> {
    return this.http.get<PatientDto[]>(this.patientsUrl);
  }

  createPatient(patient: CreatePatientDto): Observable<PatientDto> {
    return this.http.post<PatientDto>(this.patientsUrl, patient);
  }
}
