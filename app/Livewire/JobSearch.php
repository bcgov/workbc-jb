<?php

namespace App\Livewire;

use App\Search\Filters\JobSearchFilters;
use App\Search\Results\SearchResult;
use App\Services\Search\JobSearchService;
use Livewire\Attributes\Layout;
use Livewire\Attributes\Title;
use Livewire\Attributes\Url;
use Livewire\Component;

/**
 * SRCH-1 — the public, server-rendered search results page.
 *
 * Server-rendered + crawlable (ADR-002): as a full-page Livewire component the
 * initial GET renders the results into the HTML (not injected client-side), and
 * each result links to the path-based job-detail URL. Keyword, scope, sort and
 * page are bound to the query string so results are shareable/deep-linkable.
 *
 * No business logic here (copilot-instructions §6): the component maps its bound
 * state to a {@see JobSearchFilters} value object and delegates to
 * {@see JobSearchService}. Alpine handles pure view state elsewhere; this
 * component is the "data" side of the Alpine-vs-Livewire rule.
 */
#[Layout('components.layouts.app')]
#[Title('Find jobs — WorkBC Job Board')]
final class JobSearch extends Component
{
    #[Url(as: 'q', except: '')]
    public string $keyword = '';

    /** Search scope: all | title | employer | jobId. */
    #[Url(as: 'in', except: 'all')]
    public string $searchIn = 'all';

    /** SortOrder enum (contracts §1): 1..11, default 1 = DatePosted desc. */
    #[Url(except: 1)]
    public int $sort = 1;

    #[Url(except: 1)]
    public int $page = 1;

    public int $pageSize = 20;

    /** @var string[] */
    private const SEARCH_IN = ['all', 'title', 'employer', 'jobId'];

    /** Submit the keyword/scope form: re-run from page 1. */
    public function applySearch(): void
    {
        $this->page = 1;
    }

    /** Changing sort restarts paging. */
    public function updatedSort(): void
    {
        $this->page = 1;
    }

    public function gotoPage(int $page): void
    {
        $this->page = max(1, $page);
    }

    public function render()
    {
        try {
            $result = app(JobSearchService::class)->search($this->toFilters());
            $unavailable = false;
        } catch (\Throwable $e) {
            // Never leak an OpenSearch outage as a 500 on a public page; degrade to
            // an accessible "unavailable" message and log for ops.
            report($e);
            $result = new SearchResult(0, [], max(1, $this->page), $this->pageSize);
            $unavailable = true;
        }

        return view('livewire.job-search', [
            'result' => $result,
            'unavailable' => $unavailable,
            'sortOptions' => self::sortOptions(),
        ]);
    }

    /**
     * Map the bound UI state to the shared filter value object.
     */
    public function toFilters(): JobSearchFilters
    {
        $searchIn = in_array($this->searchIn, self::SEARCH_IN, true) ? $this->searchIn : 'all';
        $sort = ($this->sort >= 1 && $this->sort <= 11) ? $this->sort : 1;

        return JobSearchFilters::fromArray([
            'Keyword' => $this->keyword !== '' ? $this->keyword : null,
            'SearchInField' => $searchIn,
            'SortOrder' => $sort,
            'Page' => max(1, $this->page),
            'PageSize' => $this->pageSize,
        ]);
    }

    /**
     * SortOrder enum (contracts §1) → human labels for the sort control.
     *
     * @return array<int, string>
     */
    public static function sortOptions(): array
    {
        return [
            1 => 'Date posted (newest first)',
            2 => 'Date posted (oldest first)',
            3 => 'Job title (A–Z)',
            4 => 'Job title (Z–A)',
            5 => 'City (A–Z)',
            6 => 'City (Z–A)',
            7 => 'Employer (A–Z)',
            8 => 'Employer (Z–A)',
            9 => 'Salary (low to high)',
            10 => 'Salary (high to low)',
            11 => 'Relevance',
        ];
    }
}
