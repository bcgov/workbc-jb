<?php

namespace App\Livewire;

use App\Models\JobSeeker;
use App\Services\JobSeeker\JobSeekerDashboardService;
use App\Services\Settings\SystemSettingsService;
use App\Support\SafeRichText;
use Illuminate\Support\Facades\Auth;
use Livewire\Attributes\Layout;
use Livewire\Attributes\Title;
use Livewire\Component;

#[Layout('components.layouts.app')]
#[Title('My account — WorkBC Job Board')]
final class JobSeekerDashboard extends Component
{
    private const SETTINGS_PREFIX = 'jbAccount.dashboard.';

    private const SETTINGS_KEYS = [
        'introText',
        'jobsDescription',
        'careersDescription',
        'accountDescription',
        'newAccountMessageTitle',
        'newAccountMessageBody',
        'notification1Title',
        'notification1Body',
        'notification1Enabled',
        'notification2Title',
        'notification2Body',
        'notification2Enabled',
        'resource1Title',
        'resource1Body',
        'resource1Url',
        'resource2Title',
        'resource2Body',
        'resource2Url',
        'resource3Title',
        'resource3Body',
        'resource3Url',
    ];

    public int $savedJobs = 0;

    public int $recommendedJobs = 0;

    public int $activeAlerts = 0;

    public int $savedCareerProfiles = 0;

    public int $savedIndustryProfiles = 0;

    public string $introTextHtml = '';

    public string $jobsDescriptionHtml = '';

    public string $careersDescriptionHtml = '';

    public string $accountDescriptionHtml = '';

    /** @var array{title:string,bodyHtml:string}|null */
    public ?array $welcomeMessage = null;

    /** @var list<array{title:string,bodyHtml:string}> */
    public array $notifications = [];

    /** @var list<array{title:string,bodyHtml:string,url:string}> */
    public array $resources = [];

    public function mount(JobSeekerDashboardService $dashboardService, SystemSettingsService $settings): void
    {
        $this->refreshCounts($dashboardService);
        $this->loadCopy($settings);
    }

    public function refreshCounts(JobSeekerDashboardService $dashboardService): void
    {
        /** @var JobSeeker $jobSeeker */
        $jobSeeker = Auth::guard('web')->user();
        $summary = $dashboardService->summaryFor($jobSeeker);

        $this->savedJobs = $summary['savedJobs'];
        $this->recommendedJobs = $summary['recommendedJobs'];
        $this->activeAlerts = $summary['activeAlerts'];
        $this->savedCareerProfiles = $summary['savedCareerProfiles'];
        $this->savedIndustryProfiles = $summary['savedIndustryProfiles'];
    }

    public function render()
    {
        return view('livewire.job-seeker-dashboard');
    }

    private function loadCopy(SystemSettingsService $settings): void
    {
        $settingsByName = $settings->getMany($this->requestedSettingNames());

        $this->introTextHtml = SafeRichText::sanitize($this->setting($settingsByName, 'introText'));
        $this->jobsDescriptionHtml = SafeRichText::sanitize($this->setting($settingsByName, 'jobsDescription'));
        $this->careersDescriptionHtml = SafeRichText::sanitize($this->setting($settingsByName, 'careersDescription'));
        $this->accountDescriptionHtml = SafeRichText::sanitize($this->setting($settingsByName, 'accountDescription'));

        $welcomeTitle = trim((string) ($this->setting($settingsByName, 'newAccountMessageTitle') ?? ''));
        $welcomeBodyHtml = SafeRichText::sanitize($this->setting($settingsByName, 'newAccountMessageBody'));

        $this->welcomeMessage = ($welcomeTitle !== '' || $welcomeBodyHtml !== '')
            ? [
                'title' => $welcomeTitle,
                'bodyHtml' => $welcomeBodyHtml,
            ]
            : null;

        $this->notifications = [];
        foreach ([1, 2] as $slot) {
            if ($this->setting($settingsByName, 'notification'.$slot.'Enabled') !== '1') {
                continue;
            }

            $title = trim((string) ($this->setting($settingsByName, 'notification'.$slot.'Title') ?? ''));
            $bodyHtml = SafeRichText::sanitize($this->setting($settingsByName, 'notification'.$slot.'Body'));

            if ($title === '' && $bodyHtml === '') {
                continue;
            }

            $this->notifications[] = [
                'title' => $title,
                'bodyHtml' => $bodyHtml,
            ];
        }

        $this->resources = [];
        foreach ([1, 2, 3] as $slot) {
            $title = trim((string) ($this->setting($settingsByName, 'resource'.$slot.'Title') ?? ''));
            $bodyHtml = SafeRichText::sanitize($this->setting($settingsByName, 'resource'.$slot.'Body'));
            $url = $this->resourceUrl($this->setting($settingsByName, 'resource'.$slot.'Url'));

            if ($title === '' && $bodyHtml === '' && $url === '') {
                continue;
            }

            $this->resources[] = [
                'title' => $title,
                'bodyHtml' => $bodyHtml,
                'url' => $url,
            ];
        }
    }

    /**
     * @return list<string>
     */
    private function requestedSettingNames(): array
    {
        $names = [];

        foreach (self::SETTINGS_KEYS as $name) {
            $names[] = self::SETTINGS_PREFIX.$name;
            $names[] = $name;
        }

        return $names;
    }

    /**
     * @param  array<string, string|null>  $settingsByName
     */
    private function setting(array $settingsByName, string $key): ?string
    {
        foreach ([self::SETTINGS_PREFIX.$key, $key] as $candidate) {
            if (array_key_exists($candidate, $settingsByName)) {
                return $settingsByName[$candidate];
            }
        }

        return null;
    }

    private function resourceUrl(?string $url): string
    {
        $value = trim((string) $url);

        if ($value === '') {
            return '';
        }

        if (preg_match('/^https?:\/\//i', $value) === 1) {
            return $value;
        }

        $base = rtrim((string) config('services.workbc.base_url', 'https://www.workbc.ca'), '/');

        if (str_starts_with($value, '/')) {
            return $base.$value;
        }

        return $base.'/'.$value;
    }
}
