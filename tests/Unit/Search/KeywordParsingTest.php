<?php

namespace Tests\Unit\Search;

use App\Search\Support\KeywordParsing;
use PHPUnit\Framework\Attributes\DataProvider;
use PHPUnit\Framework\TestCase;

/**
 * Keyword parser parity tests. The four documented BRD examples (KeywordParsing.cs
 * remarks, BRD Nov 4 2019) are the acceptance cases for FND-7's keyword parser.
 */
class KeywordParsingTest extends TestCase
{
    /**
     * @return array<string, array{0: string, 1: string}>
     */
    public static function brdExamples(): array
    {
        return [
            // spaces → AND (both words must be present)
            'spaces are ANDed' => ['program manager', 'program manager'],
            // commas → OR
            'commas are ORed' => ['baker, cook', 'baker|cook'],
            // AND binds tighter than OR
            'AND groups bind tighter than OR' => ['baker manager, cook', '(baker manager)|cook'],
            // double quotes → exact phrase
            'quotes are an exact phrase' => ['"Gold Mine"', '"Gold Mine"'],
        ];
    }

    #[DataProvider('brdExamples')]
    public function test_brd_examples(string $input, string $expected): void
    {
        $this->assertSame($expected, KeywordParsing::buildSimpleQueryString($input));
    }

    public function test_empty_string_returns_empty(): void
    {
        $this->assertSame('', KeywordParsing::buildSimpleQueryString(''));
        $this->assertSame('', KeywordParsing::buildSimpleQueryString('   '));
    }

    public function test_single_word(): void
    {
        $this->assertSame('cook', KeywordParsing::buildSimpleQueryString('cook'));
    }

    public function test_three_or_terms(): void
    {
        $this->assertSame('cook|baker|cleaner', KeywordParsing::buildSimpleQueryString('cook, baker, cleaner'));
    }

    public function test_pipe_is_treated_as_or(): void
    {
        $this->assertSame('baker|cook', KeywordParsing::buildSimpleQueryString('baker|cook'));
    }

    public function test_literal_or_becomes_pipe(): void
    {
        $this->assertSame('baker|cook', KeywordParsing::buildSimpleQueryString('baker or cook'));
    }

    public function test_and_word_is_dropped(): void
    {
        $this->assertSame('program manager', KeywordParsing::buildSimpleQueryString('program and manager'));
    }

    public function test_breaking_characters_are_stripped(): void
    {
        // parentheses/tilde/braces/hash are removed by the sanitizer
        $this->assertSame('program manager', KeywordParsing::buildSimpleQueryString('(program) {manager}~'));
    }

    public function test_trailing_and_leading_commas_removed(): void
    {
        $this->assertSame('baker|cook', KeywordParsing::buildSimpleQueryString(',baker,,cook,'));
    }
}
