import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { JbLibModule, JwtInterceptor } from '../../../jb-lib/src/public-api';
import { AppRoutingModule } from './app-routing.module';
import { JobSearchModule } from './job-search/job-search.module';
import { JobDetailModule } from './job-detail/job-detail.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ObserversModule } from '@angular/cdk/observers';

@NgModule({
  declarations: [AppComponent],
  imports: [
    HttpClientModule,
    AppRoutingModule,
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    NgbModule,
    JbLibModule,
    JobSearchModule,
    JobDetailModule,
    ObserversModule
  ],
  providers: [
      { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
  ],
  entryComponents: [
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
