import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  //url is stored as environment variable
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getMembers() {
    //Member is the interface we defined under model/member.ts
    console.log(this.baseUrl);
    return this.http.get<Member[]>(this.baseUrl + 'users');
  }

  getMember(username: string) {
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  //to pass token in the http header
  // getHttpOptions() {
  //   //getting token from localstorage
  //   const userString = localStorage.getItem('user');
  //   if (!userString) return;
  //   const user = JSON.parse(userString); //parses a string to json
  //   //passing the token in the header
  //   return {
  //     headers: new HttpHeaders({
  //       Authorization: 'Bearer ' + user.token,
  //     }),
  //   };
  // }
}
