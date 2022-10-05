import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { map } from 'rxjs/operators';
import {
  IndustryProfile,
  IndustryProfileService,
} from '../../services/industry-profile.service';
import { SimpleDialogComponent, PaginationModel, PaginationComponent, SystemSettingsService } from '../../../../../jb-lib/src/public-api';
import { ToastrService } from 'ngx-toastr';
import { SortableComponent } from '../../models/base-component.model';

@Component({
  selector: 'app-saved-industry-profiles',
  templateUrl: './saved-industry-profiles.component.html',
  styleUrls: ['./saved-industry-profiles.component.scss'],
})
export class SavedIndustryProfilesComponent extends SortableComponent implements OnInit {
  loading = false;
  paginationModel = new PaginationModel();

  @ViewChild('pagination')
  paginationElement: PaginationComponent;

  allProfiles: Array<IndustryProfile> = [
    //  {
    //  id: 1,
    //  title: "Industry Profle Title",
    //  count: 27
    //},
    //{
    //  id: 2,
    //  title: "Professional, Scientific, and Technical Services",
    //  count: 0
    //},
    //{
    //  id: 3,
    //  title: "Industry Profle Title",
    //  count: 27
    //},
    //{
    //  id: 4,
    //  title: "Industry Profle Title",
    //  count: 27
    //},
  ];

  constructor(
    private industryProfileService: IndustryProfileService,
    private toastr: ToastrService,
    private settings: SystemSettingsService,
    private dialog: MatDialog
  ) {
      super();
  }

  ngOnInit(): void {
    this.loading = true;
    this.industryProfileService
      .getIndustryProfiles()
      .pipe(
        map((x) =>
          x.map((y) => ({ ...y, profileId: this.getProfileId(y.title) }))
        )
      )
      .subscribe(
        (result) => {
          this.allProfiles = result;
          this.paginationElement.setResultCount(this.allProfiles.length);
          this.loading = false;
        },
        (error) => {
          console.error(error);
          this.toastr.error('Internal Server Error');
        }
      );
  }

  get callToAction1BodyText(): string {
    return this.settings.jbAccount.industryProfiles.callToAction1BodyText;
  }

  get callToAction1LinkText(): string {
    return this.settings.jbAccount.industryProfiles.callToAction1LinkText;
  }

  get callToAction1LinkUrl(): string {
    return this.settings.jbAccount.industryProfiles.callToAction1LinkUrl;
  }

  get callToAction2BodyText(): string {
    return this.settings.jbAccount.industryProfiles.callToAction2BodyText;
  }

  get callToAction2LinkText(): string {
    return this.settings.jbAccount.industryProfiles.callToAction2LinkText;
  }

  get callToAction2LinkUrl(): string {
    return this.settings.jbAccount.industryProfiles.callToAction2LinkUrl;
  }

  get profileCount(): number {
    return this.allProfiles.length;
  }

  get profiles(): IndustryProfile[] {
    const start = (this.paginationModel.currentPage - 1) * this.paginationModel.resultsPerPage;

    const remaining = this.allProfiles.length - start;

    const end = start +
      (this.paginationModel.resultsPerPage > remaining
        ? remaining
        : +this.paginationModel.resultsPerPage);

    return this.allProfiles.slice(start, end);
  }

  delete(profileId: number): void {
    event.preventDefault();
    this.dialog
      .open(SimpleDialogComponent, {
        data: {
          title: 'DELETE INDUSTRY PROFILE',
          btnLabel: 'Delete',
          content: 'Are you sure you want to delete this industry profile?',
        },
        width: '85%',
        maxWidth: 500,
      })
      .afterClosed()
      .subscribe((result: boolean) => {
        if (result) {
          this.industryProfileService
            .deleteIndustryProfile(profileId)
            .subscribe(() => {
              this.allProfiles = this.allProfiles.filter(
                (x) => x.id !== profileId
              );

              this.paginationElement.setResultCount(
                this.allProfiles.length,
                this.profiles.length === 0
                  ? this.paginationModel.currentPage - 1
                  : this.paginationModel.currentPage
              );
            });
        }
      });
  }

  sortByChange(): void {
    if (this.paginationModel.currentPage > 1) {
      this.paginationModel.currentPage = 1;
    }
    this.sortData(this.allProfiles);
  }

  private getProfileId(title: string): string {
    return title ? title.replace(/, /g, '-').replace(/ /g, '-') : '';
  }
}
