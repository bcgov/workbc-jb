<?php

namespace App\Search\Results;

/**
 * The job-search result set — the wrapper of docs/contracts.md §2.1
 * (SearchResultsModel). The wrapper keys are camelCase (count / result /
 * pageNumber / pageSize) while each result item's keys stay PascalCase; this
 * mixed casing is part of the contract and MUST be preserved.
 */
final class SearchResult
{
    /**
     * @param  JobResult[]  $results
     */
    public function __construct(
        public int $count,
        public array $results,
        public int $pageNumber,
        public int $pageSize,
    ) {}

    /**
     * Build from a raw OpenSearch _search response.
     *
     * @param  array<string, mixed>  $response
     */
    public static function fromOpenSearchResponse(array $response, int $pageNumber, int $pageSize): self
    {
        $total = $response['hits']['total']['value'] ?? 0;
        $hits = $response['hits']['hits'] ?? [];

        $results = [];
        foreach ($hits as $hit) {
            $results[] = JobResult::fromSource($hit['_source'] ?? []);
        }

        return new self((int) $total, $results, $pageNumber, $pageSize);
    }

    /**
     * @return array{count: int, result: array<int, array<string, mixed>>, pageNumber: int, pageSize: int}
     */
    public function toArray(): array
    {
        return [
            'count' => $this->count,
            'result' => array_map(static fn (JobResult $r): array => $r->toArray(), $this->results),
            'pageNumber' => $this->pageNumber,
            'pageSize' => $this->pageSize,
        ];
    }
}
