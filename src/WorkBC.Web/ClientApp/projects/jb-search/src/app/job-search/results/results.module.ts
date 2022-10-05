import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { JbLibModule } from '../../../../../jb-lib/src/public-api';
import { FormsModule } from '@angular/forms';

import { JobAlertComponent } from './job-alert/job-alert.component';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [
    JobAlertComponent,
  ],
  imports: [
    BrowserModule,
    NgbModule,
    JbLibModule,
    FormsModule,
    RouterModule
  ],
  exports: [
  ],
  providers: [],
  entryComponents: [
    JobAlertComponent
  ],
  bootstrap: [
  ] 
})
export class ResultsModule { }
