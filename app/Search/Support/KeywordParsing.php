<?php

namespace App\Search\Support;

/**
 * Turns a user-entered keyword string into the query string required by an
 * OpenSearch simple_query_string query. Direct port of the C# KeywordParsing
 * (WorkBC.ElasticSearch.Search/Utilities/KeywordParsing.cs) so keyword behaviour
 * is identical to the existing site.
 *
 * BRD rules (Nov 4, 2019):
 *  - Words separated by spaces are ANDed ("program manager" → both must match).
 *  - Words separated by commas are ORed ("baker, cook" → either).
 *    ANDs bind tighter than ORs, so "baker manager, cook" → "(baker manager)|cook".
 *  - Double-quoted text is an exact phrase ("Gold Mine" → the phrase).
 */
final class KeywordParsing
{
    /**
     * Build the simple_query_string "query" value from raw user keywords.
     */
    public static function buildSimpleQueryString(string $keywords): string
    {
        $keywords = self::sanitizeKeywords($keywords);

        // add a space before and after a comma so it becomes its own segment on split
        $keywords = str_replace(',', ' , ', $keywords);

        // get the quoted and unquoted segments as an array
        $segments = self::splitQuotedSegments($keywords);

        // drop bare "and" segments (queries are AND by default)
        $segments = array_values(array_filter(
            $segments,
            static fn (string $s): bool => strtolower($s) !== 'and',
        ));

        if (count($segments) === 0) {
            return '';
        }

        if (count($segments) === 1) {
            return $segments[0];
        }

        // initial tidy-up: trim, and turn bare "or" into a comma
        foreach ($segments as $i => $segment) {
            $segments[$i] = trim($segment);

            if (strtolower($segments[$i]) === 'or') {
                $segments[$i] = ',';
            }
        }

        // if there are no ',' (OR) segments then just join and return
        $hasOr = false;
        foreach ($segments as $segment) {
            if ($segment === ',') {
                $hasOr = true;
                break;
            }
        }
        if (! $hasOr) {
            return implode(' ', $segments);
        }

        $lastOrPosition = -1;
        $count = count($segments);

        for ($i = 0; $i < $count; $i++) {
            if ($segments[$i] === ',') {
                // pipe = OR in simple_query_string
                $segments[$i] = '|';

                // group the words before this comma as an AND clause
                if ($i - $lastOrPosition >= 3) {
                    self::mergeAndSegments($segments, $lastOrPosition + 1, $i - 1);
                }

                $lastOrPosition = $i;
            }

            // special handling for the trailing AND group
            if ($i === $count - 1 && $i - $lastOrPosition >= 2) {
                self::mergeAndSegments($segments, $lastOrPosition + 1, $count - 1);
            }
        }

        return implode('', array_filter($segments, static fn (string $s): bool => $s !== ''));
    }

    /**
     * Joins segments between start and end into "(dog cat)" and clears the merged
     * positions (stores the grouped value in the first position).
     *
     * @param  string[]  $segments
     */
    private static function mergeAndSegments(array &$segments, int $startIndex, int $endIndex): void
    {
        $ands = '';
        for ($j = $startIndex; $j <= $endIndex; $j++) {
            $ands .= $segments[$j].' ';
            $segments[$j] = '';
        }

        $segments[$startIndex] = '('.trim($ands).')';
    }

    /**
     * Pre-processes the keyword string before parsing.
     */
    public static function sanitizeKeywords(string $keywords): string
    {
        // pipe → comma (both mean OR; we normalize to comma)
        $keywords = str_replace('|', ',', $keywords);

        // remove characters that break OpenSearch or skew the query
        $keywords = preg_replace('/[(){}~#]/u', ' ', $keywords) ?? '';

        // remove everything except letters (incl. accented/unicode), digits, * , | _ ' " - space
        $keywords = preg_replace('/[^\x{0100}-\x{FFFF}a-zA-Z0-9\x{00C0}-\x{00FF}*,|_\'" -]/u', ' ', $keywords) ?? '';

        $keywords = trim($keywords);

        // collapse consecutive whitespace to a single space
        $keywords = preg_replace('/\s+/u', ' ', $keywords) ?? '';

        // collapse consecutive commas to a single comma
        $keywords = preg_replace('/,{2,}/', ',', $keywords) ?? '';

        // remove spaces around commas, and commas at the start/end
        $keywords = str_replace([' ,', ', '], ',', $keywords);

        return trim($keywords, ',');
    }

    /**
     * Splits a string into quoted and unquoted segments, e.g.
     * 'The quick "brown fox" jumps' → ['The', 'quick', '"brown fox"', 'jumps'].
     *
     * @return string[]
     */
    public static function splitQuotedSegments(string $line): array
    {
        $insideQuotes = false;
        $start = -1;
        $parts = [];
        $length = strlen($line);

        for ($i = 0; $i < $length; $i++) {
            $char = $line[$i];

            if ($char === ' ' || $char === "\t" || $char === "\n" || $char === "\r" || $char === "\v" || $char === "\f") {
                if (! $insideQuotes && $start !== -1) {
                    $parts[] = substr($line, $start, $i - $start);
                    $start = -1;
                }
            } elseif ($char === '"') {
                if ($start !== -1) {
                    $parts[] = '"'.substr($line, $start, $i - $start).'"';
                    $start = -1;
                }

                $insideQuotes = ! $insideQuotes;
            } elseif ($start === -1) {
                $start = $i;
            }
        }

        if ($start !== -1) {
            $parts[] = $insideQuotes
                ? '"'.substr($line, $start).'"'
                : substr($line, $start);
        }

        return $parts;
    }
}
