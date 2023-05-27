import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  registerMode = false;
  users: any;
  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.getUsers();
  }
  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  getUsers() {
    //intial request fails because of cross origin request
    this.http.get('http://localhost:5033/api/users').subscribe({
      next: (response) => (this.users = response),
      error: (error) => console.log(error),
      complete: () => console.log('Request Has Completed'),
    }); //basically this returns the observable to which we have to subscribe to get the data
  }

  cancleRegisterMode(event: boolean) {
    this.registerMode = event;
  }
}
