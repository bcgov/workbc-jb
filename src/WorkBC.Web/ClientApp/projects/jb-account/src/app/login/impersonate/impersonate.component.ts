import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { StorageService, AuthenticationService } from '../../../../../jb-lib/src/public-api';

@Component({
  selector: 'app-impersonate',
  templateUrl: '../../../../../jb-lib/src/lib/components/redirect/redirect.shared.html',
  styleUrls: ['../../../../../jb-lib/src/lib/components/redirect/redirect.shared.scss']
})
export class ImpersonateComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private storageService: StorageService,
    private authenticationService: AuthenticationService,
    private toastr: ToastrService
  ) { }
    
  ngOnInit() {
    // logout if already logged in
    this.authenticationService.logout();

    // don't stay logged in
    this.storageService.staySignedIn = false;

    this.route.paramMap.subscribe(
      (params: ParamMap) => {
        const token: string = params.get('token');

        this.authenticationService
          .impersonate(token)
          .subscribe(
            () => {
              // save the admin URL
              let adminUrl: string = params.get('adminUrl');
              if (adminUrl && adminUrl.length) {
                adminUrl = adminUrl.replace('%3F', '?')
              }
              this.storageService.setItem('adminUrl', adminUrl)

              // go to the dashboard if login is successul
              this.router.navigateByUrl('/dashboard');
            },
            error => {
              // go to the login page if there is an error
              this.toastr.error(error);
              this.router.navigateByUrl('/login');
            }
          );

      }
    );

  }

}
