<?php

namespace Tests\Feature\Ui;

use Illuminate\Pagination\LengthAwarePaginator;
use Illuminate\Support\Facades\Blade;
use Tests\TestCase;

class ComponentLibraryTest extends TestCase
{
    public function test_button_renders_native_button_by_default(): void
    {
        $html = Blade::render('<x-button type="submit">Save</x-button>');

        $this->assertStringContainsString('<button', $html);
        $this->assertStringContainsString('type="submit"', $html);
        $this->assertStringContainsString('Save', $html);
        $this->assertStringContainsString('focus-visible:outline', $html);
    }

    public function test_button_with_href_renders_anchor(): void
    {
        $html = Blade::render('<x-button href="/jobs">Browse</x-button>');

        $this->assertStringContainsString('<a', $html);
        $this->assertStringContainsString('href="/jobs"', $html);
    }

    public function test_disabled_link_button_exposes_disabled_state(): void
    {
        $html = Blade::render('<x-button href="/jobs" :disabled="true">Browse</x-button>');

        $this->assertStringContainsString('aria-disabled="true"', $html);
        $this->assertStringNotContainsString('href="/jobs"', $html);
    }

    public function test_disabled_button_sets_disabled_attribute(): void
    {
        $html = Blade::render('<x-button :disabled="true">Save</x-button>');

        $this->assertStringContainsString('disabled', $html);
    }

    public function test_form_field_associates_label_with_input(): void
    {
        $html = Blade::render('<x-form-field name="email" label="Email address" />');

        $this->assertStringContainsString('<label for="email"', $html);
        $this->assertStringContainsString('id="email"', $html);
        $this->assertStringContainsString('name="email"', $html);
        $this->assertStringContainsString('Email address', $html);
    }

    public function test_form_field_required_is_announced(): void
    {
        $html = Blade::render('<x-form-field name="name" label="Name" :required="true" />');

        $this->assertStringContainsString('aria-required="true"', $html);
        $this->assertStringContainsString('(required)', $html);
    }

    public function test_form_field_error_wires_aria_and_is_not_colour_only(): void
    {
        $html = Blade::render('<x-form-field name="pc" label="Postal code" error="Invalid postal code." />');

        $this->assertStringContainsString('aria-invalid="true"', $html);
        $this->assertStringContainsString('aria-describedby="pc-error"', $html);
        $this->assertStringContainsString('id="pc-error"', $html);
        $this->assertStringContainsString('role="alert"', $html);
        // Icon accompanies the error text so colour is not the sole signal.
        $this->assertStringContainsString('<svg', $html);
        $this->assertStringContainsString('Invalid postal code.', $html);
    }

    public function test_form_field_hint_and_error_are_both_described_by(): void
    {
        $html = Blade::render('<x-form-field name="pc" label="Postal code" hint="Format V6B 1A1" error="Required." />');

        $this->assertStringContainsString('aria-describedby="pc-hint pc-error"', $html);
        $this->assertStringContainsString('id="pc-hint"', $html);
    }

    public function test_error_alert_uses_assertive_role_and_hidden_label(): void
    {
        $html = Blade::render('<x-alert type="error">Boom</x-alert>');

        $this->assertStringContainsString('role="alert"', $html);
        $this->assertStringContainsString('Error:', $html);
        $this->assertStringContainsString('<svg', $html);
    }

    public function test_info_alert_uses_polite_status_role(): void
    {
        $html = Blade::render('<x-alert type="info">FYI</x-alert>');

        $this->assertStringContainsString('role="status"', $html);
        $this->assertStringContainsString('Information:', $html);
    }

    public function test_dismissible_alert_uses_alpine_view_state(): void
    {
        $html = Blade::render('<x-alert type="warning" :dismissible="true">Careful</x-alert>');

        $this->assertStringContainsString('x-data', $html);
        $this->assertStringContainsString('aria-label="Dismiss this message"', $html);
    }

    public function test_pagination_is_an_accessible_landmark(): void
    {
        $html = Blade::render('<x-pagination :paginator="$paginator" />', [
            'paginator' => $this->makePaginator(currentPage: 2),
        ]);

        $this->assertStringContainsString('aria-label="Pagination"', $html);
        $this->assertStringContainsString('aria-current="page"', $html);
        $this->assertStringContainsString('aria-label="Go to previous page"', $html);
        $this->assertStringContainsString('aria-label="Go to next page"', $html);
    }

    public function test_pagination_disables_previous_on_first_page(): void
    {
        $html = Blade::render('<x-pagination :paginator="$paginator" />', [
            'paginator' => $this->makePaginator(currentPage: 1),
        ]);

        $this->assertStringContainsString('aria-disabled="true"', $html);
        $this->assertStringContainsString('Previous page', $html);
    }

    private function makePaginator(int $currentPage): LengthAwarePaginator
    {
        $items = collect(range(1, 47));
        $perPage = 10;

        return new LengthAwarePaginator(
            $items->forPage($currentPage, $perPage)->values(),
            $items->count(),
            $perPage,
            $currentPage,
            ['path' => 'http://localhost/ui']
        );
    }
}
