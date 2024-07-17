import { Component } from '@angular/core';
import { AuthService } from '../modules/auth/auth.service';
import { Observable, of } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  public applicationUser: string = localStorage.getItem("email") || "";
  isExpanded = false;
  constructor(private authservice : AuthService,private router:Router) { }
  isLoggedIn$: Observable<boolean>=new Observable();

  ngOnInit() {
    this.isLoggedIn$ = this.authservice.isUserLoggedIn;
  }
  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
  logout(){
    this.router.navigate(['logout']);
  }
}
