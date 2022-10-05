import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { JobSeekerComponent } from './job-seeker.component';

describe('JobSeekerComponent', () => {
  let component: JobSeekerComponent;
  let fixture: ComponentFixture<JobSeekerComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ JobSeekerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JobSeekerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
