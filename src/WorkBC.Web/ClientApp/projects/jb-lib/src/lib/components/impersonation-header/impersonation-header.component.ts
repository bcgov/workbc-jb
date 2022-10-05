import { Component, OnInit } from '@angular/core';
import { StorageService } from '../../services/storage.service';
import { AuthenticationService } from '../../services/authentication.service';
import { User } from '../../models/user.model';

@Component({
  selector: 'lib-impersonation-header',
  templateUrl: './impersonation-header.component.html',
  styleUrls: ['./impersonation-header.component.scss']
})
export class ImpersonationHeaderComponent implements OnInit {

  currentUser: User;

  constructor(
    private storageService: StorageService,
    private authenticationService: AuthenticationService
  ) { }

  ngOnInit() {
    this.currentUser = this.authenticationService.currentUser;
  }

  get isImpersonating() : boolean {
    return this.storageService.isImpersonating;
  }

  get adminUrl() : string {
    const id: string = this.currentUser.id;
    const adminUrl: string = this.storageService.getItem('adminUrl');
    return adminUrl + id;
  }

  get fullName() : string {
    return this.currentUser.firstName + ' ' + this.currentUser.lastName;
  }

  backToAdmin(e) {
    e.preventDefault();
    this.authenticationService.logout();
    const adminUrl: string = this.adminUrl;
    this.storageService.removeItem('adminUrl');
    window.location.href = adminUrl;
  }

}
