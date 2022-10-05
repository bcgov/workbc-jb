import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { XmlJobModalComponent } from './xml-job-modal.component';

describe('XmlJobModalComponent', () => {
  let component: XmlJobModalComponent;
  let fixture: ComponentFixture<XmlJobModalComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ XmlJobModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(XmlJobModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
