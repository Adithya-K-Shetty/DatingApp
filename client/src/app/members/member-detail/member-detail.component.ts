import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit {
  member: Member | undefined;
  //creates a activated route to this component
  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute //provides info on currently activated route
  ) {}

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    //paramMap contains an array of route parameter
    const username = this.route.snapshot.paramMap.get('username');
    if (!username) return;
    this.memberService.getMember(username).subscribe({
      next: (member) => (this.member = member),
    });
  }
}
