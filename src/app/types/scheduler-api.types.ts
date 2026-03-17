export type EligibilityStatus = "verified" | "pending" | "failed";
export type IntakeStatus = "notStarted" | "inProgress" | "complete";
export type PrerequisiteKind = "authorization" | "referral";
export type PrerequisiteStatus =
  | "notRequired"
  | "needed"
  | "submitted"
  | "approved"
  | "denied"
  | "expired";
export type BillingStatus = "draft" | "reviewNeeded" | "readyToSubmit";

export interface CreatePatientDto {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  phoneNumber: string;
  email: string;
  addressLine1: string;
  city: string;
  state: string;
  postalCode: string;
  payerName: string;
  memberId: string;
  insuranceSummary: string;
  eligibilityStatus: EligibilityStatus;
  lastEligibilityVerifiedAt: string | null;
  eligibilityNotes: string;
  intakeStatus: IntakeStatus;
  intakeNotes: string;
}

export interface PatientDto extends CreatePatientDto {
  id: number;
  missingIntakeItems: string[];
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
  eligibilityStatus: EligibilityStatus;
  eligibilityReviewedAt: string | null;
  eligibilityNotes: string;
  intakeStatus: IntakeStatus;
  isIntakeComplete: boolean;
  missingIntakeItems: string[];
  authorization: AppointmentPrerequisiteSummaryDto;
  referral: AppointmentPrerequisiteSummaryDto;
  hasPrerequisiteBlocker: boolean;
  billing: AppointmentBillingSummaryDto;
}

export interface UpdateAppointmentEligibilityDto {
  eligibilityStatus: EligibilityStatus;
  eligibilityReviewedAt?: string | null;
  eligibilityNotes: string;
}

export interface AppointmentPrerequisiteDto {
  id: number;
  appointmentId: number;
  patientId: number;
  kind: PrerequisiteKind;
  status: Exclude<PrerequisiteStatus, "notRequired" | "expired">;
  dueDate: string | null;
  expiresOn: string | null;
  notes: string;
}

export interface AppointmentPrerequisiteSummaryDto {
  id: number | null;
  kind: PrerequisiteKind;
  isRequired: boolean;
  status: PrerequisiteStatus;
  dueDate: string | null;
  expiresOn: string | null;
  notes: string;
  isBlocking: boolean;
}

export interface CreateAppointmentPrerequisiteDto {
  appointmentId?: number;
  patientId: number;
  kind: PrerequisiteKind;
  status: Exclude<PrerequisiteStatus, "notRequired" | "expired">;
  dueDate: string | null;
  expiresOn: string | null;
  notes: string;
}

export interface UpdateAppointmentPrerequisiteDto {
  status: Exclude<PrerequisiteStatus, "notRequired" | "expired">;
  dueDate: string | null;
  expiresOn: string | null;
  notes: string;
}

export interface ClaimScrubWarningDto {
  code: string;
  message: string;
  isBlocking: boolean;
}

export interface AppointmentBillingSummaryDto {
  chargeId: number | null;
  status: BillingStatus;
  isReadyToSubmit: boolean;
  warningCount: number;
  warnings: ClaimScrubWarningDto[];
}

export interface AppointmentChargeDto {
  id: number | null;
  appointmentId: number;
  patientId: number;
  providerId: number;
  diagnosisCode: string;
  procedureCode: string;
  modifier: string;
  units: number;
  amount: number;
  notes: string;
  billing: AppointmentBillingSummaryDto;
}

export interface CreateAppointmentChargeDto {
  diagnosisCode: string;
  procedureCode: string;
  modifier: string;
  units: number;
  amount: number;
  notes: string;
}

export interface UpdateAppointmentChargeDto extends CreateAppointmentChargeDto {}

export interface ChargeReviewItemDto {
  appointmentId: number;
  chargeId: number | null;
  patientId: number;
  patientName: string;
  payerName: string;
  providerId: number;
  providerName: string;
  appointmentDate: string;
  reason: string;
  status: BillingStatus;
  warningCount: number;
  warnings: ClaimScrubWarningDto[];
}
