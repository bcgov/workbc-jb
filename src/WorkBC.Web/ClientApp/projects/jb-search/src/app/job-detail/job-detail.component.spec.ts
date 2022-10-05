import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { DataService, SystemSettingsService } from '../../../../jb-lib/src/public-api';
import { AppRoutingModule } from '../app-routing.module';

import { JobDetailComponent } from './job-detail.component';

describe('JobDetailComponent', () => {
  let component: JobDetailComponent;
  let fixture: ComponentFixture<JobDetailComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [
        AppRoutingModule
      ],
      declarations: [JobDetailComponent],
      providers: [
        { provide: DataService, useValue: {} },
        { provide: SystemSettingsService, useValue: {} },
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JobDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
