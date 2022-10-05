import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { BaseComponent } from '../../models/base-component.model';
import { AuthenticationService } from '../../../../../jb-lib/src/public-api';

@Component({
    selector: 'app-landing',
    templateUrl: './landing.component.html',
    styleUrls: ['./landing.component.scss']
})
export class LandingComponent extends BaseComponent {

    //showAccountSelect: boolean = true;
    showJobSeeker = true;
    showEmployer = false;
    showThankYou = false;

    email: string;

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService
    ) {
        super();

        // redirect to home if already logged in
        if (this.authenticationService.currentUser) {
            this.router.navigate(['/']);
        }
    }

    onRegistered(email: string) {
        this.email = email;
        this.showJobSeeker = false;
        this.showThankYou = true;
    }

    //showJobSeekerForm() {
    //    this.showAccountSelect = false;
    //    this.showJobSeeker = true;
    //}

    showEmployerForm() {
        //this.showAccountSelect = false;
        //this.showEmployer = true;
    }
}


