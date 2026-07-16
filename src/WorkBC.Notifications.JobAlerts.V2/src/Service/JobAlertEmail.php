<?php

declare(strict_types=1);

namespace WorkBC\JobAlertNotifier\Service;

/**
 * Port of WorkBC.Notifications.JobAlerts.Services.JobAlertEmail.
 *
 * Holds one alert's outgoing message. Subject/body templates come from the
 * SystemSettings table (email.jobAlert.*) and use C# string.Format
 * placeholders:
 *   subject:  {0} = alert title
 *   bodyText: {0} = first name, {1} = frequency, {2} = alert title, {3} = url
 *   bodyHtml: {0} = first name, {1} = frequency, {2} = alert title, {3} = url, {4} = subject
 */
final class JobAlertEmail
{
    // JobAlertFrequency enum values
    public const FREQUENCY_DAILY = 1;
    public const FREQUENCY_WEEKLY = 2;
    public const FREQUENCY_BIWEEKLY = 3;
    public const FREQUENCY_MONTHLY = 4;

    public function __construct(
        public readonly int $jobAlertId,
        public readonly string $jobAlertTitle,
        public readonly int $alertFrequency,
        public readonly string $jobSeekerId,
        public readonly string $firstName,
        public readonly string $emailAddress,
        private readonly string $jbSearchUrl,
        private readonly string $subjectTemplate,
        private readonly string $bodyTextTemplate,
        private readonly string $bodyHtmlTemplate,
    ) {
    }

    public function emailSubject(): string
    {
        return self::format($this->subjectTemplate, [$this->jobAlertTitle]);
    }

    private function notificationUrl(): string
    {
        return "{$this->jbSearchUrl}#/job-search/r;nid={$this->jobAlertId};jsid={$this->jobSeekerId}";
    }

    public function textMessage(): string
    {
        return self::format($this->bodyTextTemplate, [
            $this->firstName,
            $this->frequencyString(),
            $this->jobAlertTitle,
            $this->notificationUrl(),
        ]);
    }

    public function htmlMessage(): string
    {
        return self::format($this->bodyHtmlTemplate, [
            $this->firstName,
            $this->frequencyString(),
            $this->jobAlertTitle,
            $this->notificationUrl(),
            $this->emailSubject(),
        ]);
    }

    /** Emails are sorted by domain so consecutive sends share connections. */
    public function emailDomain(): string
    {
        $at = strpos($this->emailAddress, '@');
        return substr($this->emailAddress, ($at === false ? -1 : $at) + 1);
    }

    private function frequencyString(): string
    {
        return match ($this->alertFrequency) {
            self::FREQUENCY_DAILY => 'daily',
            self::FREQUENCY_WEEKLY => 'weekly',
            self::FREQUENCY_BIWEEKLY => 'biweekly',
            self::FREQUENCY_MONTHLY => 'monthly',
            default => 'unscheduled',
        };
    }

    /**
     * C# string.Format semantics: "{{" → "{", "}}" → "}", "{N}" → args[N]
     * (out-of-range indexes throw, like FormatException).
     *
     * @param list<string> $args
     */
    private static function format(string $template, array $args): string
    {
        return preg_replace_callback(
            '/\{\{|\}\}|\{(\d+)\}/',
            static function (array $m) use ($args): string {
                if ($m[0] === '{{') {
                    return '{';
                }
                if ($m[0] === '}}') {
                    return '}';
                }
                $index = (int) $m[1];
                if (!array_key_exists($index, $args)) {
                    throw new \InvalidArgumentException("Template placeholder {{$index}} has no argument");
                }
                return $args[$index];
            },
            $template
        );
    }
}
