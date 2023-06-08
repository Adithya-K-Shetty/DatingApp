import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css'],
})
export class RolesModalComponent implements OnInit {
  username = '';
  availableRoles: any[] = [];
  selectedRoles: any[] = [];

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit(): void {}

  updateChecked(checkedValue: string) {
    const index = this.selectedRoles.indexOf(checkedValue);
    //explanation for the above line
    //if a new element is selected then it wont be inside selectedroles
    //so it will give a index of -1
    //so if it gives a index of -1 then it is a new element which is been checked
    //then we have to add this to selected Roles
    //else dont add cause it is already been selected
    index !== -1
      ? this.selectedRoles.splice(index, 1)
      : this.selectedRoles.push(checkedValue);
  }
}
