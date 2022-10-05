import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { NgbModule, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { ToastrModule } from 'ngx-toastr';

import { StickyPopoverDirective } from './directives/stickyPopovers/sticky-popover.directive';
import { FocusInvalidInputDirective } from './directives/focusInvalidInput/focus-invalid-input.directive';
import { DateComponent } from './filters/date/date.component';
import { EducationComponent } from './filters/education/education.component';
import { FiltersComponent } from './filters/filters.component';
import { IndustryComponent } from './filters/industry/industry.component';
import { JobTypeComponent } from './filters/job-type/job-type.component';
import { LocationComponent } from './filters/location/location.component';
import { MoreComponent } from './filters/more/more.component';
import { SalaryComponent } from './filters/salary/salary.component';
import { NgbDateCustomParserFormatter } from '../lib/filters/date/date-formatter.service';
import { JbALinkComponent } from './components/jb-a-link/jb-a-link.component';
import { ItemComponent } from './components/item/item.component';
import { ExternalJobModalComponent } from './components/external-job-modal/external-job-modal.component';
import { XmlJobModalComponent } from './components/xml-job-modal/xml-job-modal.component';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';
import { PaginationComponent } from './components/pagination/pagination.component';
import { RouteEventsService } from './services/route-events';
import { SimpleDialogComponent } from './components/simple-dialog/simple-dialog.component';
import { SvgIconComponent } from './components/icon/svgIcon.component';
import { SavedJobNoteComponent } from './components/saved-job-note/saved-job-note.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ErrorInterceptor } from './interceptors/error-interceptor';
import { CacheInterceptor } from './interceptors/cache-interceptor';
import { SearchByComponent } from './components/search-by/search-by.component';
import { SearchCriteriesComponent } from './components/search-criteries/search-criteries.component';
import { RecommendationFilterComponent } from './components/recommendation-filter/recommendation-filter.component';
import { ImpersonationHeaderComponent } from './components/impersonation-header/impersonation-header.component';
import { WorkProgramsComponent } from './filters/work-programs/work-programs.component';

@NgModule({
  declarations: [
    MoreComponent,
    DateComponent,
    EducationComponent,
    IndustryComponent,
    SalaryComponent,
    JobTypeComponent,
    LocationComponent,
    FiltersComponent,
    SvgIconComponent,
    StickyPopoverDirective,
    FocusInvalidInputDirective,
    JbALinkComponent,
    ItemComponent,
    ExternalJobModalComponent,
    XmlJobModalComponent,
    PaginationComponent,
    BreadcrumbComponent,
    SimpleDialogComponent,
    SavedJobNoteComponent,
    SearchByComponent,
    SearchCriteriesComponent,
    RecommendationFilterComponent,
    ImpersonationHeaderComponent,
    WorkProgramsComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    RouterModule,
    MatAutocompleteModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    NgbModule,
    ToastrModule.forRoot({
      positionClass: 'toast-top-full-width',
      closeButton: true,
      preventDuplicates: true
    })
    ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: CacheInterceptor, multi: true },
    { provide: NgbDateParserFormatter, useClass: NgbDateCustomParserFormatter },
    RouteEventsService
  ],
  exports: [
    MatAutocompleteModule,
    MatDialogModule,
    FiltersComponent,
    SvgIconComponent,
    StickyPopoverDirective,
    FocusInvalidInputDirective,
    JbALinkComponent,
    ItemComponent,
    ExternalJobModalComponent,
    XmlJobModalComponent,
    BreadcrumbComponent,
    PaginationComponent,
    SimpleDialogComponent,
    SearchByComponent,
    SearchCriteriesComponent,
    RecommendationFilterComponent,
    ImpersonationHeaderComponent,
    WorkProgramsComponent
  ],
  entryComponents: [
  ]
})
export class JbLibModule {}
