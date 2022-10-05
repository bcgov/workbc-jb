import { Component, OnInit } from '@angular/core';
import { Router, NavigationStart, NavigationEnd, Event } from '@angular/router';
import { ViewportScroller } from '@angular/common';
import { filter, scan, observeOn } from 'rxjs/operators';
import { asyncScheduler } from 'rxjs';

interface ScrollPositionRestore {
  event: Event;
  positions: { [eventId: number]: number };
  trigger: 'imperative' | 'popstate' | 'hashchange';
  idToRestore: number;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'jb-account';

  constructor(
    private router: Router,
    private viewportScroller: ViewportScroller
  ) {}

  ngOnInit(): void {
    this.setRouterEvents();
  }

  private setRouterEvents(): void {
    this.router.events
      .pipe(
        filter(
          (event) =>
            event instanceof NavigationStart || event instanceof NavigationEnd
        ),
        scan<Event, ScrollPositionRestore>((acc, event) => {
          const result = {
            event,
            positions: {
              ...acc.positions,
              ...(event instanceof NavigationStart
                ? {
                    [event.id]: this.viewportScroller.getScrollPosition()[1],
                  }
                : {}),
            },
            trigger:
              event instanceof NavigationStart
                ? event.navigationTrigger
                : acc.trigger,
            idToRestore:
              (event instanceof NavigationStart &&
                event.restoredState &&
                event.restoredState.navigationId + 1) ||
              acc.idToRestore,
          };
          return result;
        }),
        filter(
          (x) => x.event instanceof NavigationEnd && !!x.trigger
          // ({ event, trigger }) => event instanceof NavigationEnd && !!trigger
        ),
        observeOn(asyncScheduler)
      )
      .subscribe(({ trigger, positions, idToRestore }) => {
        if (trigger === 'popstate' && idToRestore && positions[idToRestore]) {
          setTimeout(() => {
            this.viewportScroller.scrollToPosition([0, positions[idToRestore]]);
          });
        }
      });
  }

  updateSnowplowLinks(mutationRecords: MutationRecord[]): void {
      /* eslint-disable  @typescript-eslint/no-explicit-any */
      if ((window as any).snowplow) {
        (window as any).snowplow('refreshLinkClickTracking');
      }
  }
}
