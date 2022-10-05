import { Component, OnInit } from '@angular/core';
import {
  trigger,
  state,
  style,
  animate,
  transition
} from '@angular/animations';
import { Location } from '@angular/common';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import {
  FilterService,
  JobAlertModel,
  JobService,
  SystemSettingsService} from '../../../../../../jb-lib/src/public-api';
import { ToastrService } from 'ngx-toastr';
import { MatDialog } from '@angular/material/dialog';
import { map } from 'rxjs/operators';
import { BaseFormComponent } from '../../../models/base-component.model';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.scss'],
  animations: [
    trigger('smoothCollapse', [
      state(
        'initial',
        style({
          height: '0',
          overflow: 'hidden',
          opacity: '0'
        })
      ),
      state(
        'final',
        style({
          overflow: 'hidden',
          opacity: '1'
        })
      ),
      transition('initial=>final', animate('200ms')),
      transition('final=>initial', animate('200ms'))
    ])
  ]
})
export class CreateComponent extends BaseFormComponent implements OnInit {

  id = -1;
  submitted = false;
  saved = false;
  isCollapsed = true;

  private _vm = new JobAlertModel();
  private originalJobAlert: JobAlertModel;

  marginTopPx = 0;
  hasOpen = false;

  constructor(
    private router: Router,
    private location: Location,
    private jobService: JobService,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    protected dialog: MatDialog,
    private settings: SystemSettingsService,
    private filterService: FilterService
  ) {
    super(dialog);
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params: ParamMap) => {
      if (params.has('id')) {
        this.id = +params.get('id');
        this.jobService
          .getJobAlert(this.id)
          .pipe(
            map(
              x =>
                new JobAlertModel(
                  x.title,
                  x.alertFrequency,
                  x.urlParameters,
                  x.jobSearchFilters,
                  x.id
                )
            )
          )
          .subscribe(result => {
            this._vm = result;
            this.originalJobAlert = Object.assign({}, this._vm);

            this.filterService.currentFilter.activeFilters = [
              ...result.filters
            ];
            this.filterService.currentFilter.setMainFilterModel(
              result.jobSearchFiltersObj
            );

            const updatedUrl = this.filterService.iniRedirect.split(';')[0];
            const urlParams = this.getUrlParamsWithoutKeyword(this._vm.urlParameters);
            this.filterService.goLocation(updatedUrl + urlParams, true);
          });
      } else {
        // if there is no id param, then the create button was clicked.  reset the filters
        this.filterService.removeAllTags();
      }
    });

    this.filterService.mainFilterModels$.subscribe(mainFilterModel => {
      if (
        !(
          this._vm.filters.length === 0 &&
          mainFilterModel.activeFilters.length === 0
        )
      ) {
        this._vm.filters = [...mainFilterModel.activeFilters];
        this._vm.jobSearchFilters = JSON.stringify(
          mainFilterModel.convertToElasticSearchJobSearchFilters()
        );
      }

      if (window.location.href.indexOf(';') !== -1) {
        const updatedUrl = this.filterService.iniRedirect;
        this.filterService.goLocation(
          this.getUrlParamsWithoutKeyword(updatedUrl, true),
          true
        );
      }
    });

    this.setMarginTopPx(['locationDropdownMenu', 'salaryDropdownMenu']);
  }

  get noEmailHelpQuestion(): string {
    return this.settings.jbAccount.jobAlerts.noEmailHelpQuestion;
  }

  get noEmailHelpAnswer(): string {
    return this.settings.jbAccount.jobAlerts.noEmailHelpAnswer;
  }

  get MessageForRequiredJobAlertTitle(): string {
    return this.settings.shared.errors.jobAlertTitleRequired;
  }

  get MessageForDuplicateJobAlertTitle(): string {
    return this.settings.shared.errors.jobAlertTitleDuplicate;
  }

  private setMarginTopPx(elementIds: string[]): void {
    if (!this.isMobile) {
      for (const elementId of elementIds) {
        const elm = document.getElementById(elementId);
        if (elm) {
          this.onElementHeightChange(elm, (clientHeight: number) => {
            if (
              this.marginTopPx !== clientHeight && !(clientHeight === 0 && this.hasOpen)
            ) {
              this.marginTopPx = clientHeight;
            }
          });
        }
      }
    }
  }

  // keep the following for testing
  //get jobAlertVm() {
  //  return {
  //    title: this._vm.title,
  //    alertFrequency: this.vm.alertFrequency,
  //    keyword: this._vm.keyword,
  //    searchField: this._vm.searchField,
  //    originalUrlParameters: this.originalUrlParameters,
  //    urlParameters: this._vm.urlParameters,
  //    jobSearchFiltersObj: this.vm.jobSearchFiltersObj
  //  };
  //}

  private getUrlParamsWithoutKeyword(
    urlParameters: string,
    hasFullPath = false
  ): string {
    let result = urlParameters;
    if (urlParameters) {
      const params = urlParameters.split(';').filter(x => !!x);
      for (const param of params) {
        const paramName = param.split('=')[0];
        if (['search', 'title', 'employer'].indexOf(paramName) !== -1) {
          const tmpResult = params.filter(x => x !== param);
          result =
            tmpResult.length === 0
              ? ''
              : (!hasFullPath ? ';' : '') +
                (tmpResult.length === 1 ? tmpResult[0] : tmpResult.join(';'));
          break;
        }
      }
    }
    return result && result.endsWith(';')
      ? result.substr(0, result.length - 1)
      : result;
  }

  private get urlParameters(): string {
    const urlParams = this.filterService.getUrlParams(this.location.path());
    return this.getUrlParamsWithoutKeyword(urlParams) + this._vm.keywordsPart;
  }

  get vm(): JobAlertModel {
    const filtersObj = this._vm.jobSearchFiltersObj;

    if (filtersObj.Keyword !== this._vm.keyword)
      filtersObj.Keyword = this._vm.keyword;

    if (filtersObj.SearchInField !== this._vm.searchField)
      filtersObj.SearchInField = this._vm.searchField;

    this._vm.jobSearchFilters = JSON.stringify(filtersObj);

    this._vm.urlParameters = this.urlParameters;

    this._vm.alertFrequency = +this._vm.alertFrequency;

    return this._vm;
  }


  get creatingJobAlert(): boolean {
    return this.id === -1;
  }

  private get isIE(): boolean {
    const result = /MSIE|Trident/.test(navigator.userAgent);
    return result;
  }

  save(): void {
    this.submitted = true;

    if (!this._vm.title || this._vm.title.trim().length === 0) {
      document.getElementById('jobAlertHeader').scrollIntoView();
      return;
    }

    // strip out percent and semicolon characters from the keyword.
    // percents cause URL encoding errors and semicolons break the bookmarkable URL.
    this._vm.keyword = this._vm.keyword.replace('  ', ' ').replace('%', ' ').replace(';', ' ');

    this.jobService.saveJobAlert(this.vm).subscribe(
      () => {
        this.saved = true;
        this.toastr.success(
          this.creatingJobAlert
            ? 'Your Job Alert has successfully been added to your account.'
            : 'Your Job Alert has successfully been updated.'
        );
        this.router.navigate(['/job-alerts']);
        if (this.isIE) location.reload();
      },
      (error: string) => {
        console.error(error);
        if (error && error.indexOf('title') !== -1) {
          this.error = this.MessageForDuplicateJobAlertTitle;
          document.getElementById('jobAlertHeader').scrollIntoView();
        }
      }
    );
  }

  private removeAllTags(): void {
    if (this.filterService.currentFilter.activeFilters.length > 0) {
      this.filterService.removeAllTags();
    }
  }

  cancel(): void {
    this.location.back();
  }

  onSelected(searchField: string): void {
    this._vm.searchField = searchField;
  }

  onDeleted(filter: string): void {
    this._vm.filters = this._vm.filters.filter(x => x !== filter);
  }

  onDeletedAll(): void {
    this._vm.filters = this._vm.filters.filter(
      x => this._vm.filterTags.indexOf(x) === -1
    );
  }

  onCleared(): void {
    this._vm.searchField = 'all';
    this._vm.alertFrequency = 1;
    this._vm.keyword = '';
  }

  onTitleChanged(): void {
    this.clearError();
  }

  private get isMobile(): boolean {
    const result = /Mobi/i.test(navigator.userAgent) || /Android/i.test(navigator.userAgent);
    return result;
  }

  onOpenChanged(event: {
    isOpen: boolean;
    dropdownMenu: string;
    hasOpen: boolean;
  }): void {
    if (!this.isMobile) {
      setTimeout(() => {
        this.hasOpen = event.hasOpen;
        if (event.isOpen) {
          const elm = document.getElementById(event.dropdownMenu);
          if (this.marginTopPx !== elm.clientHeight) {
            this.marginTopPx = elm.clientHeight;
          }
        } else {
          if (this.marginTopPx !== 0 && !this.hasOpen) {
            this.marginTopPx = 0;
          }
        }
      });
    }
  }

  onElementHeightChange(elm: HTMLElement, callback: (newHeight: number) => void): void {
    let lastHeight = elm.clientHeight;
    (function run(): void {
      const newHeight = elm.clientHeight;
      if (lastHeight !== newHeight) callback(newHeight);

      lastHeight = newHeight;

      const onElementHeightChangeTimer = Symbol();
      if (elm[onElementHeightChangeTimer]) {
        clearTimeout(elm[onElementHeightChangeTimer]);
      }
      elm[onElementHeightChangeTimer] = setTimeout(run, 200);
    })();
  }
}
