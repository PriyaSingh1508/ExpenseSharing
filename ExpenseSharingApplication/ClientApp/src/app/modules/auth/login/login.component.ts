import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';
import { IResponse } from 'src/app/interfaces/i-response';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  loginForm: any = FormGroup;

  constructor(private fb: FormBuilder, private authService:AuthService,private router:Router) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$")]],
      password: ['',  [Validators.required]],
    });
  }
 
  login(){
    const headers = new HttpHeaders().set('Content-Type', 'application/json');

    const loginObj = {
      Email: this.loginForm.get("email").value,
      Password: this.loginForm.get("password").value,
    };

    const req = this.authService.postData(`account/Login`, { headers: headers }, JSON.stringify(loginObj));
    

    try{
      req.subscribe(async (res: any) => {
        console.log(res.data); 
        localStorage.setItem("token",res.data.token);
        localStorage.setItem("email",res.data.email);
        this.router.navigate(['/user']);
      });
    }
    catch(ex){
     console.log(ex);
     }
   }
 }

