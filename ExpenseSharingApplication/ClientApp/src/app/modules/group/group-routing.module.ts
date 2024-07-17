import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserDashboardComponent } from './user-dashboard/user-dashboard.component';
import { AddExpensComponent } from './add-expens/add-expens.component';
import { AddGroupComponent } from './add-group/add-group.component';
import { ExpensesComponent } from './expenses/expenses.component';

const routes: Routes = [
  {path:'',component:UserDashboardComponent},
  {path:'add-expense',component:AddExpensComponent},
  {path:'add-group',component:AddGroupComponent},
  {path:'expenses/:id',component:ExpensesComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GroupRoutingModule { }
