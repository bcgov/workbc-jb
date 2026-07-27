<?php

declare(strict_types=1);

namespace WorkBC\JobAlertNotifier\Service;

use Monolog\Logger;
use WorkBC\JobAlertNotifier\Config\AppConfig;
use WorkBC\JobAlertNotifier\Email\EmailSenderInterface;

/**
 * Port of WorkBC.Notifications.JobAlerts.Services.JobAlertSenderService.
 *
 * Decides which alert frequencies are due today, matches each alert against
 * Elasticsearch, and emails every alert with at least one new job.
 */
final class JobAlertSenderService
{
    // AccountStatus enum
    private const ACCOUNT_STATUS_ACTIVE = 1;

    private string $subjectTemplate;
    private string $bodyTextTemplate;
    private string $bodyHtmlTemplate;

    public function __construct(
        private readonly \PDO $pdo,
        private readonly AppConfig $config,
        private readonly Logger $log,
        private readonly EmailSenderInterface $emailSender,
        private readonly JobAlertSearchService $searchService,
    ) {
        // The C# EmailSettings ctor reads these keys from the SystemSettings
        // dictionary and throws if any is missing — do the same.
        $settings = [];
        $stmt = $this->pdo->query(
            'SELECT "Name", "Value" FROM "SystemSettings" WHERE "Name" LIKE \'email.%\''
        );
        foreach ($stmt->fetchAll() as $row) {
            $settings[(string) $row['Name']] = (string) $row['Value'];
        }

        foreach (['email.jobAlert.subject', 'email.jobAlert.bodyText', 'email.jobAlert.bodyHtml'] as $key) {
            if (!array_key_exists($key, $settings)) {
                throw new \RuntimeException("SystemSettings is missing the '{$key}' entry");
            }
        }

        $this->subjectTemplate = $settings['email.jobAlert.subject'];
        $this->bodyTextTemplate = $settings['email.jobAlert.bodyText'];
        $this->bodyHtmlTemplate = $settings['email.jobAlert.bodyHtml'];
    }

    /**
     * Checks the date against the alert frequencies and sends the emails
     * that are due.
     */
    public function runJobAlertSender(): void
    {
        $emails = [];
        $jobAlerts = [];

        $now = new \DateTimeImmutable('now');

        if ((int) $now->format('N') === 1) {
            // weekly reports (Mondays)
            $this->log->info('Getting weekly job alerts');
            $jobAlerts = array_merge($jobAlerts, $this->getJobAlerts(JobAlertEmail::FREQUENCY_WEEKLY));
        }

        if ((int) $now->format('j') === 1) {
            // monthly reports
            $this->log->info('Getting monthly job alerts');
            $jobAlerts = array_merge($jobAlerts, $this->getJobAlerts(JobAlertEmail::FREQUENCY_MONTHLY));
        }

        if ((int) $now->format('j') === 15 || (int) $now->format('j') === (int) $now->format('t')) {
            // biweekly reports (15th and last day of the month)
            $this->log->info('Getting biweekly job alerts');
            $jobAlerts = array_merge($jobAlerts, $this->getJobAlerts(JobAlertEmail::FREQUENCY_BIWEEKLY));
        }

        // daily reports
        $this->log->info('Getting daily job alerts');
        $jobAlerts = array_merge($jobAlerts, $this->getJobAlerts(JobAlertEmail::FREQUENCY_DAILY));

        // get the job alerts out into their email format
        foreach ($jobAlerts as $jobAlert) {
            try {
                $resultCount = $this->searchService->getJobAlertSearchResultCount(
                    (string) $jobAlert['JobSearchFilters'],
                    (int) $jobAlert['AlertFrequency']
                );

                if ($resultCount > 0) {
                    $emails[] = new JobAlertEmail(
                        (int) $jobAlert['Id'],
                        (string) $jobAlert['Title'],
                        (int) $jobAlert['AlertFrequency'],
                        (string) $jobAlert['AspNetUserId'],
                        (string) $jobAlert['FirstName'],
                        (string) $jobAlert['Email'],
                        $this->config->jbSearchUrl,
                        $this->subjectTemplate,
                        $this->bodyTextTemplate,
                        $this->bodyHtmlTemplate
                    );
                }
            } catch (\Throwable $e) {
                $this->log->error("Failed to get jobs for job alert id: {$jobAlert['Id']}\n{$e}");
            }
        }

        $this->sendJobAlertEmails($emails);
    }

    /**
     * Sorts the outgoing emails by domain (to reuse connections per domain,
     * like the C# IComparable sort) and sends them one by one.
     *
     * @param list<JobAlertEmail> $emails
     */
    private function sendJobAlertEmails(array $emails): void
    {
        usort($emails, fn(JobAlertEmail $a, JobAlertEmail $b) => strcmp($a->emailDomain(), $b->emailDomain()));

        $testingEmail = $this->config->sendEmailTestingTo;
        $count = 0;

        foreach ($emails as $email) {
            $sendToEmail = '';
            try {
                $sendToEmail = $email->emailAddress;
                if (trim($testingEmail) !== '') {
                    $sendToEmail = $testingEmail;
                }

                if (filter_var($sendToEmail, FILTER_VALIDATE_EMAIL) === false) {
                    // The e-mail address is invalid.
                    // This is typically caused by the data being cleansed by ISB and replaced
                    // with a string of 'x' characters. Skipped silently (the C# FormatException
                    // path) so the logs don't fill up with these.
                    continue;
                }

                $this->emailSender->sendEmail(
                    $sendToEmail,
                    $email->emailSubject(),
                    $email->htmlMessage(),
                    $email->textMessage()
                );

                $this->log->info("Send email to:{$sendToEmail}");
                $count++;
            } catch (\Throwable $e) {
                $this->log->error("Error sending email to: {$sendToEmail}\n{$e}");
            }
        }

        $this->log->info("Sent {$count} job alert emails");
    }

    /**
     * Gets all the (active, non-deleted) job alerts in the system for a
     * frequency — the port of the EF JobAlerts + JobSeeker projection.
     *
     * @return list<array<string, mixed>>
     */
    private function getJobAlerts(int $frequency): array
    {
        $stmt = $this->pdo->prepare(
            'SELECT a."Id", a."AlertFrequency", a."AspNetUserId", a."JobSearchFilters",
                    a."JobSearchFiltersVersion", a."Title",
                    u."FirstName", u."Email"
             FROM "JobAlerts" a
             INNER JOIN "AspNetUsers" u ON u."Id" = a."AspNetUserId"
             WHERE a."AlertFrequency" = :frequency
               AND u."AccountStatus" = :active
               AND a."IsDeleted" = FALSE'
        );
        $stmt->execute([':frequency' => $frequency, ':active' => self::ACCOUNT_STATUS_ACTIVE]);

        return $stmt->fetchAll();
    }
}
