import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AccountComponent } from './account.component';
import { AccountDashboardComponent } from './account-dashboard/account-dashboard.component';
import { RecommendedJobsComponent } from './recommended-jobs/recommended-jobs.component';
import { PersonalSettingsComponent } from './personal-settings/personal-settings.component';
import { SavedJobsComponent } from './saved-jobs/saved-jobs.component';
import { ListComponent } from './job-alerts/list/list.component';
import { CreateComponent } from './job-alerts/create/create.component';
import { SavedCareerProfilesComponent } from './saved-career-profiles/saved-career-profiles.component';
import { SavedIndustryProfilesComponent } from './saved-industry-profiles/saved-industry-profiles.component';
import { JbLibModule } from '../../../../jb-lib/src/public-api';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDialogModule } from '@angular/material/dialog';
import { RouterModule } from '@angular/router';
import { PersonalSettingsDialogComponent } from './personal-settings-dialog/personal-settings-dialog.component';

@NgModule({
  declarations: [
    AccountComponent,
    AccountDashboardComponent,
    RecommendedJobsComponent,
    PersonalSettingsComponent,
    SavedJobsComponent,
    ListComponent,
    CreateComponent,
    SavedCareerProfilesComponent,
    SavedIndustryProfilesComponent,
    PersonalSettingsDialogComponent
  ],
  imports: [
    CommonModule,
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    RouterModule,
    NgbModule,
    JbLibModule,
    BrowserAnimationsModule,
    MatAutocompleteModule,
    MatDialogModule
  ],
  exports: [
    AccountComponent,   
    AccountDashboardComponent,
    RecommendedJobsComponent,
    PersonalSettingsComponent,
    SavedJobsComponent,
    ListComponent,
    CreateComponent,
    SavedCareerProfilesComponent,
    SavedIndustryProfilesComponent
  ],
  entryComponents: [
  ]
})
export class AccountModule { }
