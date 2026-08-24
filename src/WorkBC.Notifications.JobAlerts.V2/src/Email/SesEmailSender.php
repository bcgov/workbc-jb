<?php

declare(strict_types=1);

namespace WorkBC\JobAlertNotifier\Email;

use Aws\Ses\SesClient;
use Monolog\Logger;
use WorkBC\JobAlertNotifier\Config\AppConfig;

/**
 * Port of WorkBC.Shared.Services.AmazonSesEmailSender.
 *
 * Uses the AWS SDK default credential chain (env vars / IRSA web identity /
 * instance role), exactly like the C# AmazonSimpleEmailServiceClient did in
 * the EKS pods.
 */
final class SesEmailSender implements EmailSenderInterface
{
    private SesClient $client;

    public function __construct(
        private readonly AppConfig $config,
        private readonly Logger $log,
    ) {
        // The C# sender built a fresh client (and TLS connection) per email,
        // which kept it under the SES max send rate by accident. This client
        // is reused with keep-alive, so bursts hit Throttling errors —
        // adaptive retry adds a client-side rate limiter that backs off to
        // the accepted rate instead of dropping the email after the default
        // three attempts.
        $this->client = new SesClient([
            'version' => '2010-12-01',
            'region' => $config->emailSesRegion,
            'retries' => [
                'mode' => 'adaptive',
                'max_attempts' => 10,
            ],
        ]);
    }

    public function sendEmail(string $email, string $subject, string $htmlMessage, string $textMessage): void
    {
        if (trim($email) === '' || trim($subject) === '') {
            return;
        }

        $fromEmail = $this->config->emailFromEmail !== '' ? $this->config->emailFromEmail : 'noreply@workbc.ca';
        $fromName = $this->config->emailFromName !== '' ? $this->config->emailFromName : 'WorkBC.ca No Reply';

        $result = $this->client->sendEmail([
            'Source' => "{$fromName} <{$fromEmail}>",
            'Destination' => ['ToAddresses' => [$email]],
            'Message' => [
                'Subject' => ['Data' => $subject],
                'Body' => [
                    'Text' => ['Data' => $textMessage],
                    'Html' => ['Data' => $htmlMessage],
                ],
            ],
        ]);

        $this->log->debug('SES MessageId: ' . ($result['MessageId'] ?? 'unknown'));
    }
}
