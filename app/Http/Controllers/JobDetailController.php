<?php

namespace App\Http\Controllers;

use App\Search\Seo\JobPostingSchema;
use App\Services\Search\JobDetailService;
use App\Support\JobSlug;
use Illuminate\Http\Request;
use Illuminate\Support\Str;

/**
 * SRCH-7 job-detail page — server-rendered, crawlable Blade (architecture.md
 * §6, ADR-002; NOT Livewire, because this page is SEO-critical).
 *
 * The URL is path-based `{slug}-{JobId}`. The job is fetched by id from the
 * derived OpenSearch read model with no ExpireDate filter (a linked expired
 * job still renders). The page emits schema.org/JobPosting JSON-LD, a canonical
 * link and EN/FR hreflang alternates, and increments the federal-job view
 * counter as a fire-and-forget side effect on the canonical request only.
 */
final class JobDetailController extends Controller
{
    public function __invoke(Request $request, JobDetailService $service, string $job)
    {
        $lang = $request->query('lang') === 'fr' ? 'fr' : 'en';
        app()->setLocale($lang);

        $jobId = JobSlug::extractId($job);

        $detail = $service->find($jobId);
        if ($detail === null) {
            abort(404);
        }

        $data = $detail->data;

        // One canonical URL per job: redirect a missing/incorrect slug (and the
        // bare-id links from SRCH-1) to the title-based slug BEFORE recording a
        // view, so redirects never inflate the counter.
        $canonicalSegment = JobSlug::path($jobId, $detail->title());
        if ($job !== $canonicalSegment) {
            return redirect()->to($this->jobUrl($canonicalSegment, $lang), 301);
        }

        $views = $service->recordView($jobId, $detail->isFederalJob(), $request->boolean('toggle'));
        if ($views !== null) {
            $data['Views'] = $views;
        }

        $canonicalUrl = $this->jobUrl($canonicalSegment, $lang);

        return view('jobs.show', [
            'job' => $data,
            'lang' => $lang,
            'canonicalUrl' => $canonicalUrl,
            'alternateEnUrl' => $this->jobUrl($canonicalSegment, 'en'),
            'alternateFrUrl' => $this->jobUrl($canonicalSegment, 'fr'),
            'metaTitle' => $this->metaTitle($data),
            'metaDescription' => $this->metaDescription($data),
            'jsonLd' => JobPostingSchema::build($data, $canonicalUrl),
        ]);
    }

    private function jobUrl(string $segment, string $lang): string
    {
        $params = ['job' => $segment];
        if ($lang === 'fr') {
            $params['lang'] = 'fr';
        }

        return route('jobs.show', $params);
    }

    /**
     * @param  array<string, mixed>  $job
     */
    private function metaTitle(array $job): string
    {
        $title = (string) ($job['Title'] ?? 'Job posting');
        $employer = $job['EmployerName'] ?? null;

        $label = $employer ? "{$title} — {$employer}" : $title;

        return "{$label} | WorkBC Job Board";
    }

    /**
     * @param  array<string, mixed>  $job
     */
    private function metaDescription(array $job): string
    {
        $parts = array_filter([
            $job['Title'] ?? null,
            $job['EmployerName'] ?? null,
            $job['City'] ?? null,
            $job['SalarySummary'] ?? null,
        ]);

        $text = $parts === [] ? 'Job posting on the WorkBC Job Board.' : implode(' · ', $parts);

        return Str::limit(trim($text), 155);
    }
}
