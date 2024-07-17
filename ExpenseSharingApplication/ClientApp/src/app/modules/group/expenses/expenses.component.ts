import { Component } from '@angular/core';
import { GroupService } from '../group.service';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';
import { iGetExpenses } from 'src/app/interfaces/iGetExpenses';
import { IResponse } from 'src/app/interfaces/i-response';

@Component({
  selector: 'app-expenses',
  templateUrl: './expenses.component.html',
  styleUrls: ['./expenses.component.css']
})
export class ExpensesComponent {
  headers = new HttpHeaders().set('Content-Type', 'application/json');
  dataSource:any
  displayedColumns:string[]=["id","groupId","desc","amount","paidBy","date","splitAmong","individualAmount","action"];
  public groupInfos:iGetExpenses[]=[];
 constructor(private groupService:GroupService,private router:Router,private route: ActivatedRoute,){}
 ngOnInit(): void {
  this.route.params.subscribe(params => {
    if (params['id']) {
      this.getAllExpensesOfAGroup( params['id']);
    }
  });
} 
private getAllExpensesOfAGroup(id:number){
  this.groupService.getData<IResponse<iGetExpenses[]>>(`expense/get-all-expenses-of-a-group/${id}`,{ headers: this.headers }).subscribe(
   (data) => {
    if (data.isSuccess && data.data) {
      this.groupInfos=data.data;  
      this.dataSource=data.data;}
   },
   (error) => {
     console.log('Something went wrong: ', error);
   }
 );
 }

 expenseSettlement(element:number){
  this.groupService.getData<IResponse<string>>(`expense/expense-settlement/${element}`,{headers: this.headers}).subscribe(
    (data) => {
      console.log(data.message);
      this.route.params.subscribe(params => {
        if (params['id']) {
          this.getAllExpensesOfAGroup( params['id']);
        }
      });
   },
   (error) => {
     console.log('Something went wrong: ', error);
   }
  )
 }
  amtContributeByUser(element:number){
    const amtContributedInfoObj = {
      email:localStorage.getItem("email") ,
      expenseId: element,
    };
    this.groupService.postData<IResponse<string>>(`expense/amount-contributed-by-user`,{headers: this.headers},JSON.stringify(amtContributedInfoObj)).subscribe(
      (data) => {
        console.log(data.message);
        this.route.params.subscribe(params => {
          if (params['id']) {
            this.getAllExpensesOfAGroup( params['id']);
          }
        });
     },
     (error) => {
       console.log('Something went wrong: ', error);
     }
    )
  }

  canPay(element: iGetExpenses): boolean {
    const email = localStorage.getItem("email")!;
    return element.splitAmong.includes(email) && !element.contributedBy.includes(email);
}

 canSettleExpense(element: iGetExpenses):boolean{
  const email =localStorage.getItem("email")!;
  return element.paidBy==email && !element.isSettled;
 }
 createExpense(){
  this.router.navigateByUrl(`user/add-expense`);
}
}
