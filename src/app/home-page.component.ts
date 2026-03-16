import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { RouterLink } from "@angular/router";

@Component({
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: "./home-page.component.html",
  styleUrl: "./entity-forms.css",
})
export class HomePageComponent {
  readonly actions = [
    {
      title: "Add Patient",
      description: "Create a new patient record and send it to the practice API.",
      route: "/patients",
    },
    {
      title: "Add Provider",
      description: "Maintain provider roster details and specialties.",
      route: "/providers",
    },
    {
      title: "Schedule Appointment",
      description: "Create new appointments with live patient and provider selections.",
      route: "/appointments",
    },
  ];
}
