import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { UserService } from '../../services/user.service';
import { SystemSettingsService } from '../../../../../jb-lib/src/public-api';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss']
})
export class ConfirmEmailComponent implements OnInit {
  loading = false;
  isEmailAlreadyConfirmed = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private settings: SystemSettingsService,
    private userService: UserService
  ) { }

  ngOnInit(): void {
    this.loading = true;
    this.route.paramMap.pipe(
      switchMap((params: ParamMap) => {
        const userId = params.get('userId');
        const code = params.get('code');
        return this.userService.confirmEmail(userId, code);
      })
    ).subscribe((result: { isEmailAlreadyConfirmed: boolean }) => {
      this.isEmailAlreadyConfirmed = result.isEmailAlreadyConfirmed;

      if (!this.isEmailAlreadyConfirmed) {
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 5000);
      }

      this.loading = false;
    });
  }

  get activationTitle(): string {
    return this.settings.jbAccount.registration.activationTitle;
  }

  get activationBody(): string {
    return this.settings.jbAccount.registration.activationBody;
  }
}
