<?php

namespace App\Search\Filters;

/**
 * Part of the JobSearchFilters contract (docs/contracts.md §1 "Sub-objects").
 *
 * { City, Region, Postal }. Postal is normalized on read — uppercased with all
 * spaces removed — matching the existing C# LocationField.Postal getter so the
 * same value hits PostalCode.keyword / geocoding regardless of how it was typed.
 */
final class LocationField
{
    private ?string $postal;

    public function __construct(
        public ?string $City = null,
        public ?string $Region = null,
        ?string $Postal = null,
    ) {
        $this->postal = $Postal;
    }

    /** Uppercased, spaces removed. */
    public function getPostal(): ?string
    {
        if ($this->postal === null) {
            return null;
        }

        return str_replace(' ', '', strtoupper($this->postal));
    }

    /**
     * @param  array<string, mixed>  $data
     */
    public static function fromArray(array $data): self
    {
        $allowed = ['City', 'Region', 'Postal'];

        foreach (array_keys($data) as $key) {
            if (! in_array($key, $allowed, true)) {
                throw new InvalidFilterException("Unknown LocationField field: {$key}");
            }
        }

        return new self(
            City: isset($data['City']) ? (string) $data['City'] : null,
            Region: isset($data['Region']) ? (string) $data['Region'] : null,
            Postal: isset($data['Postal']) ? (string) $data['Postal'] : null,
        );
    }

    /**
     * @return array<string, string|null>
     */
    public function toArray(): array
    {
        return [
            'City' => $this->City,
            'Region' => $this->Region,
            'Postal' => $this->getPostal(),
        ];
    }
}
