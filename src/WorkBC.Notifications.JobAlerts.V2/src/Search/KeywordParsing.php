<?php

declare(strict_types=1);

namespace WorkBC\JobAlertNotifier\Search;

/**
 * Utilities for turning a user-inputted keyword string into the format
 * required by an Elasticsearch simple_query_string query.
 *
 * Line-for-line port of WorkBC.ElasticSearch.Search.Utilities.KeywordParsing.
 * BRD rules: spaces = AND, commas/pipes = OR (comma groups get (+a +b)
 * grouping), double quotes = exact phrase.
 */
final class KeywordParsing
{
    public static function buildSimpleQueryString(string $keywords): string
    {
        $keywords = self::sanitizeKeywords($keywords);

        // add a space before and after a comma so it will become its own segment during the split
        $keywords = str_replace(',', ' , ', $keywords);

        // get the quoted and unquoted segments as an array
        $segments = self::splitQuotedSegments($keywords);

        // remove any segments that just contain the word and (all queries are and queries by default)
        $segments = array_values(array_filter($segments, fn(string $s) => mb_strtolower($s) !== 'and'));

        if (count($segments) === 0) {
            return '';
        }

        // if there is only 1 segment then exit and return it
        if (count($segments) === 1) {
            return $segments[0];
        }

        // loop through the segments to do some initial tidying up
        foreach ($segments as $i => $segment) {
            // trim spaces from beginning and end of the segment
            $segments[$i] = trim($segment);

            // if a segment just contains the word OR then turn it into a comma since an OR search was the
            // user's intention anyway
            if (mb_strtolower($segments[$i]) === 'or') {
                $segments[$i] = ',';
            }
        }

        // if there are no ',' (OR) segments then just join the segments back together and return
        if (!in_array(',', $segments, true)) {
            return implode(' ', $segments);
        }

        $lastOrPosition = -1;
        $count = count($segments);

        // loop through the segments
        for ($i = 0; $i < $count; $i++) {
            // if we find a comma (OR search)...
            if ($segments[$i] === ',') {
                // turn it into a pipe character (used by elasticsearch simple_query_string for OR)
                $segments[$i] = '|';

                // turn the words before the comma into 'AND' (+) conditions and group them with brackets around them
                if ($i - $lastOrPosition >= 3) {
                    // loop though all the segments after the last comma and join them as ANDs
                    self::mergeAndSegments($segments, $lastOrPosition + 1, $i - 1);
                }

                $lastOrPosition = $i;
            }

            // handle special condition for the last segment in the list.
            if ($i === $count - 1 && $i - $lastOrPosition >= 2) {
                // loop though all the segments after the last comma and join them as ANDs
                self::mergeAndSegments($segments, $lastOrPosition + 1, $count - 1);
            }
        }

        // remove the blank segments and then join all the segments
        return implode('', array_filter($segments, fn(string $s) => $s !== ''));
    }

    /**
     * Joins segments between start and end into a query like (+dog +cat).
     * Stores the new value in the first segment position and clears the other
     * segment positions.
     *
     * @param list<string> $segments
     */
    private static function mergeAndSegments(array &$segments, int $startIndex, int $endIndex): void
    {
        $ands = '';
        for ($j = $startIndex; $j <= $endIndex; $j++) {
            $ands .= $segments[$j] . ' ';
            $segments[$j] = '';
        }

        // put round brackets around the joined segments
        $segments[$startIndex] = '(' . trim($ands) . ')';
    }

    /**
     * Pre-processes the string before parsing.
     */
    public static function sanitizeKeywords(string $keywords): string
    {
        // turn pipe character into comma so it can be used as OR condition
        $keywords = str_replace('|', ',', $keywords);

        // remove characters that will break elasticsearch or impact the query results
        $keywords = preg_replace('/(\(|\)|~|\{|\}|\#)/u', ' ', $keywords);

        // remove everything except A-Z, a-z, 0-9, À-ÿ, *, (comma), |, ', ", -, (space)
        $keywords = preg_replace('/[^\x{0100}-\x{FFFF}a-zA-Z0-9À-ÿ*,|_\'" -]/u', ' ', $keywords);

        // trim whitespace from the beginning and end
        $keywords = trim($keywords);

        // remove whitespace characters and turn consecutive spaces into single spaces
        $keywords = preg_replace('/\s+/u', ' ', $keywords);

        // turn consecutive commas into single commas (comma means OR)
        $keywords = preg_replace('/,{2,}/', ',', $keywords);

        // remove spaces around commas and commas at the beginning and end
        $keywords = trim(str_replace([' ,', ', '], ',', $keywords), ',');

        return $keywords;
    }

    /**
     * Parses a string into quoted and unquoted segments
     * e.g. 'The quick "brown fox" jumps "over the" "lazy" dog'
     * becomes ['The', 'quick', '"brown fox"', 'jumps', '"over the"', '"lazy"', 'dog']
     *
     * @return list<string>
     */
    public static function splitQuotedSegments(string $line): array
    {
        $insideQuotes = false;
        $start = -1;

        $parts = [];
        $chars = $line === '' ? [] : mb_str_split($line);
        $len = count($chars);

        for ($i = 0; $i < $len; $i++) {
            $ch = $chars[$i];

            if (preg_match('/^\s$/u', $ch) === 1) {
                if (!$insideQuotes) {
                    if ($start !== -1) {
                        $parts[] = implode('', array_slice($chars, $start, $i - $start));
                        $start = -1;
                    }
                }
            } elseif ($ch === '"') {
                if ($start !== -1) {
                    $parts[] = '"' . implode('', array_slice($chars, $start, $i - $start)) . '"';
                    $start = -1;
                }

                $insideQuotes = !$insideQuotes;
            } else {
                if ($start === -1) {
                    $start = $i;
                }
            }
        }

        if ($start !== -1) {
            if ($insideQuotes) {
                $parts[] = '"' . implode('', array_slice($chars, $start)) . '"';
            } else {
                $parts[] = implode('', array_slice($chars, $start));
            }
        }

        return $parts;
    }
}
