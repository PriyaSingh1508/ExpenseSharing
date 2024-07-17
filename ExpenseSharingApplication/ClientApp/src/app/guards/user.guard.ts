import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { AuthService } from '../modules/auth/auth.service';
import { IUser } from '../interfaces/iuser';

@Injectable({
  providedIn: 'root'
})
export class UserGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router){ }
 
  async canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Promise<boolean> {
 
    let user: IUser | false = await this.auth.isAuthorised();
    if(user){
      console.log("Hiii ",user.role);
      let role =  user.role== "User";
      if(role) return  true;
    }
 
    this.router.navigate(["/logout"]);
    return false;
  }
  
}
