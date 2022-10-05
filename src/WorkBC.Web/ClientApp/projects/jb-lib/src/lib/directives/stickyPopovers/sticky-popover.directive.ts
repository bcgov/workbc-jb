import {
  ElementRef,
  Directive, Input,
  Renderer2,
  Injector,
  ComponentFactoryResolver,
  ViewContainerRef,
  NgZone,
  Inject,
  ChangeDetectorRef,
  ApplicationRef
} from '@angular/core';
import { OnInit, OnDestroy } from '@angular/core';

import { DOCUMENT } from '@angular/common';
import { NgbPopover, NgbPopoverConfig } from '@ng-bootstrap/ng-bootstrap';
@Directive({
  selector: '[stickyPopover]',
  exportAs: 'stickyPopover'
})
export class StickyPopoverDirective extends NgbPopover implements OnInit, OnDestroy {
  @Input() stickyPopover;

  popoverTitle: string;

  placement: 'auto' | 'top' | 'bottom' | 'left' | 'right' | 'top-left' | 'top-right' | 'bottom-left' | 'bottom-right' | 'left-top' | 'left-bottom' | 'right-top' | 'right-bottom' | ('auto' | 'top' | 'bottom' | 'left' | 'right' | 'top-left' | 'top-right' | 'bottom-left' | 'bottom-right' | 'left-top' | 'left-bottom' | 'right-top' | 'right-bottom')[];

  triggers: string;
  container: string;
  canClosePopover: boolean;

  toggle(): void {
    super.toggle();
  }

  isOpen(): boolean {
    return super.isOpen();
  }

  constructor(
    private _elRef: ElementRef,
    private _render: Renderer2,
    injector: Injector,
    componentFactoryResolver: ComponentFactoryResolver,
    private viewContainerRef: ViewContainerRef,
    config: NgbPopoverConfig,
    ngZone: NgZone,
    private changeRef: ChangeDetectorRef,
    private appRef: ApplicationRef,
    @Inject(DOCUMENT) _document) {
    super(_elRef, _render, injector, componentFactoryResolver, viewContainerRef, config, ngZone, _document, changeRef, appRef);
    this.triggers = 'manual';
    this.popoverTitle = '';
  }

  ngOnInit(): void {   
    super.ngOnInit();
    this.ngbPopover = this.stickyPopover;
    // console.log(this.ngbPopover);
    this._render.listen(this._elRef.nativeElement, 'mouseenter', () => {
      //console.log("on");
      this.canClosePopover = true;
      this.open();
    });

    this._render.listen(this._elRef.nativeElement, 'mouseleave', () => {
      //console.log("off");
      setTimeout(() => { if (this.canClosePopover) { this.close(); } }, 100);
    });

    this._render.listen(this._elRef.nativeElement, 'click', () => {
      this.toggle();
    });
  }

  ngOnDestroy(): void {
    super.ngOnDestroy();
  }

  open() {
    super.open();
    setTimeout(() => {
      const popover = window.document.querySelector('.popover');
      if (popover !== null) {
        this._render.listen(popover, 'mouseover', () => {
          this.canClosePopover = false;
        });

        this._render.listen(popover, 'mouseout', () => {
          this.canClosePopover = true;
          setTimeout(() => { if (this.canClosePopover) { this.close(); } }, 0);
        });
      }
    }, 0);
  }

  close() {
    super.close();
  }
}
