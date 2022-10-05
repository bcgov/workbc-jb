import { Injectable, Renderer2, RendererFactory2  } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GlobalService {

  private props: unknown = {};
  private renderer: Renderer2;

  constructor(rendererFactory: RendererFactory2) {
    //create renderer instance
    this.renderer = rendererFactory.createRenderer(null, null);
  }

  getApiBaseUrl(): string {
    return this.getRootProp('api', 'http://localhost:9220/');
  }

  getJbSearchUrl(): string {
    return this.getRootProp('jbsearch', '/');
  }
  
  getJbAccountUrl(): string {
    return this.getRootProp('jbaccount', '/');
  }

  getRootProp(name: string, defaultValue: string): string {
    if (this.props[name] === undefined) {
      const rootElement = this.renderer.selectRootElement('app-root', true);
      if (rootElement && rootElement.getAttribute(name) != null) {
        this.props[name] = rootElement.getAttribute(name);
      } else {
        console.log('Property "' + name + '" is not set in <app-root>.  Using default value "' + defaultValue + '"');
        this.props[name] = defaultValue;
      }
    }
    return this.props[name];
  }
}
