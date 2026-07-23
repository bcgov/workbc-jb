{{--
    SRCH-9 — Google Maps view partial.

    The Google Maps JavaScript API is wrapped in a single Alpine component
    (`jobMap`) — Alpine owns this browser-only view behaviour while Livewire owns
    the pin DATA (built server-side from the same filters). The Maps API key is
    read from config('services.google_maps.js_key') and passed in; it is NEVER
    hardcoded (production supplies it from Secrets Manager via the environment).

    a11y: this map is supplementary. The list view remains a full, always-available
    equivalent (the "List"/"Map" toggle above), so results are reachable without
    the map. Loading/empty/error states are announced via a polite live region.
--}}

@assets
<script>
    document.addEventListener('alpine:init', () => {
        Alpine.data('jobMap', (config) => ({
            status: 'loading', // loading | ready | empty | error
            pins: [],
            map: null,
            infoWindow: null,

            init() {
                // Pins are read from a JSON <script> block (see the markup below)
                // rather than an attribute, so their detail URLs stay unescaped.
                this.pins = this.readPins();

                if (this.pins.length === 0) {
                    this.status = 'empty';
                    return;
                }
                if (!config.apiKey) {
                    this.status = 'error';
                    return;
                }
                this.loadMaps(config.apiKey)
                    .then(() => this.loadClusterer())
                    .then(() => this.draw())
                    .catch(() => { this.status = 'error'; });
            },

            readPins() {
                const el = this.$refs.pins;
                if (!el) { return []; }
                try {
                    const parsed = JSON.parse(el.textContent || '[]');
                    return Array.isArray(parsed) ? parsed : [];
                } catch (e) {
                    return [];
                }
            },

            // Load the Maps JS API once per page via its async callback bootstrap.
            loadMaps(apiKey) {
                return new Promise((resolve, reject) => {
                    if (window.google && window.google.maps) { resolve(); return; }
                    if (window.__jobMapMapsPromise) { window.__jobMapMapsPromise.then(resolve, reject); return; }
                    window.__jobMapMapsPromise = new Promise((res, rej) => {
                        window.__jobMapMapsReady = res;
                        const s = document.createElement('script');
                        s.src = 'https://maps.googleapis.com/maps/api/js?key='
                            + encodeURIComponent(apiKey) + '&callback=__jobMapMapsReady&loading=async';
                        s.async = true;
                        s.onerror = rej;
                        document.head.appendChild(s);
                    });
                    window.__jobMapMapsPromise.then(resolve, reject);
                });
            },

            // Marker clustering is provided by the official library; if it fails
            // to load we still plot the markers individually (graceful degrade).
            loadClusterer() {
                return new Promise((resolve) => {
                    if (window.markerClusterer) { resolve(); return; }
                    const s = document.createElement('script');
                    s.src = 'https://unpkg.com/@googlemaps/markerclusterer/dist/index.min.js';
                    s.async = true;
                    s.onload = () => resolve();
                    s.onerror = () => resolve();
                    document.head.appendChild(s);
                });
            },

            draw() {
                const canvas = this.$refs.canvas;
                // Centre on British Columbia; fitBounds refines to the pins below.
                this.map = new google.maps.Map(canvas, {
                    center: { lat: 53.7267, lng: -127.6476 },
                    zoom: 5,
                    mapTypeControl: false,
                    streetViewControl: false,
                });
                this.infoWindow = new google.maps.InfoWindow();

                const bounds = new google.maps.LatLngBounds();
                const markers = this.pins.map((pin) => {
                    const position = { lat: pin.lat, lng: pin.lng };
                    bounds.extend(position);
                    const marker = new google.maps.Marker({ position, title: pin.title || '' });
                    marker.addListener('click', () => this.openInfoWindow(marker, pin));
                    return marker;
                });

                if (markers.length) {
                    this.map.fitBounds(bounds);
                }

                if (window.markerClusterer && window.markerClusterer.MarkerClusterer) {
                    new window.markerClusterer.MarkerClusterer({ map: this.map, markers });
                } else {
                    markers.forEach((m) => m.setMap(this.map));
                }

                this.status = 'ready';
            },

            // Build the info-window content with DOM nodes (textContent), never
            // innerHTML — the title comes from the index and must not be able to
            // inject markup (XSS-safe).
            openInfoWindow(marker, pin) {
                const wrap = document.createElement('div');
                wrap.className = 'space-y-1';

                const link = document.createElement('a');
                link.href = pin.url;
                link.textContent = pin.title && pin.title.length ? pin.title : 'View job details';
                link.className = 'font-semibold text-blue-800 underline';
                wrap.appendChild(link);

                const cta = document.createElement('div');
                const view = document.createElement('a');
                view.href = pin.url;
                view.textContent = 'View job details';
                view.className = 'text-sm text-blue-800 underline';
                cta.appendChild(view);
                wrap.appendChild(cta);

                this.infoWindow.setContent(wrap);
                this.infoWindow.open(this.map, marker);
            },
        }));
    });
</script>
@endassets

{{-- A fresh wire:key per pin set forces Livewire to replace the wrapper when the
     filters change, so Alpine re-inits the map with the new pins. --}}
<div wire:key="job-map-{{ md5(json_encode($mapPins)) }}"
     x-data="jobMap({ apiKey: @js($mapApiKey) })"
     role="region"
     aria-label="Job locations map">

    {{-- Status/announcements for assistive tech (the map canvas itself is not
         screen-reader friendly; the list view is the accessible equivalent). --}}
    <p role="status" aria-live="polite" class="sr-only" x-text="{
        loading: 'Loading the map of job locations.',
        ready: 'Map of job locations loaded. Use the List view for a full, accessible list of results.',
        empty: 'No mappable job locations for this search.',
        error: 'The map could not be loaded.'
    }[status]"></p>

    <div class="rounded-lg border border-slate-200 bg-white p-3">
        <p class="mb-3 text-sm text-slate-700">
            Showing up to {{ number_format(\App\Search\Support\MapPins::MAX_PINS) }} job locations for your search.
            Prefer a list? Use the <strong>List</strong> view above &mdash; it has every result and works with a keyboard and screen reader.
        </p>

        {{-- Empty state (no geo-located jobs). --}}
        @if (empty($mapPins))
            <div class="flex min-h-[24rem] items-center justify-center rounded-md bg-slate-50 p-6 text-center">
                <p class="text-sm text-slate-700">
                    None of these jobs have a location to plot. Switch to the <strong>List</strong> view to see the results.
                </p>
            </div>
        @elseif (empty($mapApiKey))
            {{-- Config/Secrets Manager didn't supply a browser Maps key. --}}
            <div class="flex min-h-[24rem] items-center justify-center rounded-md bg-slate-50 p-6 text-center">
                <p class="text-sm text-slate-700">
                    The map is unavailable right now. Switch to the <strong>List</strong> view to see the results.
                </p>
            </div>
        @else
            {{-- Pin data for the Alpine component. Embedded as JSON (not an
                 attribute) with unescaped slashes so each pin's real detail URL
                 is preserved; JSON_HEX_TAG/AMP keep it safe inside <script>. --}}
            <script type="application/json" x-ref="pins">{!! json_encode($mapPins, JSON_UNESCAPED_SLASHES | JSON_HEX_TAG | JSON_HEX_AMP | JSON_HEX_APOS | JSON_HEX_QUOT) !!}</script>

            {{-- Loading / error overlays (shown until the Alpine component draws). --}}
            <div class="relative">
                <div x-show="status === 'loading'" x-cloak
                     class="absolute inset-0 z-10 flex items-center justify-center rounded-md bg-slate-50/80">
                    <p class="text-sm font-medium text-slate-700">Loading map&hellip;</p>
                </div>
                <div x-show="status === 'error'" x-cloak
                     class="absolute inset-0 z-10 flex items-center justify-center rounded-md bg-slate-50 p-6 text-center">
                    <p class="text-sm text-slate-700">
                        The map could not be loaded. Switch to the <strong>List</strong> view to see the results.
                    </p>
                </div>

                {{-- Google Maps manages this element's DOM directly, so keep
                     Livewire's DOM-morphing away from it. --}}
                <div x-ref="canvas" wire:ignore
                     class="h-[32rem] w-full overflow-hidden rounded-md bg-slate-100"
                     aria-hidden="true"></div>
            </div>
        @endif
    </div>
</div>
