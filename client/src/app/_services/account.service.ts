import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root',
})
//main advantage is that they are created and destroyed along with
//the application intialization and Shutdown
export class AccountService {
  baseUrl = 'http://localhost:5033/api/';
  private currentUserSource = new BehaviorSubject<User | null>(null);
  //this oberservable is being used inside nav component
  //where it is being subscribed
  //to check whether the authentication is been done
  //bascially checking whether data is still
  //present inside the persistent storage
  //as soon as the component is being created
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) {}

  login(model: any) {
    //here we are storing the user name and token in persistent storage
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );
  }

  //this method is being called from app.component.ts
  setCurrentUser(user: User) {
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((user) => {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );
  }
}
