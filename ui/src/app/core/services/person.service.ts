import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { CreatePersonResponse, GetPeopleResponse, ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class PersonService {
  private apiUrl = `${environment.apiUrl}/Person`;

  constructor(private http: HttpClient) {}

  getAllPeople(): Observable<GetPeopleResponse> {
    return this.http.get<GetPeopleResponse>(this.apiUrl)
      .pipe(catchError(this.handleError));
  }

  getPersonByName(name: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${encodeURIComponent(name)}`)
      .pipe(catchError(this.handleError));
  }

  createPerson(name: string): Observable<CreatePersonResponse> {
    return this.http.post<CreatePersonResponse>(this.apiUrl, JSON.stringify(name), {
      headers: { 'Content-Type': 'application/json' }
    }).pipe(catchError(this.handleError));
  }

  updatePerson(currentName: string, newName: string): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(
      `${this.apiUrl}/${encodeURIComponent(currentName)}`,
      JSON.stringify(newName),
      { headers: { 'Content-Type': 'application/json' } }
    ).pipe(catchError(this.handleError));
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
