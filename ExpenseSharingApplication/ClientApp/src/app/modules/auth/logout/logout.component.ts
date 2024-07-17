import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';


@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.css']
})
export class LogoutComponent {
  constructor(private router: Router) { }
  
  ngOnInit(): void {
    localStorage.removeItem("email");
    localStorage.removeItem("token");
   
    this.router.navigate([""]);
  }
}
