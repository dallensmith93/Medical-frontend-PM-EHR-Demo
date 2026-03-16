import { Injectable, inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";
import { CreateProviderDto, ProviderDto } from "../types/scheduler-api.types";

@Injectable({
  providedIn: "root",
})
export class ProviderService {
  private readonly http = inject(HttpClient);
  private readonly providersUrl = `${environment.apiUrl}/providers`;

  getProviders(): Observable<ProviderDto[]> {
    return this.http.get<ProviderDto[]>(this.providersUrl);
  }

  createProvider(provider: CreateProviderDto): Observable<ProviderDto> {
    return this.http.post<ProviderDto>(this.providersUrl, provider);
  }
}
