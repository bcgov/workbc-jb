/*
 * original source: https://coryrylan.com/blog/focus-first-invalid-input-with-angular-forms
 */
import { Directive, HostListener, ElementRef } from '@angular/core';

@Directive({
  selector: '[focusInvalidInput]'
})
export class FocusInvalidInputDirective {

  constructor(private el: ElementRef) { }

  @HostListener('submit')
  onFormSubmit() {
    setTimeout(() => {
      const invalidControl = this.el.nativeElement.querySelector('.input-danger');

      if(invalidControl) {
        invalidControl.focus();
      }
    }, 100);
  }
}
