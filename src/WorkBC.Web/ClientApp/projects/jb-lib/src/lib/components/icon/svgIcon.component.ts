import { Component, Input } from '@angular/core';
import { GlobalService } from '../../services/global.service';

@Component({
  selector: 'lib-svg-icon',
  styleUrls: ['./svgIcon.component.scss'],
  template: '<img src="{{globalService.getApiBaseUrl()}}assets/icons/{{icon}}.svg" attr.alt={{altText}} class="icon" attr.width={{width}} attr.height={{height}} [style.max-height]="height">'
})
export class SvgIconComponent {

  constructor(public globalService: GlobalService) {}

  @Input() icon: string;
  @Input() width: string;
  @Input() height: string;
  @Input() direction: string;
  @Input() alt: string;

  get altText(): string {
    return this.alt ? this.alt : '';
  }

}
