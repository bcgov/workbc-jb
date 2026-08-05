<?php

namespace App\Http\Controllers;

use App\Search\Url\FilterUrlSerializer;
use Illuminate\Http\Request;
use Illuminate\Http\Response;
use Illuminate\Support\Facades\Redirect;

/**
 * SRCH-6 redirect shim for previously-sent alert emails.
 *
 * Old alert emails deep-link to the Angular app's `#/job-search{matrix}` route,
 * where the filters live in the URL hash as `;key=value` matrix params (see
 * LARAVEL-MIGRATION-PLAN risk #3). This controller decodes that legacy format
 * via {@see FilterUrlSerializer::fromLegacy()} and 302-redirects to the
 * canonical, crawlable `/jobs?…` search URL.
 *
 * Because a URL hash fragment is never sent to the server, links that still
 * carry their params in the hash first hit {@see landing()}, whose tiny script
 * copies the hash into a `p=` query param and reloads — after which this
 * controller can decode it server-side. Links whose params are already in the
 * query string (or a stored UrlParameters string passed as `p`) are redirected
 * immediately.
 */
final class LegacyAlertRedirectController extends Controller
{
    public function __invoke(Request $request, FilterUrlSerializer $serializer): Response|\Illuminate\Http\RedirectResponse
    {
        // A stored UrlParameters / hash string handed over as `p`.
        if (is_string($p = $request->query('p')) && trim($p) !== '') {
            $filters = $serializer->fromLegacy($p);

            return Redirect::to($serializer->toUrl($filters), 302);
        }

        // Legacy params supplied directly on the query string.
        $params = $request->query();
        unset($params['p']);
        if ($params !== []) {
            $filters = $serializer->fromLegacy($params);

            return Redirect::to($serializer->toUrl($filters), 302);
        }

        // No server-visible params: the filters are probably still in the URL
        // hash. Render the landing shim that forwards them back as `p=…`.
        return response()->view('jobs.alert-redirect');
    }
}
