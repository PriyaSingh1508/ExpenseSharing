import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GroupRoutingModule } from './group-routing.module';
import { AddGroupComponent } from './add-group/add-group.component';
import { AddExpensComponent } from './add-expens/add-expens.component';
import { UserDashboardComponent } from './user-dashboard/user-dashboard.component';
import { SharedModule } from '../shared/shared.module';
import { ExpensesComponent } from './expenses/expenses.component';

@NgModule({
  declarations: [
    AddGroupComponent,
    AddExpensComponent,
    UserDashboardComponent,
    ExpensesComponent
  ],
  imports: [
    CommonModule,
    GroupRoutingModule,
    SharedModule
  ]
})
export class GroupModule { }
