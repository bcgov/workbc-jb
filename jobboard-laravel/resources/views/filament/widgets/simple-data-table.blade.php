<x-filament-widgets::widget>
    <x-filament::section>
        <div class="overflow-x-auto">
            <table class="fi-ta-table w-full text-start">
                <caption class="sr-only">{{ $caption }}</caption>
                <thead>
                    <tr>
                        @foreach ($columns as $column)
                            <th scope="col" class="px-3 py-2 text-start text-xs font-semibold uppercase tracking-wide text-gray-500 dark:text-gray-400">
                                {{ $column }}
                            </th>
                        @endforeach
                    </tr>
                </thead>
                <tbody>
                    @forelse ($rows as $row)
                        <tr class="border-t border-gray-100 dark:border-white/5">
                            @foreach ($row as $i => $cell)
                                @if ($i === 0)
                                    <th scope="row" class="px-3 py-2 text-start text-sm font-medium text-gray-950 dark:text-white">{{ $cell }}</th>
                                @else
                                    <td class="px-3 py-2 text-start text-sm tabular-nums text-gray-700 dark:text-gray-300">{{ $cell }}</td>
                                @endif
                            @endforeach
                        </tr>
                    @empty
                        <tr>
                            <td colspan="{{ count($columns) }}" class="px-3 py-2 text-sm text-gray-500 dark:text-gray-400">No data.</td>
                        </tr>
                    @endforelse
                </tbody>
            </table>
        </div>
    </x-filament::section>
</x-filament-widgets::widget>
