import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(
    public accountService: AccountService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}

  //custom observable has to be unsubscribed
  //memory leak issue
  // getCurrentUser() {
  //   this.accountService.currentUser$.subscribe({
  //     next: (user) => (this.loggedIn = !!user), // converts object into boolean
  //     error: (error) => console.log(error),
  //   });
  // }

  //here it is not necessary to unsubscribe cause
  //as soon as the hhtp method ends
  //automatic unsubscription takes place
  login() {
    this.accountService.login(this.model).subscribe({
      next: (_) => this.router.navigateByUrl('/members'),
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
