/*
 *  Redirect handler for job alert emails
 */
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { JobService } from '../../../../../jb-lib/src/public-api';

@Component({
  selector: 'app-redirect',
  templateUrl: '../../../../../jb-lib/src/lib/components/redirect/redirect.shared.html',
  styleUrls: ['../../../../../jb-lib/src/lib/components/redirect/redirect.shared.scss']
})
export class RedirectComponent implements OnInit {

  constructor(
    private jobService: JobService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const jobAlertId: number = params.nid;
      const aspNetUserId: string = params.jsid;
      this.jobService.getUrlParameters(jobAlertId, aspNetUserId).subscribe(redirectParams => {
        this.router.navigateByUrl('/job-search' + redirectParams);
      })
    });
  }

}
