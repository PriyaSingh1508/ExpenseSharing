import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GroupService } from '../group.service';
import { Router } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { IResponse } from 'src/app/interfaces/i-response';

@Component({
  selector: 'app-add-expens',
  templateUrl: './add-expens.component.html',
  styleUrls: ['./add-expens.component.css']
})
export class AddExpensComponent {
  addExpenseForm: any = FormGroup;
  constructor(private fb: FormBuilder, private groupService:GroupService,private router:Router,private datePipe: DatePipe) {
    this.addExpenseForm = this.fb.group({
      groupId: [0, [Validators.required]],
      description: ['',  [Validators.required]],
      amount: [0,[Validators.required]],
      paidBy: ['',[Validators.required]],
      splitAmong:['',[Validators.required]],
      date:['',[Validators.required]]
    });
  }
 
  addExpense(){
    const headers = new HttpHeaders().set('Content-Type', 'application/json');

    const expenseObj = {
      ExpenseId:0,
      GroupId: this.addExpenseForm.get("groupId").value,
      Description: this.addExpenseForm.get("description").value,
      Amount:this.addExpenseForm.get("amount").value,
      PaidBy:this.addExpenseForm.get("paidBy").value,
      SplitAmong:this.addExpenseForm.get("splitAmong").value,
      
      Date: new Date(this.addExpenseForm.get("date").value).toISOString(),
      IndividualAmount:0
    };

    const req = this.groupService.postData<IResponse<string>>(`expense/create-expense`, { headers: headers }, JSON.stringify(expenseObj));
    

    try{
      req.subscribe(async (res: any) => {
        console.log(res.message); 
        this.router.navigateByUrl(`user`)
      });
    }
    catch(ex){
     console.log(ex);
     }
   }
}
