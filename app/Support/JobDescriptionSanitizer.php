<?php

namespace App\Support;

/**
 * Turns an external posting's `JobDescription` HTML (from Innovibe, stored in
 * OpenSearch) into safe plain text with preserved line breaks, for rendering
 * on our own detail page (SRCH-7b).
 *
 * Security: all markup is stripped (no tags, scripts or event handlers survive
 * — OWASP A03 XSS), and the Blade view re-escapes the result with `e()` before
 * output, so the text is neutralised twice. Block-level tags become newlines and
 * list items become bullets so the plain text stays readable.
 */
final class JobDescriptionSanitizer
{
    public static function toPlainText(?string $html): string
    {
        if ($html === null || trim($html) === '') {
            return '';
        }

        $text = str_replace(["\r\n", "\r"], "\n", $html);

        // Preserve intentional breaks: <br> and block-closing tags -> newline.
        $text = preg_replace('/<\s*br\s*\/?\s*>/i', "\n", $text);
        $text = preg_replace(
            '/<\/\s*(p|div|ul|ol|tr|h[1-6]|section|article|header|footer|table|blockquote)\s*>/i',
            "\n",
            $text,
        );
        // List items become bullet lines.
        $text = preg_replace('/<\s*li[^>]*>/i', "\n\u{2022} ", $text);

        // Strip every remaining tag, then decode entities to their characters.
        $text = strip_tags($text);
        $text = html_entity_decode($text, ENT_QUOTES | ENT_HTML5, 'UTF-8');

        // Tidy whitespace: trim each line and collapse runs of blank lines.
        $lines = array_map(static fn (string $line): string => trim($line), explode("\n", $text));
        $text = preg_replace('/\n{3,}/', "\n\n", implode("\n", $lines));

        return trim($text);
    }
}
