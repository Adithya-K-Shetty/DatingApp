import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html', //this is the template
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  constructor(private http: HttpClient) {} // dependency injection
  ngOnInit(): void {
    //intial request fails because of cross origin request
    this.http.get('http://localhost:5033/api/users').subscribe({
      next: (response) => (this.users = response),
      error: (error) => console.log(error),
      complete: () => console.log('Request Has Completed'),
    }); //basically this returns the observable to which we have to subscribe to get the data
  }
  title = 'Dating App';
  users: any;
}
