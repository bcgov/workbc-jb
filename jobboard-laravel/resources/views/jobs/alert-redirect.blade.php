<x-layouts.app title="Redirecting to your search — WorkBC Job Board">
    <div class="space-y-4" role="status" aria-live="polite">
        <h1 class="text-3xl font-bold">Taking you to your search…</h1>
        <p>
            We’re updating your saved-search link to the new Job Board. If you are not
            redirected automatically,
            <a href="{{ route('jobs.index') }}"
               class="text-blue-800 hover:underline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900">
                continue to job search
            </a>.
        </p>
    </div>

    {{--
        SRCH-6 hash shim: old alert emails keep their filters in the URL hash
        (e.g. #/job-search;search=nurse;noc=31301), which the server never sees.
        Copy the hash into a server-visible `p=` param so the controller can
        decode it, or fall back to the canonical search page.
    --}}
    <script>
        (function () {
            var hash = window.location.hash || '';
            var target;
            if (hash.length > 1) {
                target = '{{ route('jobs.alert-redirect') }}?p=' + encodeURIComponent(hash);
            } else {
                target = '{{ route('jobs.index') }}';
            }
            window.location.replace(target);
        })();
    </script>
</x-layouts.app>
