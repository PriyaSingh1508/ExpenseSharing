import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CommonService } from '../shared/common.service';
import { BehaviorSubject, Observable, firstValueFrom } from 'rxjs';
import { IUser } from 'src/app/interfaces/iuser';
import { IResponse } from 'src/app/interfaces/i-response';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  HEADERS = new HttpHeaders().set('Content-Type', 'application/json');
  public myUser!: IUser;
  private loggedIn: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  constructor(private apiService:CommonService) { }

  getData<T>(url:string,options:any): Observable<T> {
    return this.apiService.get<T>(url, options);
  }
  postData<T>(url:string,options:any,data:string):Observable<T>{
   return this.apiService.post<T>(url,options,data);
  }

  
  async isAuthorised(): Promise<IUser | false> {
    
    let user: IUser | null = await this.getUser();
    if(user == null) return false;
    return user;

  }

  checkUser(): IUser | false{
    const user  = localStorage.getItem('user');
    if(!user) return false;
    return JSON.parse(user);
  }

  async getUser(): Promise<IUser | null> {
    console.log(localStorage.getItem("token"));
    try {
      let userEmail= localStorage.getItem("email");
      let resObservable = this.getData<any>(`account/find-by-email/${userEmail}`, { headers: this.HEADERS });
      let res = await firstValueFrom(resObservable);
      localStorage.setItem("user", JSON.stringify(res.data));
      return res.data;
    } catch (ex) { 
      console.log(ex);
      return null; 
    }
  }

  get isUserLoggedIn(){
    if(localStorage.getItem('email') != null)
      this.loggedIn.next(true);
    return this.loggedIn.asObservable();
} 
}
