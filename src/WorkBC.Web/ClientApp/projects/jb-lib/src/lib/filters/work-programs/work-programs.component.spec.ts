import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { WorkProgramsComponent } from './work-programs.component';

describe('MoreComponent', () => {
  let component: WorkProgramsComponent;
  let fixture: ComponentFixture<WorkProgramsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [WorkProgramsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkProgramsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
