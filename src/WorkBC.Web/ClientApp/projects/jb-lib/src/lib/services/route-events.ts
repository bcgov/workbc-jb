import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Location } from '@angular/common';
import { Router, NavigationEnd } from '@angular/router';

@Injectable()
export class RouteEventsService {

    // save the previous route
    public previousRoutePath = new BehaviorSubject<string>('');
    public oldUrl = '';
    public newUrl = '';
    public trackingActivated = false;

    constructor(
        private location: Location,
        private router: Router
    )
    {
        // ..initial prvious route will be the current path for now
        this.previousRoutePath.next(this.location.path());

        this.location.onUrlChange((url: string) => {
            if (url.toLowerCase().indexOf('job-search') > -1) {
                this.previousRoutePath.next(url.replace('#', ''));
            }
        });

        this.newUrl = this.router.url;
        router.events.subscribe(event => {
            if (event instanceof NavigationEnd) {
                this.oldUrl = this.newUrl;
                this.newUrl = event.url;
                // track a pageview in snowlplow analytics                
                if ((window as any).snowplow) {
                    if (this.trackingActivated) {
                        (window as any).snowplow('trackPageView');
                    } else {
                        // We don't want to track a pageview to Snowplow analytics until
                        // the 2nd navigation event in the Single Page App. The Kentico base
                        // page already tracks a pageview for the first navigation event
                        this.trackingActivated = true;
                    }
                }
            }
        });
    }
}
