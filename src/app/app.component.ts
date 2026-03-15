import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";
import { alerts } from "./mock-data";

type ThemeMode = "light" | "dark";

@Component({
  selector: "app-root",
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.css",
})
export class AppComponent {
  readonly alerts = alerts;
  theme: ThemeMode = "light";

  constructor(public router: Router) {
    const savedTheme = localStorage.getItem("app-theme");
    this.theme = savedTheme === "dark" ? "dark" : "light";
  }

  get workspaceLabel(): string {
    return this.router.url.startsWith("/ehr") ? "EHR" : "PM";
  }

  toggleTheme(): void {
    this.theme = this.theme === "light" ? "dark" : "light";
    localStorage.setItem("app-theme", this.theme);
  }
}
