import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { RecommendedJobsComponent } from './recommended-jobs.component';

describe('RecommendedJobsComponent', () => {
  let component: RecommendedJobsComponent;
  let fixture: ComponentFixture<RecommendedJobsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ RecommendedJobsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RecommendedJobsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
