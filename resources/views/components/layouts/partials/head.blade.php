{{-- Shared <head> for both the full layout and the FND-4 embed layout, so the
     two cannot drift apart. Anything page-specific (canonical, hreflang,
     JSON-LD) arrives via the $head slot. --}}
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<meta name="color-scheme" content="light">
<title>{{ $title ?? 'WorkBC Job Board' }}</title>
@isset($description)
    <meta name="description" content="{{ $description }}">
@endisset
{{ $head ?? '' }}
@vite(['resources/css/app.css', 'resources/js/app.js'])
