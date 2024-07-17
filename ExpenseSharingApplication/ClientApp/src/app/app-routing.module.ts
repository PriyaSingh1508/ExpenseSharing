import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserGuard } from './guards/user.guard';

const routes: Routes = [
    {
        path: '',
        loadChildren:()=> import("../app/modules/auth/auth.module").then(m=>m.AuthModule)
      },
      {
        path:'user',
        children:[  { path: "" ,loadChildren:()=>import("../app/modules/group/group.module").then(m=>m.GroupModule)}],
       canActivate:[UserGuard]
      }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
