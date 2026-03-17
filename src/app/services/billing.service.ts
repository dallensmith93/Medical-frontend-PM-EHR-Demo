import { Injectable, inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";
import { ChargeReviewItemDto } from "../types/scheduler-api.types";

@Injectable({
  providedIn: "root",
})
export class BillingService {
  private readonly http = inject(HttpClient);
  private readonly reviewUrl = `${environment.apiUrl}/charges/review`;

  getReviewQueue(): Observable<ChargeReviewItemDto[]> {
    return this.http.get<ChargeReviewItemDto[]>(this.reviewUrl);
  }
}
