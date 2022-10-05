import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SavedJobsComponent } from './saved-jobs.component';

describe('SavedJobsComponent', () => {
  let component: SavedJobsComponent;
  let fixture: ComponentFixture<SavedJobsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SavedJobsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SavedJobsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
