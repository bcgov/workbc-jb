import { Component, OnInit } from '@angular/core';
import {
  StorageService,
  AuthenticationService,  SystemSettingsService,
  JbDashBoard
} from '../../../../../jb-lib/src/public-api';
import { MatDialog } from '@angular/material/dialog';
import { PersonalSettingsDialogComponent } from '../personal-settings-dialog/personal-settings-dialog.component';

@Component({
  selector: 'app-account-dashboard',
  templateUrl: './account-dashboard.component.html',
  styleUrls: ['./account-dashboard.component.scss']
})
export class AccountDashboardComponent implements OnInit {

  private readonly showNewMessage = 'showNewMsg';
  private readonly showNotice1 = 'showNotice1';
  private readonly notice1Hash = 'notice1Hash';
  private readonly showNotice2 = 'showNotice2';
  private readonly notice2Hash = 'notice2Hash';

  constructor(
    private dialog: MatDialog,
    private authenticationService: AuthenticationService,
    private settings: SystemSettingsService,
    private storageService: StorageService
  ) {}

  ngOnInit(): void {
    window.scroll(0, 0);

    const justLoggedIn = this.storageService.getLocalStorageItem(
      this.storageService.justLoggedInkey
    );
    if (justLoggedIn) {
      const dialogRef = this.dialog.open(PersonalSettingsDialogComponent, {
        width: '0%', 
        maxWidth: 800,
        disableClose: true,
        panelClass: 'personal-settings-dialog-modal'
      });

      dialogRef.afterClosed().subscribe(result => {
        this.storageService.removeLocalStorageItem(
          this.storageService.justLoggedInkey
        );

        if (!result) {
          if (this.adminUrl !== '') {
            const logoutRedirect: string = this.adminUrl;
            this.authenticationService.logout();
            this.storageService.removeItem('adminUrl');
            window.location.href = logoutRedirect;
          } else {
            this.authenticationService.logout();
            location.reload(true);
          }
        }
      });
    }
  }

  get introText(): string {
    return this.jbDashBoard.introText;
  }

  get jobsDescription(): string {
    return this.jbDashBoard.jobsDescription;
  }

  get careersDescription(): string {
    return this.jbDashBoard.careersDescription;
  }

  get accountDescription(): string {
    return this.jbDashBoard.accountDescription;
  }

  get resource1Title(): string {
    return this.jbDashBoard.resource1Title;
  }

  get resource1Body(): string {
    return this.jbDashBoard.resource1Body;
  }

  get resource1Url(): string {
    return this.jbDashBoard.resource1Url;
  }

  get resource2Title(): string {
    return this.jbDashBoard.resource2Title;
  }

  get resource2Body(): string {
    return this.jbDashBoard.resource2Body;
  }

  get resource2Url(): string {
    return this.jbDashBoard.resource2Url;
  }

  get resource3Title(): string {
    return this.jbDashBoard.resource3Title;
  }

  get resource3Body(): string {
    return this.jbDashBoard.resource3Body;
  }

  get resource3Url(): string {
    return this.jbDashBoard.resource3Url;
  }

  closeNotification1(): void {
    this.showNotification1 = false;
  }

  get showNotification1(): boolean {
    let result = this.jbDashBoard.notification1Enabled;
    if (result) {
      result = this.getResult(
        result,
        this.notice1Hash,
        this.jbDashBoard.notification1Hash,
        this.showNotice1
      );
    }
    return result; 
  }
  set showNotification1(value: boolean) {
    this.storageService.setLocalStorageItem(this.showNotice1, value.toString());
  }

  private getResult(
    showNotification: boolean,
    noticeHash: string,
    notificationBodyHash: string,
    showNoticeKey: string
  ): boolean {
    const hash = this.storageService.getLocalStorageItem(noticeHash);
    if (!hash) {
      this.storageService.setLocalStorageItem(
        noticeHash,
        notificationBodyHash);
    } else {
      const showNotice = this.storageService.getLocalStorageItem(showNoticeKey);
      const isDifferent = hash !== notificationBodyHash;

      if (isDifferent) {
        this.storageService.setLocalStorageItem(
          noticeHash,
          notificationBodyHash);

        if (showNotice) {
          this.storageService.removeLocalStorageItem(showNoticeKey);
        }
      }

      showNotification = isDifferent || !showNotice || (showNotice.toLowerCase() !== 'false');
    }
    return showNotification;
  }

  clearRecommendedJobsState() {
    sessionStorage.removeItem('recommendedJobsState');
  }

  clearSavedJobsPageState() {
    sessionStorage.removeItem('savedJobsSortOrder');
  }

  get showNotification2(): boolean {
    let result = this.jbDashBoard.notification2Enabled;
    if (result) {
      result = this.getResult(
        result,
        this.notice2Hash,
        this.jbDashBoard.notification2Hash,
        this.showNotice2
      );
    }
    return result;
  }
  set showNotification2(value: boolean) {
    this.storageService.setLocalStorageItem(this.showNotice2, value.toString());
  }

  get showNewAccountMessage(): boolean {
    const result = this.storageService.getLocalStorageItem(this.showNewMessage);
    return !result || result.toLowerCase() === 'true';
  }
  set showNewAccountMessage(value: boolean) {
    this.storageService.setLocalStorageItem(this.showNewMessage, value.toString());
  }

  get totalSavedJobs(): number {
    return this.storageService.totalSavedJobs;
  }

  get jbDashBoard(): JbDashBoard {
    return this.settings.jbAccount.dashboard;
  }

  get adminUrl(): string {
    const adminUrlBase: string = this.storageService.getItem('adminUrl');

    if (!adminUrlBase || !adminUrlBase.length) {
      return '';
    }

    const id: string = this.authenticationService.currentUser.id;
    return adminUrlBase + id;
  }
}
