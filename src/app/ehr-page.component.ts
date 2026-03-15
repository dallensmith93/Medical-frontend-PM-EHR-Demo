import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import { chartTabs, ehrMetrics, patients, timeline } from "./mock-data";

@Component({
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: "./ehr-page.component.html",
})
export class EhrPageComponent {
  readonly metrics = ehrMetrics;
  readonly patients = patients;
  readonly chartTabs = chartTabs;
  readonly timeline = timeline;

  activeChartTab = "Summary";
  activePatient = this.patients[0];

  constructor(public router: Router) {}

  get encounterOpen(): boolean {
    return this.router.url.includes("/encounter");
  }

  selectPatient(patientId: string): void {
    const patient = this.patients.find((item) => item.id === patientId);
    if (patient) {
      this.activePatient = patient;
    }
  }

  selectChartTab(tab: string): void {
    this.activeChartTab = tab;
  }
}
