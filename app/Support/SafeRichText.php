<?php

namespace App\Support;

/**
 * Sanitizes small admin-managed HTML fragments from SystemSettings for safe
 * server-side rendering.
 */
final class SafeRichText
{
    /**
     * @var array<string, true>
     */
    private const ALLOWED_TAGS = [
        'p' => true,
        'br' => true,
        'ul' => true,
        'ol' => true,
        'li' => true,
        'strong' => true,
        'em' => true,
        'b' => true,
        'i' => true,
        'a' => true,
    ];

    /**
     * @var array<string, true>
     */
    private const DROP_TAGS = [
        'script' => true,
        'style' => true,
        'iframe' => true,
        'object' => true,
        'embed' => true,
        'svg' => true,
        'math' => true,
        'meta' => true,
        'link' => true,
    ];

    public static function sanitize(?string $html): string
    {
        if ($html === null || trim($html) === '') {
            return '';
        }

        $document = new \DOMDocument('1.0', 'UTF-8');
        $wrapped = '<div>'.$html.'</div>';

        $previous = libxml_use_internal_errors(true);
        $loaded = $document->loadHTML('<?xml encoding="UTF-8">'.$wrapped, LIBXML_HTML_NOIMPLIED | LIBXML_HTML_NODEFDTD);
        libxml_clear_errors();
        libxml_use_internal_errors($previous);

        if (! $loaded) {
            return e(strip_tags($html));
        }

        $root = $document->getElementsByTagName('div')->item(0);

        if (! $root instanceof \DOMNode) {
            return e(strip_tags($html));
        }

        return self::renderChildren($root);
    }

    private static function renderChildren(\DOMNode $node): string
    {
        $html = '';

        foreach ($node->childNodes as $child) {
            $html .= self::renderNode($child);
        }

        return $html;
    }

    private static function renderNode(\DOMNode $node): string
    {
        if ($node->nodeType === XML_TEXT_NODE) {
            return e($node->nodeValue ?? '');
        }

        if ($node->nodeType !== XML_ELEMENT_NODE || ! $node instanceof \DOMElement) {
            return '';
        }

        $tag = strtolower($node->tagName);

        if (isset(self::DROP_TAGS[$tag])) {
            return '';
        }

        if (! isset(self::ALLOWED_TAGS[$tag])) {
            return self::renderChildren($node);
        }

        if ($tag === 'br') {
            return '<br>';
        }

        $attrs = '';

        if ($tag === 'a') {
            $href = trim((string) $node->getAttribute('href'));

            if (self::isAllowedHref($href)) {
                $attrs .= ' href="'.e($href).'"';
            }

            $target = trim((string) $node->getAttribute('target'));
            if ($target === '_blank') {
                $attrs .= ' target="_blank" rel="noopener noreferrer"';
            }

            $title = trim((string) $node->getAttribute('title'));
            if ($title !== '') {
                $attrs .= ' title="'.e($title).'"';
            }
        }

        return '<'.$tag.$attrs.'>'.self::renderChildren($node).'</'.$tag.'>';
    }

    private static function isAllowedHref(string $href): bool
    {
        if ($href === '') {
            return false;
        }

        if (str_starts_with($href, '/') || str_starts_with($href, '#')) {
            return true;
        }

        return preg_match('/^(https?:|mailto:)/i', $href) === 1;
    }
}