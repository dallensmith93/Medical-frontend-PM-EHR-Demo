import { Routes } from "@angular/router";
import { AppointmentSchedulerComponent } from "./appointment-scheduler.component";
import { EhrPageComponent } from "./ehr-page.component";
import { HomePageComponent } from "./home-page.component";
import { PmPageComponent } from "./pm-page.component";
import { PatientFormComponent } from "./patient-form.component";
import { ProviderFormComponent } from "./provider-form.component";

export const routes: Routes = [
  { path: "", component: HomePageComponent },
  { path: "patients", component: PatientFormComponent },
  { path: "providers", component: ProviderFormComponent },
  { path: "appointments", component: AppointmentSchedulerComponent },
  { path: "pm", pathMatch: "full", redirectTo: "pm/schedule" },
  { path: "pm/schedule", component: PmPageComponent },
  { path: "pm/claims", component: PmPageComponent },
  { path: "pm/master-files", component: PmPageComponent },
  { path: "pm/user-settings", component: PmPageComponent },
  { path: "pm/reports", component: PmPageComponent },
  { path: "ehr", component: EhrPageComponent },
  { path: "ehr/encounter", component: EhrPageComponent },
  { path: "**", redirectTo: "" },
];
