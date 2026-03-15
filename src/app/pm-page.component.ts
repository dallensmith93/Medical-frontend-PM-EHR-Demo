import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import {
  claimQueue,
  masterFiles,
  Patient,
  patients,
  pmMetrics,
  reports,
  schedule,
  userSettings,
} from "./mock-data";

@Component({
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: "./pm-page.component.html",
})
export class PmPageComponent {
  readonly metrics = pmMetrics;
  readonly schedule = schedule;
  readonly claimQueue = claimQueue;
  readonly masterFiles = masterFiles;
  readonly userSettings = userSettings;
  readonly reports = reports;
  readonly patients = patients;

  activePatient: Patient = this.patients[0];

  constructor(public router: Router) {}

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
}
