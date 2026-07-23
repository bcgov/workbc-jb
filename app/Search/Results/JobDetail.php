<?php

namespace App\Search\Results;

/**
 * The projected job-detail read model handed to the Blade view — the §2.1
 * job shape (PascalCase keys, Rule B mapping via {@see JobResult}) plus the
 * live view count decorated on the federal-job read path.
 */
final class JobDetail
{
    /**
     * @param  array<string, mixed>  $data  Projected §2.1 job fields.
     */
    public function __construct(
        public readonly array $data,
        public readonly ?int $views = null,
    ) {}

    public function isFederalJob(): bool
    {
        return ($this->data['IsFederalJob'] ?? false) === true;
    }

    public function title(): ?string
    {
        return $this->data['Title'] ?? null;
    }
}
