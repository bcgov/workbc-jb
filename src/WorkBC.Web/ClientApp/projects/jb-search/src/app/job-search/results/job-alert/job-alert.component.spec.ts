import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { HttpClientModule } from '@angular/common/http';
import { JobAlertComponent } from './job-alert.component';
import { GlobalService, SystemSettingsService } from '../../../../../../jb-lib/src/public-api';

describe('JobAlertComponent', () => {
  let component: JobAlertComponent;
  let fixture: ComponentFixture<JobAlertComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [JobAlertComponent],
      imports: [
        HttpClientModule,
      ],
      providers: [
        NgbActiveModal,
        { provide: SystemSettingsService, useValue: {} },
        { provide: GlobalService, useValue: { getJbAccountUrl: () => '' } },
        { provide: ToastrService, useValue: {} }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JobAlertComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
