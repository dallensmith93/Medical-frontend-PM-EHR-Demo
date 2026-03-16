export interface CreatePatientDto {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
}

export interface PatientDto extends CreatePatientDto {
  id: number;
}

export interface CreateProviderDto {
  name: string;
  specialty: string;
}

export interface ProviderDto extends CreateProviderDto {
  id: number;
}

export interface CreateAppointmentDto {
  patientId: number;
  providerId: number;
  appointmentDate: string;
  durationMinutes: number;
  reason: string;
}

export interface AppointmentResponseDto {
  id: number;
  patientId: number;
  providerId: number;
  appointmentDate: string;
  durationMinutes: number;
  reason: string;
}
