<?php

namespace App\Search\Filters;

/**
 * Part of the JobSearchFilters contract (docs/contracts.md §1 "Sub-objects").
 *
 * Serializes to the exact shape the existing C# model uses:
 * { Year, Month, Day, Hour, Minute, Second, Millisecond } (all ints) and
 * a string form of YYYY-MM-DDThh:mm:ss.fff. Used for the custom date range
 * (SearchDateSelection = 3).
 */
final class DateField
{
    public function __construct(
        public int $Year = 0,
        public int $Month = 0,
        public int $Day = 0,
        public int $Hour = 0,
        public int $Minute = 0,
        public int $Second = 0,
        public int $Millisecond = 0,
    ) {}

    /**
     * @param  array<string, mixed>  $data
     */
    public static function fromArray(array $data): self
    {
        $allowed = ['Year', 'Month', 'Day', 'Hour', 'Minute', 'Second', 'Millisecond'];

        foreach (array_keys($data) as $key) {
            if (! in_array($key, $allowed, true)) {
                throw new InvalidFilterException("Unknown DateField field: {$key}");
            }
        }

        return new self(
            Year: (int) ($data['Year'] ?? 0),
            Month: (int) ($data['Month'] ?? 0),
            Day: (int) ($data['Day'] ?? 0),
            Hour: (int) ($data['Hour'] ?? 0),
            Minute: (int) ($data['Minute'] ?? 0),
            Second: (int) ($data['Second'] ?? 0),
            Millisecond: (int) ($data['Millisecond'] ?? 0),
        );
    }

    /**
     * @return array<string, int>
     */
    public function toArray(): array
    {
        return [
            'Year' => $this->Year,
            'Month' => $this->Month,
            'Day' => $this->Day,
            'Hour' => $this->Hour,
            'Minute' => $this->Minute,
            'Second' => $this->Second,
            'Millisecond' => $this->Millisecond,
        ];
    }

    /** YYYY-MM-DDThh:mm:ss.fff — mirrors the C# DateField.ToString(). */
    public function __toString(): string
    {
        return sprintf(
            '%d-%02d-%02dT%02d:%02d:%02d.%03d',
            $this->Year,
            $this->Month,
            $this->Day,
            $this->Hour,
            $this->Minute,
            $this->Second,
            $this->Millisecond,
        );
    }
}
