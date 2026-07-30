import './bootstrap';

/**
 * SRCH-9 — marker clustering for the job map.
 *
 * Bundled and served from our own origin. It was previously injected at
 * runtime from `https://unpkg.com/@googlemaps/markerclusterer/...` with no
 * version pin and no SRI hash, so the code executing on the search page (which
 * is framed inside WorkBC.ca) could change without a deploy on our side.
 *
 * `import()` rather than a static import keeps it in its own Vite chunk: the
 * library plus its supercluster/fast-equals dependencies only download when a
 * user actually opens the map view, not on every page.
 *
 * Callers must tolerate rejection — the map degrades to individually plotted
 * markers if this fails (see resources/views/livewire/partials/job-map.blade.php).
 */
window.loadMarkerClusterer = async () => {
    const { MarkerClusterer } = await import('@googlemaps/markerclusterer');

    return MarkerClusterer;
};
