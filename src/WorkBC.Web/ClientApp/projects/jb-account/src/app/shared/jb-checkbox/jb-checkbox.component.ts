import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-jb-checkbox',
  templateUrl: './jb-checkbox.component.html',
    styleUrls: ['../../register/job-seeker/job-seeker.component.scss', './jb-checkbox.component.css']
})
export class JbCheckboxComponent {

    @Input() inputIdOrName: string;
    @Input() checkboxLabel: string;
    //@Input() registrationFormGroup: FormGroup;
    //@Input() formControlName: FormControl;

}
