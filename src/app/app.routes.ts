import { Routes } from "@angular/router";
import { EhrPageComponent } from "./ehr-page.component";
import { PmPageComponent } from "./pm-page.component";

export const routes: Routes = [
  { path: "", pathMatch: "full", redirectTo: "pm/schedule" },
  { path: "pm", pathMatch: "full", redirectTo: "pm/schedule" },
  { path: "pm/schedule", component: PmPageComponent },
  { path: "pm/claims", component: PmPageComponent },
  { path: "pm/master-files", component: PmPageComponent },
  { path: "pm/user-settings", component: PmPageComponent },
  { path: "pm/reports", component: PmPageComponent },
  { path: "ehr", component: EhrPageComponent },
  { path: "ehr/encounter", component: EhrPageComponent },
  { path: "**", redirectTo: "pm/schedule" },
];
