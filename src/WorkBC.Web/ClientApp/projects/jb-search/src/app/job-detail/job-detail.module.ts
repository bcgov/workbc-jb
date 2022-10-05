import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { JobDetailComponent } from './job-detail.component';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { JbLibModule } from '../../../../jb-lib/src/public-api';
//import { DirectivesModule } from '../directives/directives.module';
import { RouterModule } from '@angular/router';
import { FormatPhonePipe } from './../pipes/format-phone.pipe';
import { ShareButtonsModule } from 'ngx-sharebuttons/buttons';
import { SocialSharingComponent } from './social-sharing/social-sharing.component'

@NgModule({
  declarations: [
    JobDetailComponent,
    FormatPhonePipe,
    SocialSharingComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    NgbModule,
    JbLibModule,
    RouterModule,
    ShareButtonsModule,
    //DirectivesModule    
  ]
})
export class JobDetailModule { }
