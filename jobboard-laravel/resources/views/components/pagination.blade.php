@props(['paginator'])

@php
    /** @var \Illuminate\Contracts\Pagination\LengthAwarePaginator $paginator */
    $linkBase = 'inline-flex min-w-9 items-center justify-center rounded-md px-3 py-2 text-sm font-medium '
        . 'focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900';
@endphp

@if ($paginator->hasPages())
    <nav role="navigation" aria-label="Pagination" class="flex flex-wrap items-center justify-between gap-3 border-t border-slate-200 pt-3">
        <p class="text-sm text-slate-700">
            Showing <span class="font-medium">{{ $paginator->firstItem() }}</span>&ndash;<span class="font-medium">{{ $paginator->lastItem() }}</span>
            of <span class="font-medium">{{ $paginator->total() }}</span>
        </p>

        <ul class="flex items-center gap-1">
            {{-- Previous --}}
            <li>
                @if ($paginator->onFirstPage())
                    <span aria-disabled="true" class="{{ $linkBase }} cursor-not-allowed text-slate-400">
                        <span aria-hidden="true">&lsaquo;</span>
                        <span class="sr-only">Previous page</span>
                    </span>
                @else
                    <a href="{{ $paginator->previousPageUrl() }}" rel="prev" aria-label="Go to previous page"
                       class="{{ $linkBase }} text-slate-700 hover:bg-slate-100">
                        <span aria-hidden="true">&lsaquo;</span>
                    </a>
                @endif
            </li>

            {{-- Numbered pages --}}
            @foreach ($paginator->getUrlRange(1, $paginator->lastPage()) as $page => $url)
                <li>
                    @if ($page == $paginator->currentPage())
                        {{-- aria-current marks the active page for AT (WCAG 4.1.2). --}}
                        <span aria-current="page" class="{{ $linkBase }} bg-blue-700 text-white">
                            <span class="sr-only">Page </span>{{ $page }}<span class="sr-only"> (current)</span>
                        </span>
                    @else
                        <a href="{{ $url }}" aria-label="Go to page {{ $page }}"
                           class="{{ $linkBase }} text-slate-700 hover:bg-slate-100">
                            {{ $page }}
                        </a>
                    @endif
                </li>
            @endforeach

            {{-- Next --}}
            <li>
                @if ($paginator->hasMorePages())
                    <a href="{{ $paginator->nextPageUrl() }}" rel="next" aria-label="Go to next page"
                       class="{{ $linkBase }} text-slate-700 hover:bg-slate-100">
                        <span aria-hidden="true">&rsaquo;</span>
                    </a>
                @else
                    <span aria-disabled="true" class="{{ $linkBase }} cursor-not-allowed text-slate-400">
                        <span aria-hidden="true">&rsaquo;</span>
                        <span class="sr-only">Next page</span>
                    </span>
                @endif
            </li>
        </ul>
    </nav>
@endif
