import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { JobSearchComponent } from './job-search/job-search.component';
import { RedirectComponent } from './job-search/redirect/redirect.component';
import { JobDetailComponent } from './job-detail/job-detail.component';

const appRoutes: Routes = [
  { path: '', redirectTo: 'job-search', pathMatch: 'full' },
  { path: 'job-search', component: JobSearchComponent },
  { path: 'job-search/r', component: RedirectComponent },
  { path: 'job-details/:id', component: JobDetailComponent }
]

@NgModule({
  imports: [
    RouterModule.forRoot(appRoutes, {
    scrollPositionRestoration: 'enabled',
    useHash: true,
    relativeLinkResolution: 'legacy'
})
  ],
  exports: [
    RouterModule
  ]
})
export class AppRoutingModule { }
