import { Component, ViewChild } from '@angular/core';
import { Router } from '@angular/router';

import { BaseComponent } from '../models/base-component.model';
import { AuthenticationService } from '../../../../jb-lib/src/public-api';
import { MatDialog } from '@angular/material/dialog';
import { JobSeekerComponent } from './job-seeker/job-seeker.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent extends BaseComponent {
  //showAccountSelect: boolean = true;
  //showEmployer: boolean = false;
  showJobSeeker = true;
  showThankYou = false;

  email: string;

  @ViewChild(JobSeekerComponent)
  private jobSeekerComponent: JobSeekerComponent;

  constructor(
    private router: Router,
    protected dialog: MatDialog,
    private authenticationService: AuthenticationService
  ) {
    super(dialog);

    // redirect to home if already logged in
    if (this.authenticationService.currentUser) {
      this.router.navigate(['/']);
    }
  }

  onRegistered(email: string): void {
    this.email = email;
    this.showJobSeeker = false;
    this.showThankYou = true;
  }

}
