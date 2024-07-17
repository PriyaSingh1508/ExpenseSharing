import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommonService {

  private baseUrl='http://localhost:5077';
  constructor(private http: HttpClient) { }

  public get<T>(url: string, options: any): Observable<T>{
    return this.http.get<T>(`${this.baseUrl}/${url}`,<Object>options).pipe(catchError(this.handleError));
  }

  public delete<T>(url: string, options: any): Observable<T>{
    return this.http.delete<T>(`${this.baseUrl}/${url}`,<Object>options).pipe(catchError(this.handleError));
  }

  public post<T>(url: string, options: any, data: string): Observable<T>{
    return this.http.post<T>(`${this.baseUrl}/${url}`,data,<Object>options).pipe(catchError(this.handleError));
  }

  public postMultiPart<T>(url: string, options: any, data: FormData): Observable<T>{
    return this.http.post<T>(`${this.baseUrl}/${url}`,data,<Object>options).pipe(catchError(this.handleError));
  }

  public put<T>(url: string, options: any, data: string): Observable<T>{
    return this.http.put<T>(`${this.baseUrl}/${url}`,data,<Object>options).pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    console.log(error);


    console.error(`Code: ${error.status}, Message: `, error.error);
    if (error.status === 0) {
     
      console.error('An error occurred:', error.error);    
    } 
    else if(error.status === 419){
    }

    else if(error.status === 404){
    }

    else if(error.status === 500){
    }    
    return throwError(() => new Error('Something bad happened; please try again later.'));
  }
}
