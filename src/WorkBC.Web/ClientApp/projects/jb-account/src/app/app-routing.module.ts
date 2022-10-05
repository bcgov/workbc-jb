import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AccountDashboardComponent } from './account/account-dashboard/account-dashboard.component';
import { RecommendedJobsComponent } from './account/recommended-jobs/recommended-jobs.component';
import { PersonalSettingsComponent } from './account/personal-settings/personal-settings.component';
import { CreateComponent } from './account/job-alerts/create/create.component';
import { ListComponent } from './account/job-alerts/list/list.component';
import { SavedJobsComponent } from './account/saved-jobs/saved-jobs.component';
import { SavedCareerProfilesComponent } from './account/saved-career-profiles/saved-career-profiles.component';
import { SavedIndustryProfilesComponent } from './account/saved-industry-profiles/saved-industry-profiles.component';

import { AuthGuard } from './guards/auth.guard';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { LandingComponent } from './register/landing/landing.component';
import { ConfirmEmailComponent } from './register/confirm-email/confirm-email.component';
import { PasswordResetComponent } from './login/password-reset/password-reset.component';
import { ImpersonateComponent } from './login/impersonate/impersonate.component';
import { CanDeactivateGuard } from '../../../jb-lib/src/public-api';

const appRoutes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'dashboard',
    component: AccountDashboardComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'recommended-jobs',
    component: RecommendedJobsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'personal-settings',
    component: PersonalSettingsComponent,
    canActivate: [AuthGuard],
    canDeactivate: [CanDeactivateGuard]
  },
  {
    path: 'job-alerts',
    component: ListComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'job-alerts/create',
    component: CreateComponent,
    canActivate: [AuthGuard],
    canDeactivate: [CanDeactivateGuard]
  },
  {
    path: 'job-alerts/edit/:id',
    component: CreateComponent,
    canActivate: [AuthGuard],
    canDeactivate: [CanDeactivateGuard]
  },
  {
    path: 'saved-jobs',
    component: SavedJobsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'saved-career-profiles',
    component: SavedCareerProfilesComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'saved-industry-profiles',
    component: SavedIndustryProfilesComponent,
    canActivate: [AuthGuard]
  },
  { path: 'login', component: LoginComponent },
  {
    path: 'register',
    component: RegisterComponent,
    canDeactivate: [CanDeactivateGuard]
  },
  { path: 'register/landing', component: LandingComponent },
  {
    path: 'confirm-email/:userId/:code',
    component: ConfirmEmailComponent
  },
  {
    path: 'password-reset/:email/:code',
    component: PasswordResetComponent
  },
  {
    path: 'impersonate/:token/:adminUrl',
    component: ImpersonateComponent
  },
  { path: '**',
    redirectTo: 'dashboard',
    pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(appRoutes, {
    useHash: true,
    scrollPositionRestoration: 'enabled',
    anchorScrolling: 'enabled',
    relativeLinkResolution: 'legacy'
})],
  exports: [RouterModule]
})
export class AppRoutingModule {}
