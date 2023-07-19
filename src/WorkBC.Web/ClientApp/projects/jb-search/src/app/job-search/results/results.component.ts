/// <reference types="@types/google.maps" />

import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { JobAlertComponent } from './job-alert/job-alert.component';
import { MarkerClusterer, SuperClusterAlgorithm, Cluster, ClusterStats, Renderer } from '@googlemaps/markerclusterer';
import {
  GoogleMapData,
  LocationInformation,
  MainFilterModel,
  DataService,
  FilterService,
  Job,
  PaginationComponent,
  JobBase,
  ExternalJobModalComponent,
  SystemSettingsService,
  GlobalService
} from '../../../../../jb-lib/src/public-api';

class LegacyMarkerClusterRenderer implements Renderer {
  public render(
    { count, position }: Cluster,
    stats: ClusterStats
  ): google.maps.Marker {
    // change color if this cluster has more markers than the mean cluster

    let color;
    let size;
    if (count >= 10000) {
      color = '#9C00FF';
      size = 60;
    } else if (count >= 1000) {
      color = '#FF00ED';
      size = 54;
    } else if (count >= 100) {
      color = '#FF0000';
      size = 48;
    } else if (count >= 10) {
      color = '#FFBF00';
      size = 42;
    } else {
      color = '#008CFF';
      size = 36
    }

    // create svg url with fill color
    const svg = window.btoa(`
  <svg fill="${color}" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 240 240">
    <circle cx="120" cy="120" opacity=".6" r="70" />
    <circle cx="120" cy="120" opacity=".3" r="90" />
    <circle cx="120" cy="120" opacity=".2" r="110" />
  </svg>`);

    // create marker using svg icon
    return new google.maps.Marker({
      position,
      icon: {
        url: `data:image/svg+xml;base64,${svg}`,
        scaledSize: new google.maps.Size(size, size),
      },
      label: {
        text: String(count),
        color: "rgba(0,0,0,0.9)",
        fontSize: "11px",
        fontWeight: "bold"
      },
      title: `Cluster of ${count} markers`,
      // adjust zIndex to be above other markers
      zIndex: Number(google.maps.Marker.MAX_ZINDEX) + count,
    });
  }
}

@Component({
  selector: 'app-results',
  templateUrl: './results.component.html',
  styleUrls: ['./results.component.scss']
})
export class ResultsComponent implements OnInit {
  @Input() isJobNoSearch = false;

  resultsCount = 0;
  markersCount = 0;
  mappedJobCount = 0;
  results: Job[];
  mainFilterModel: MainFilterModel;
  justLoaded = false;

  // pagination control
  @ViewChild('pagination')
  paginationElement: PaginationComponent;

  //Google map element
  @ViewChild('map') mapElement: { nativeElement };

  //reference to the google map object
  map: google.maps.Map;

  //reference to all markers on the current map
  markers: google.maps.Marker[];

  //reference to the clusters on the map
  markerCluster: MarkerClusterer; 

  //Show map on screen
  showMap = false;

  //Indicator if map loaded
  mapLoaded = false;

  //Show loader while we load the map
  showMapLoader = false;

  //points
  points: Array<GoogleMapData>;
  locationInformation: LocationInformation[] = [];

  loading = false;

  urlParams: string;

  constructor(
    private dataService: DataService,
    private filterService: FilterService,
    private modalService: NgbModal,
    private location: Location,
    private settings: SystemSettingsService,
    private route: ActivatedRoute,
    private globalService: GlobalService
  ) { }

  get htmlForNoResultsFound(): string {
    return this.settings.jbSearch.errors.noSearchResults;
  }

  get tooManyMapPins(): string {
    return this.settings.jbSearch.errors.tooManyMapPins
      .replace('{0}', this.resultsCount.toLocaleString('en'))
      .replace('{1}', '5,000');
  }

  get noMapPins(): string {
    return this.settings.jbSearch.errors.noMapPins;
  }

  get missingMapPins(): string {
    // {0} of the jobs in your search results do not have a location specified and because of this,
    // they do not appear on the map.All jobs that match your filter(with locations or otherwise)
    // will appear in the list of job search results.
    if (this.resultsCount - this.mappedJobCount === 1) {
      return this.settings.jbSearch.errors.missingOneMapPin;
    }
    return this.settings.jbSearch.errors.missingMapPins
      .replace('{0}', (this.resultsCount - this.mappedJobCount).toLocaleString('en'));
  }

  get vm(): MainFilterModel {
    return this.filterService.currentFilter;
  }

  private getJobs(jobs: JobBase[]): Job[] {
    const result: Job[] = [];
    for (const job of jobs) {
      result.push(new Job(null, job));
    }
    return result;
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.filterService.setBookmarkableUrl(params);
    });

    this.filterService.mainFilterModels$.subscribe(filter => {
      this.loading = true;

      //set local variable (for use later in maps)
      this.mainFilterModel = filter;

      //pass the filter to the results service
      this.dataService.getResults(filter).subscribe(results => {

        this.results = this.getJobs(results.result);

        //total results
        this.resultsCount = results.count;

        // if the pagination is messed up then go back to page 1
        if (this.results.length === 0 && results.count !== 0) {
          this.mainFilterModel.pagination.currentPage = 1;
          this.filterService.setFilters();
        }

        // scroll to the searchSrollAnchor
        if (!this.justLoaded) {
          this.filterService.scrollIntoView();
        }

        // don't scroll to the searchSrollAnchor the first time the page loads
        this.justLoaded = false;

        //paging
        this.paginationElement.setResultCount(this.resultsCount);

        this.urlParams = this.filterService.getUrlParams(this.location.path());

        this.loading = false;
      });

      //refresh Google Maps results
      if (this.showMap) {
        //only refresh google map results if the map is currently showing,
        //else this is not necessary as this will happen automatically when the map gets opened.
        this.renderMap(true);
      }
    });
  }

  ngOnDestroy() {
    this.map = null;
    this.markerCluster = null;
    this.markers = [];
    this.points = [];
  }

  get isJobNumberSearch(): boolean {
    return this.isJobNoSearch
      || this.vm.activeFilters.some(f => f.startsWith('Job number'));
  }

  get defaultSearchRadius(): number {
    return this.settings.shared?.settings.defaultSearchRadius || 15;
  }

  sortByChange() {
    this.filterService.sortByChange('sortby');
  }

  open() {
    const modalRef = this.modalService.open(JobAlertComponent, {
      container: 'app-root',
      centered: true
    });
    (modalRef.componentInstance as JobAlertComponent).urlParams = this.urlParams;
  }

  onCleared(): void {
    if (this.showMap) {
      this.renderMap(false);
    }
  }

  renderMap(doRefresh) {
    //set map properties
    const mapProperties = {
      center: new google.maps.LatLng(49.167857, -123.119118),
      zoom: 16,
      minZoom: 5,
      maxZoom: 16,
      mapTypeId: google.maps.MapTypeId.ROADMAP,
      streetViewControl: false,
      mapTypeControl: false
    };

    if (doRefresh) {
      this.showMap = true;
      this.showMapLoader = true;
      this.mapLoaded = false;
    } else {
      //Toggle map
      this.showMap = !this.showMap;

      if (this.showMap) {
        this.showMapLoader = true;
        this.mapLoaded = false;
      }
    }

    if (this.showMap) {
      //set map center
      this.map = new google.maps.Map(
        this.mapElement.nativeElement,
        mapProperties
      );
      const thisButton = document.createElement("div");
      const britishColumbia = { lat: 53.726668, lng: -127.647621 };
      thisButton.textContent = "View full map";
      thisButton.style.background = "white";
      thisButton.style.border = "1px solid #CCC";
      thisButton.style.cursor = "pointer";
      thisButton.style.boxShadow = "0 0 5px -1px rgba(0,0,0,0.2)";
      thisButton.style.padding = "5px";
      thisButton.addEventListener('click', () => {
        this.map.setCenter(britishColumbia);
        this.map.setZoom(5);
      });
      this.map.controls[google.maps.ControlPosition.TOP_CENTER].push(thisButton);

      //clear map
      if (this.markers != null) {
        // Clear out the old markers.
        this.markers.forEach(marker => {
          marker.setMap(null);
        });
        this.markers = [];

        //clear clusters
        if (this.markerCluster) {
          this.markerCluster.clearMarkers();
        }
      }

      //reference variables
      const mapReference = this.map;
      const mapElementReference = this.mapElement;

      //Call service for map data
      this.dataService.getMapResults(this.mainFilterModel).subscribe(result => {
        //init variable
        this.points = new Array<GoogleMapData>();

        this.mappedJobCount = result.uniqueJobCount;

        //add points to array that will be used to display markers on the GA map
        for (let i = 0; i < result.results.length; i++) {
          this.points.push({
            longitude: result.results[i].longitude,
            latitude: result.results[i].latitude,
            jobId: result.results[i].jobId,
            numberOfJobs: 0
          });
        }

        //get number of jobs per location(long/lat)
        for (let k = 0; k < this.points.length; k++) {
          this.points[k].numberOfJobs = this.getNumberOfJobsForLocation(
            this.points[k].longitude,
            this.points[k].latitude
          );
        }

        //loop through points and create markers on the map
        this.markers = this.points.map(data => {
          //create marker
          const marker = new google.maps.Marker({
            position: new google.maps.LatLng(data.latitude, data.longitude),
            map: mapReference,
            animation: null, // anything other than null causes massive memory spikes
            icon: {
              url:
                'http://maps.google.com/mapfiles/ms/icons/' +
                (data.numberOfJobs > 1 ? 'blue' : 'red') +
                '-dot.png'
            }
          });

          marker['jobId'] = data.jobId;

          //add "click" event to marker
          google.maps.event.addListener(marker, 'click', () => {
            this.onMarkerClick(marker); 
          });

          return marker;
        });

        // Add a marker clusterer to manage the markers.
        //after maxZoom the clusters will disappear and show markers
        this.markerCluster = new MarkerClusterer(
          {
            map: this.map,
            markers: this.markers,
            algorithm: new SuperClusterAlgorithm({ maxZoom: 14 }),
            renderer: new LegacyMarkerClusterRenderer()
          }
        );

        // set the opacity to zero and top margin to -500px to give markercluser a bit more time to render
        mapElementReference.nativeElement.style.opacity = 0;
        mapElementReference.nativeElement.style.marginTop = '-500px';

        this.mapLoaded = true;

        // wait 500 milliseconds for the markerclusterer to finish
        setTimeout(() => {
          this.showMapLoader = false;
          mapElementReference.nativeElement.style.opacity = 1;
          mapElementReference.nativeElement.style.marginTop = '0';
        }, 500);

        //set the result count to the number of markers shown
        this.markersCount = this.markers.length;

        // set bounds
        this.zoomToMarkers();
      });
    } else {
      //hide loader
      this.showMapLoader = false;
      this.mapLoaded = false;
    }
  }

  infoWin: google.maps.InfoWindow;

  onMarkerClick(marker) {
    if (this.infoWin) {
      this.infoWin.close();
    }

    // get all the markers with the same lat/lon as the one you clicked on
    const jobIdMarkers = this.markers.filter(
      point =>
        point.getPosition().lat() === marker.position.lat() &&
        point.getPosition().lng() === marker.position.lng()
    );
    const jobIds = jobIdMarkers.map(m => m['jobId']).join(',');

    //GET
    this.dataService.getLocationInformation(jobIds).subscribe(
      result => {
        if (result != null) {
          let content = '';

          if (jobIdMarkers.length > 1) {
            content +=
              `<div id="jobNo">
                  There are ${jobIdMarkers.length} jobs at this location
                  <img src="${this.globalService.getApiBaseUrl()}assets/icons/arrow-right.svg" width="14px" height="14px">
               </div>
               <div id="jobDetails" hidden class="p-1">`; 
          }

          for (let i = 0; i < result.length; i++) {
            const dPosted = new Date(result[i].datePosted);
            const dExpire =
              result[i].isFederalJob && result[i].expire != ''
                ? new Date(result[i].expire)
                : null;

            let jobTypeAndTerm = result[i].hoursOfWork;
            jobTypeAndTerm +=
              result[i].hoursOfWork.length > 0 &&
              result[i].periodOfEmployment.length > 0
                ? ', ' + result[i].periodOfEmployment
                : result[i].periodOfEmployment;

            //Title
            content +=
              (result[i].isFederalJob
                ? '<b><a href=\'#/job-details/' +
                  result[i].jobId +
                  '\'>' +
                  result[i].jobTitle +
                  '</a></b><br />'
                : '<b class=\'externalJob\' data-jobId=\'' +
                  result[i].jobId +
                  '\' data-title=\'' +
                  result[i].jobTitle +
                  '\' data-source=\'' +
                  result[i].jobSource +
                  '\' data-url=\'' +
                  result[i].externalUrl +
                  '\'>' +
                  result[i].jobTitle +
                  '</b><br />') +
              //Company
              '<b>' +
              result[i].company +
              '</b><br /><br />' +
              //Job source (external job)
              (!result[i].isFederalJob && result[i].jobSource != ''
                ? '<b>Source:</b> ' + result[i].jobSource + '<br />'
                : '') +
              //Location
              (result[i].city != ''
                ? '<b>Location:</b> ' + result[i].city + '<br />'
                : '') +
              //Salary
              (result[i].salary != '' && result[i].salary != null
                ? '<b>Salary:</b> ' + result[i].salary + '<br />'
                : '<b>Salary:</b> N/A<br />') +
              //Job Type, Job Term
              (jobTypeAndTerm.length > 0
                ? '<b>Job type:</b> ' + jobTypeAndTerm + '<br />'
                : '') +
              //Date posted
              '<b>Posted:</b> ' +
              this.date2Str(dPosted, 'yyyy-MM-dd') +
              ' <br />' +
              //Expire
              (dExpire != null
                ? '<b>Expires:</b> ' + this.date2Str(dExpire, 'yyyy-MM-dd')
                : '') +
              '<br /><br /><br />';
          }

          content += (jobIdMarkers.length > 1 ? '</div>' : '');

          //display info window for marker
          this.infoWin = new google.maps.InfoWindow({
            content: content,
            maxWidth: 300
          });
          this.infoWin.open(this.map, marker);

          google.maps.event.addListener(this.infoWin, 'domready', () => {
            //test that we can find the DOM elements for external jobs
            if (document.getElementsByClassName('externalJob') != null) {
              const jobs = document.getElementsByClassName('externalJob');

              Array.from(jobs).map(element => {
                //Add CLICK listner to the job title
                element.addEventListener('click', () => {
                  const modalRef = this.modalService.open(
                    ExternalJobModalComponent,
                    {
                      container: 'app-root',
                      size: 'lg',
                      centered: true
                    }
                  );

                  //read data- attributes and set the modal properties
                  modalRef.componentInstance.jobUrl = element.getAttribute(
                    'data-url'
                  );
                  modalRef.componentInstance.jobName = element.getAttribute(
                    'data-title'
                  );
                  modalRef.componentInstance.jobOrigin = element.getAttribute(
                    'data-source'
                  );
                });
              });
            }

            const div1 = document.getElementById('jobNo');
            if (div1) {
              div1.addEventListener('click', () => {
                //div1.hidden = true;
                //const div2 = document.getElementById('jobDetails');
                //div2.hidden = false;

                this.infoWin.setContent(content
                  .replace('id="jobDetails" hidden', 'id="jobDetails"')
                  .replace('id="jobNo"', 'id="jobNo" hidden'));
                this.infoWin.open(this.map, marker);
              });
            }
          });

          google.maps.event.addListenerOnce(this.infoWin, 'closeclick', () => {
            this.infoWin = null;
          });
        }
      },
      error => console.error(error)
    );
  }

  /* Zooms the map to include all the markers */
  zoomToMarkers() {
    const bounds = new google.maps.LatLngBounds();

    this.markers.forEach(item => {
      const pos = item.getPosition();
      const lat = pos.lat();
      const lng = pos.lng();
      if (lat > 48.2 && lat < 60 && lng < -114 && lng > -139) {
        // ignore points outside BC
        bounds.extend(pos);
      }
    });

    if (this.markers.length > 1) {
      const padding = { top: 1, right: 0, bottom: 1, left: 0 };
      // slight delay to let the canvas finish resizing
      setTimeout(() => {
        this.map.fitBounds(bounds, padding);
      }, 50);
    } else if (this.markers.length === 1) {
      this.map.setCenter(this.markers[0].getPosition());
      this.map.setZoom(11);
    }
  }

  getNumberOfJobsForLocation(lon, lat) {
    let count = 0;
    for (let i = 0; i < this.points.length; i++) {
      if (this.points[i].longitude == lon && this.points[i].latitude == lat) {
        count++;
      }
    }

    return count;
  }

  date2Str(x, y) {
    const z = {
      M: x.getMonth() + 1,
      d: x.getDate(),
      h: x.getHours(),
      m: x.getMinutes(),
      s: x.getSeconds()
    };
    y = y.replace(/(M+|d+|h+|m+|s+)/g, function(v) {
      return ((v.length > 1 ? '0' : '') + z[v.slice(-1)]).slice(-2);
    });

    return y.replace(/(y+)/g, function(v) {
      return x
        .getFullYear()
        .toString()
        .slice(-v.length);
    });
  }
}
