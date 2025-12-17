import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { CreateAstronautDutyRequest, AstronautDutyHistory } from '../models/astronaut-duty.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class AstronautDutyService {
  private apiUrl = `${environment.apiUrl}/AstronautDuty`; 

  constructor(private http: HttpClient) {}

  createAstronautDuty(request: CreateAstronautDutyRequest): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(this.apiUrl, request)
      .pipe(catchError(this.handleError));
  }

  getAstronautDutiesByName(name: string): Observable<AstronautDutyHistory> {
    return this.http.get<AstronautDutyHistory>(`${this.apiUrl}/${encodeURIComponent(name)}`)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An error occurred';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = error.error?.message || `Server returned code ${error.status}`;
    }
    return throwError(() => new Error(errorMessage));
  }
}
