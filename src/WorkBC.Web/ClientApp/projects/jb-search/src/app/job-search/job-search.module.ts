import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { JbLibModule } from '../../../../jb-lib/src/public-api';
import { JobSearchComponent } from './job-search.component';
import { ResultsComponent } from './results/results.component';
import { ResultsModule } from './results/results.module';
import { RedirectComponent } from './redirect/redirect.component';

@NgModule({
  declarations: [
    JobSearchComponent,
    ResultsComponent,
    RedirectComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    NgbModule,
    JbLibModule,
    //DirectivesModule,
    //FiltersModule,
    ResultsModule
  ],
  exports: [
    // FiltersComponent,
    ResultsComponent
  ]
})
export class JobSearchModule {}
