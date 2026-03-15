import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";
import { alerts } from "./mock-data";

@Component({
  selector: "app-root",
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.css",
})
export class AppComponent {
  readonly alerts = alerts;

  constructor(public router: Router) {}

  get workspaceLabel(): string {
    return this.router.url.startsWith("/ehr") ? "EHR" : "PM";
  }
}
