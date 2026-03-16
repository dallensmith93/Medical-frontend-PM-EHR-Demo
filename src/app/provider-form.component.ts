import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { finalize } from "rxjs";
import { ProviderDto } from "./types/scheduler-api.types";
import { ProviderService } from "./services/provider.service";

@Component({
  standalone: true,
  selector: "app-provider-form",
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: "./provider-form.component.html",
  styleUrl: "./entity-forms.css",
})
export class ProviderFormComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly providerService = inject(ProviderService);

  readonly providerForm = this.formBuilder.nonNullable.group({
    name: ["", [Validators.required, Validators.maxLength(100)]],
    specialty: ["", [Validators.required, Validators.maxLength(100)]],
  });

  providers: ProviderDto[] = [];
  isSaving = false;

  constructor() {
    this.loadProviders();
  }

  createProvider(): void {
    if (this.providerForm.invalid) {
      this.providerForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;

    this.providerService
      .createProvider(this.providerForm.getRawValue())
      .pipe(finalize(() => (this.isSaving = false)))
      .subscribe({
        next: (provider) => {
          this.providers = [...this.providers, provider];
          this.providerForm.reset({
            name: "",
            specialty: "",
          });
        },
      });
  }

  private loadProviders(): void {
    this.providerService.getProviders().subscribe({
      next: (providers) => {
        this.providers = providers;
      },
    });
  }
}
