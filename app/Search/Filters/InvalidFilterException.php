<?php

namespace App\Search\Filters;

use InvalidArgumentException;

/**
 * Thrown when a JobSearchFilters payload contains an unknown field or an
 * unsupported version. Mirrors the C# MissingMemberHandling.Error behaviour
 * (docs/contracts.md §1 "Serialization rules") so the API layer can map it to
 * an HTTP 400 (fail closed on unexpected input).
 */
final class InvalidFilterException extends InvalidArgumentException {}
