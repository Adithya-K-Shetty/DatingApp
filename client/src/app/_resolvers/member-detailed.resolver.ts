import { Injectable } from '@angular/core';
import {
  Router,
  Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot,
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { MembersService } from '../_services/members.service';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root', //this is intialized when the app first starts
})
export class MemberDetailedResolver implements Resolve<Member> {
  constructor(private memberService: MembersService) {}
  resolve(route: ActivatedRouteSnapshot): Observable<Member> {
    //paramMap contains an array of route parameter
    //snapshot refers to state of route in a particular time
    return this.memberService.getMember(route.paramMap.get('username')!);
  }
}
