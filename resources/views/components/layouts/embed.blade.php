{{--
    FND-4 / ADR-006 — the chrome-less layout served inside the Drupal iframe.

    Deliberately a separate file from app.blade.php rather than a conditional:
    what an embedded page emits is exactly what is in here, which is the thing
    the Drupal team needs to be able to read. The <head> is shared so the two
    cannot drift.

    No WorkBC header, nav or footer — the Drupal host page renders all of that.
    Emitting ours too would stack two sets of site chrome inside one page.

    Framing is permitted by the `frame-ancestors` CSP from SecurityHeaders
    middleware, driven by config/embed.php. X-Frame-Options is never sent.
--}}
<!DOCTYPE html>
<html lang="{{ str_replace('_', '-', app()->getLocale()) }}" class="h-full">
<head>
    <x-layouts.partials.head :title="$title ?? null" :description="$description ?? null">
        <x-slot:head>{{ $head ?? '' }}</x-slot:head>
    </x-layouts.partials.head>
</head>
{{-- No min-h-full/flex here: the body must be only as tall as its content, or
     the height we report to the parent would be the viewport rather than the
     page, and the frame would never shrink back after a shorter result set. --}}
<body class="bg-white text-slate-900 antialiased">
    {{-- Still a landmark, and still the skip-link target: assistive tech treats
         the framed document as its own document. --}}
    <main id="main" tabindex="-1" class="w-full px-4 py-4 focus:outline-none">
        {{ $slot }}
    </main>

    @php
        /** @var list<string> $embedParentOrigins */
        $embedParentOrigins = config('embed.parent_origins', []);
    @endphp

    @if ($embedParentOrigins !== [])
        <script>
            // Content-height bridge. The parent frame has no way to know how tall
            // this document is, so without this the iframe stays a fixed box with
            // its own inner scrollbar.
            //
            // A ResizeObserver on <body> is used rather than hooking Livewire's
            // lifecycle: it catches initial load, viewport resize, filter changes,
            // and any future DOM mutation without depending on framework internals.
            (function () {
                var origins = @js($embedParentOrigins);
                var last = 0;

                function postHeight() {
                    var height = Math.ceil(document.body.getBoundingClientRect().height);
                    // Skip sub-pixel jitter, which would otherwise post on every frame.
                    if (Math.abs(height - last) < 2) { return; }
                    last = height;
                    // One explicit target per allowed parent — never '*', which
                    // would leak page dimensions to any site that framed us. The
                    // browser delivers only to the origin that actually matches.
                    origins.forEach(function (origin) {
                        parent.postMessage({ type: 'jobboard:height', height: height }, origin);
                    });
                }

                if (window.ResizeObserver) {
                    new ResizeObserver(postHeight).observe(document.body);
                } else {
                    window.addEventListener('resize', postHeight);
                }

                window.addEventListener('load', postHeight);
                document.addEventListener('livewire:navigated', postHeight);
                postHeight();
            })();
        </script>
    @endif
</body>
</html>
