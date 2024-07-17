import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GroupService } from '../group.service';
import { Router } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';
import { IResponse } from 'src/app/interfaces/i-response';

@Component({
  selector: 'app-add-group',
  templateUrl: './add-group.component.html',
  styleUrls: ['./add-group.component.css']
})
export class AddGroupComponent {
  addGroupForm: any = FormGroup;

  constructor(private fb: FormBuilder, private groupService:GroupService,private router:Router) {
    this.addGroupForm = this.fb.group({
      name: ['', [Validators.required]],
      description: ['',  [Validators.required]],
      teamMembers: ['',[Validators.required]]
    });
  }
 
  addGroup(){
    const headers = new HttpHeaders().set('Content-Type', 'application/json');

    const groupObj = {
      GroupName: this.addGroupForm.get("name").value,
      GroupDescription: this.addGroupForm.get("description").value,
      TeamMembers:this.addGroupForm.get("teamMembers").value,
      TotalMembers:0
    };

    const req = this.groupService.postData<IResponse<string>>(`group/add-group`, { headers: headers }, JSON.stringify(groupObj));
    

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
