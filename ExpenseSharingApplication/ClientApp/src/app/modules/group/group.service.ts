import { Injectable } from '@angular/core';
import { CommonService } from '../shared/common.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GroupService {
 
  constructor(private apiService:CommonService) { }

  getData<T>(url:string,options:any): Observable<T> {
    return this.apiService.get<T>(url, options);
  }
  postData<T>(url:string,options:any,data:string):Observable<T>{
   return this.apiService.post<T>(url,options,data);
  }
  deleteData<T>(url:string,options:any):Observable<T>{
  return this.apiService.delete<T>(url,options);
  }
}
