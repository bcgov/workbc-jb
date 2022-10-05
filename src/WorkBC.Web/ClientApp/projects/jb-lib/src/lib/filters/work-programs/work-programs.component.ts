import { Component, ElementRef } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-work-programs',
  templateUrl: './work-programs.component.html',
  styleUrls: ['./work-programs.component.scss'],
  host: {
    '(document:click)': 'onClick($event)',
  },
  animations: [
    trigger('smoothCollapse', [
      state('initial', style({
        height: '0',
        overflow: 'hidden',
        opacity: '0'
      })),
      state('final', style({
        overflow: 'hidden',
        opacity: '1'
      })),
      transition('initial=>final', animate('200ms')),
      transition('final=>initial', animate('200ms'))
    ]),
  ]
})
export class WorkProgramsComponent {

  CareerFocusYouthEmploymentStrategy = false;
  DigitalSkillsForYouth = false;
  InternationalYouthInternship = false;
  SkillsLinkYouthEmploymentStrategy = false;
  StudentWorkIntegratedLearningProgram = false;
  YoungCanadaWorks = false;
  AllGovernmentSponsoredJobs = false;

  constructor(private dropdown: NgbDropdown, private _eref: ElementRef) { }

  onClick(event) {
    // if user clicks off the dropdown or opens another one, close this one
    if (!this._eref.nativeElement.contains(event.target))
      this.dropdown.close();
  }

}
