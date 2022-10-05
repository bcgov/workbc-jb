import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { JbCheckboxComponent } from './jb-checkbox.component';

describe('JbCheckboxComponent', () => {
  let component: JbCheckboxComponent;
  let fixture: ComponentFixture<JbCheckboxComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ JbCheckboxComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JbCheckboxComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
