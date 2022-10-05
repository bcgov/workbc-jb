import { Component, OnInit, ViewChild } from '@angular/core';
import {
  CareerProfileService,
  CareerProfile,
} from '../../services/career-profile.service';
import { MatDialog } from '@angular/material/dialog';
import {
  SimpleDialogComponent,
  PaginationModel,
  PaginationComponent,
  SystemSettingsService,
} from '../../../../../jb-lib/src/public-api';
import { ToastrService } from 'ngx-toastr';
import { SortableComponent } from '../../models/base-component.model';

@Component({
  selector: 'app-saved-career-profiles',
  templateUrl: './saved-career-profiles.component.html',
  styleUrls: ['./saved-career-profiles.component.scss'],
})
export class SavedCareerProfilesComponent extends SortableComponent implements OnInit {
  paginationModel = new PaginationModel();
  profiles: Array<CareerProfile> = [];
  allProfiles: CareerProfile[] = [];
  loading = false;

  @ViewChild('pagination')
  paginationElement: PaginationComponent;

  constructor(
    private careerProfileService: CareerProfileService,
    private toastr: ToastrService,
    private settings: SystemSettingsService,
    private dialog: MatDialog
  ) {
      super();
  }

  ngOnInit(): void {
    this.loading = true;
    this.careerProfileService.getCareerProfiles().subscribe(
      (result) => {
        if (result) {
          this.allProfiles = result;
          this.profiles = [...this.allProfiles];
          this.paginationElement.setResultCount(this.allProfiles.length);
          this.loading = false;
        }
      },
      (error) => {
        console.error(error);
        this.toastr.error('Internal Server Error');
      }
    );
  }

  get callToAction1BodyText(): string {
    return this.settings.jbAccount.careerProfiles.callToAction1BodyText;
  }

  get callToAction1LinkText(): string {
    return this.settings.jbAccount.careerProfiles.callToAction1LinkText;
  }

  get callToAction1LinkUrl(): string {
    return this.settings.jbAccount.careerProfiles.callToAction1LinkUrl;
  }

  get callToAction2BodyText(): string {
    return this.settings.jbAccount.careerProfiles.callToAction2BodyText;
  }

  get callToAction2LinkText(): string {
    return this.settings.jbAccount.careerProfiles.callToAction2LinkText;
  }

  get callToAction2LinkUrl(): string {
    return this.settings.jbAccount.careerProfiles.callToAction2LinkUrl;
  }

  get hasProfiles(): boolean {
    return this.allProfiles.length > 0;
  }

  delete(profileId: number): void {
    event.preventDefault();
    this.dialog
      .open(SimpleDialogComponent, {
        data: {
          title: 'DELETE CAREER PROFILE',
          btnLabel: 'Delete',
          content: 'Are you sure you want to delete this career profile?',
        },
        width: '85%',
        maxWidth: 500,
      })
      .afterClosed()
      .subscribe((result: boolean) => {
        if (result) {
          this.careerProfileService
            .deleteCareerProfile(profileId)
            .subscribe(() => {
              this.allProfiles = this.allProfiles.filter(
                (x) => x.id !== profileId
              );

              this.paginationElement.setResultCount(
                this.allProfiles.length,
                this.profiles.length - 1 === 0
                  ? this.paginationModel.currentPage - 1
                  : this.paginationModel.currentPage
              );

              this.setPageResult();
            });
        }
      });
  }

  private setPageResult(): void {
    const start =
      (this.paginationModel.currentPage - 1) *
      this.paginationModel.resultsPerPage;
    const remaining = this.allProfiles.length - start;
    const end =
      start +
      (this.paginationModel.resultsPerPage > remaining
        ? remaining
        : +this.paginationModel.resultsPerPage);
    this.profiles = this.allProfiles.slice(start, end);
  }

  onCurrentPageChanged(): void {
    this.setPageResult();
  }

  sortByChange(): void {
    if (this.paginationModel.currentPage > 1) {
      this.paginationModel.currentPage = 1;
    }
    this.sortData(this.allProfiles);
    this.setPageResult();
  }
}
