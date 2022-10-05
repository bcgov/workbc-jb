import { Component, Output, EventEmitter, Input, OnInit } from '@angular/core';
import {
  RecommendationFilterVm,
  RecommendationFilterInput,
} from '../../models/recommendation-filter.model';
import { RecommendedJobFilter } from '../../filters/models/job.model';
import { SystemSettingsService } from '../../services/system-settings.service';

@Component({
  selector: 'lib-recommendation-filter',
  templateUrl: './recommendation-filter.component.html',
  styleUrls: ['./recommendation-filter.component.scss']
})
export class RecommendationFilterComponent implements OnInit  {
  @Input() filterInput = new RecommendationFilterInput();
  @Input() savedState = new RecommendedJobFilter();
  @Output() filterApplied = new EventEmitter<RecommendationFilterVm>();

  vm = new RecommendationFilterVm();

  constructor(private settings: SystemSettingsService) { }

  ngOnInit(): void {
    this.vm.hasSameJobTitle = this.savedState.filterSavedJobTitles || false;
    this.vm.hasSameNoc = this.savedState.filterSavedJobNocs || false;
    this.vm.hasSameEmployer = this.savedState.filterSavedJobEmployers || false;
    this.vm.inTheSameCity = this.savedState.filterJobSeekerCity || false;
    this.vm.isYouth = this.savedState.filterIsYouth || false;
    this.vm.isIndigenous = this.savedState.filterIsIndigenous || false;
    this.vm.isNewcomer = this.savedState.filterIsNewcomers || false;
    this.vm.isApprentice = this.savedState.filterIsApprentice || false;
    this.vm.isMatureWorker = this.savedState.filterIsMatureWorkers || false;
    this.vm.hasDisability = this.savedState.filterIsPeopleWithDisabilities || false;
    this.vm.isStudent = this.savedState.filterIsStudents || false;
    this.vm.isVeteran = this.savedState.filterIsVeterans || false;
    this.vm.isMinority = this.savedState.filterIsVisibleMinority || false;
  }

  get filterIntroText(): string {
    return this.settings.jbAccount.recommendedJobs.filterIntroText;
  }

  get heightPx(): number {
    let i = 1;
    if (this.filterInput.totalSavedJobs > 0) i += 3;
    if (this.filterInput.isYouth) i++;
    if (this.filterInput.isIndigenous) i++;
    if (this.filterInput.isNewcomer) i++;
    if (this.filterInput.isApprentice) i++;
    if (this.filterInput.isMatureWorker) i++;
    if (this.filterInput.hasDisability) i++;
    if (this.filterInput.isStudent) i++;
    if (this.filterInput.isVeteran) i++;
    if (this.filterInput.isMinority) i++;

    const result = 34 * Math.ceil(i / 2);
    return result;
  }

  get isVisible(): boolean {
    return this.filterInput && (
      this.filterInput.totalSavedJobs > 0 ||
      this.filterInput.isYouth ||
      this.filterInput.isIndigenous ||
      this.filterInput.isNewcomer ||
      this.filterInput.isApprentice ||
      this.filterInput.isMatureWorker ||
      this.filterInput.hasDisability ||
      this.filterInput.isStudent ||
      this.filterInput.isVeteran ||
      this.filterInput.isMinority
    );
  }

  applyFilter(): void {
    this.filterApplied.emit(this.vm);
  }

  clearAll(): void {
    this.vm = new RecommendationFilterVm();
    this.filterApplied.emit(this.vm);
  }
}
