import { Component, Input, ViewChild, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'lib-search-by',
  templateUrl: './search-by.component.html',
  styleUrls: ['./search-by.component.scss']
})
export class SearchByComponent {

  @Input() searchField = 'all';
  @Input() inJobAlert = false;
  @Output() selected = new EventEmitter<string>();  

  @ViewChild('searchByTemplate', { static: true }) searchByTemplate;

  searchFieldChange() {
    this.selected.emit(this.searchField);
  }

}
