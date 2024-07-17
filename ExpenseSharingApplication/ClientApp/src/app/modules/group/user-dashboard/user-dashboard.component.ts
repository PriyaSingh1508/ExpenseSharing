import { Component } from '@angular/core';
import { IGroupInfo } from 'src/app/interfaces/igroupInfo';
import { GroupService } from '../group.service';
import { HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { IResponse } from 'src/app/interfaces/i-response';


@Component({
  selector: 'app-user-dashboard',
  templateUrl: './user-dashboard.component.html',
  styleUrls: ['./user-dashboard.component.css']
})
export class UserDashboardComponent {
  headers = new HttpHeaders().set('Content-Type', 'application/json');
  public applicationUser: string = localStorage.getItem("email") || "";
  dataSource:any;
  displayedColumns:string[]=["id","groupName","groupDescription","groupCreatedDate","teamMembers","action"];
  public groupInfos:IGroupInfo[]=[];
 constructor(private groupService:GroupService,private router:Router){

 }
 ngOnInit(): void {
  this.getAllGroupsByUserEmail();
} 
private getAllGroupsByUserEmail() {
  let userEmail = localStorage.getItem("email");
  this.groupService.getData<IResponse<IGroupInfo[]>>(`account/get-all-group-by-user-email/${userEmail}`, { headers: this.headers }).subscribe(
    (data) => {
      if (data.isSuccess && data.data) {
        this.groupInfos = data.data.map(group => ({
          ...group,
          groupCreatedDate: new Date(group.groupCreatedDate)
        }));
      this.dataSource = this.groupInfos;
      console.log("Tech ", this.groupInfos);
    }
  },
    (error) => {
      console.log('Something went wrong: ', error);
    }
  );
}

  onExpenses(element:number){
    this.router.navigateByUrl(`user/expenses/${element}`)
  }
  onGroupDelete(element:number){
  this.groupService.deleteData<IResponse<boolean>>(`group/delete-group/${element}`, { headers: this.headers }).subscribe(
    (data)=>{
      if(data.isSuccess){
        console.log(data.message);
      }

    },
    (error)=>{
      console.log('Something went wrong: ', error);
    }
  );
  }

  createGroup(){
    this.router.navigateByUrl(`user/add-group`);
  }
}
