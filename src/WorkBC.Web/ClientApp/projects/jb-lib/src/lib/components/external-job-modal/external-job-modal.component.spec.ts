import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ExternalJobModalComponent } from './external-job-modal.component';

describe('ExternalJobModalComponent', () => {
  let component: ExternalJobModalComponent;
  let fixture: ComponentFixture<ExternalJobModalComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ExternalJobModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExternalJobModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
