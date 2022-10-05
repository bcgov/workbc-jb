import { Component } from '@angular/core';
import { RouteEventsService } from '../../../jb-lib/src/lib/services/route-events';
import {ObserversModule} from '@angular/cdk/observers';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent {
    title = 'jb-search';
    constructor(private routeEventsService: RouteEventsService) {

    }

    updateSnowplowLinks(mutationRecords: MutationRecord[]): void {
      /* eslint-disable  @typescript-eslint/no-explicit-any */
      if ((window as any).snowplow) {
        (window as any).snowplow('refreshLinkClickTracking');
      }
  }
}
