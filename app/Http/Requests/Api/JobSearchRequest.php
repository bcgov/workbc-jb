<?php

namespace App\Http\Requests\Api;

use App\Search\Filters\InvalidFilterException;
use App\Search\Filters\JobSearchFilters;
use Illuminate\Foundation\Http\FormRequest;
use Illuminate\Http\Exceptions\HttpResponseException;

/**
 * §2.1 `POST /api/Search/JobSearch` request body — a `JobSearchFilters` JSON
 * object (contracts.md §1). Strict/fail-closed (contracts.md §1 "Serialization
 * rules" + §3): any field {@see JobSearchFilters} doesn't recognise is
 * rejected with HTTP 400, mirroring the legacy `MissingMemberHandling.Error`
 * behaviour — never silently dropped.
 */
final class JobSearchRequest extends FormRequest
{
    private JobSearchFilters $filters;

    public function authorize(): bool
    {
        return true;
    }

    /**
     * @return array<string, mixed>
     */
    public function rules(): array
    {
        // No per-field rules: JobSearchFilters::fromArray() (called from
        // passedValidation() below) IS the validation — it already knows every
        // supported field/type and throws on anything else.
        return [];
    }

    protected function passedValidation(): void
    {
        try {
            $this->filters = JobSearchFilters::fromArray($this->all());
        } catch (InvalidFilterException $e) {
            throw new HttpResponseException(
                response()->json(['message' => $e->getMessage()], 400)
            );
        }
    }

    public function filters(): JobSearchFilters
    {
        return $this->filters;
    }
}
