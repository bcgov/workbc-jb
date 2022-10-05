import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastrModule } from 'ngx-toastr';

import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { AppRoutingModule } from './app-routing.module';
import { AccountModule } from './account/account.module';
import { JbLibModule, JwtInterceptor } from '../../../jb-lib/src/public-api';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { LandingComponent } from './register/landing/landing.component';
import { JobSeekerComponent } from './register/job-seeker/job-seeker.component';
import { ThankYouComponent } from './register/thank-you/thank-you.component';
import { ConfirmEmailComponent } from './register/confirm-email/confirm-email.component';
import { JbCheckboxComponent } from './shared/jb-checkbox/jb-checkbox.component';
import { ResetPasswordDialogComponent } from './login/reset-password-dialog/reset-password-dialog.component';
import { PasswordResetComponent } from './login/password-reset/password-reset.component';
import { TermsOfUseComponent } from './register/terms-of-use/terms-of-use.component';
import { ImpersonateComponent } from './login/impersonate/impersonate.component';
import { ObserversModule } from '@angular/cdk/observers';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    LandingComponent,
    JobSeekerComponent,
    ThankYouComponent,
    ConfirmEmailComponent,
    JbCheckboxComponent,
    ResetPasswordDialogComponent,
    PasswordResetComponent,
    TermsOfUseComponent,
    ImpersonateComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgbModule,
    MatAutocompleteModule,
      ToastrModule.forRoot({
          positionClass: 'toast-top-full-width',
          closeButton: true,
          preventDuplicates: true
      }),
    JbLibModule,
    AppRoutingModule,
    AccountModule,
    ObserversModule 
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
  ],
  entryComponents: [
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
