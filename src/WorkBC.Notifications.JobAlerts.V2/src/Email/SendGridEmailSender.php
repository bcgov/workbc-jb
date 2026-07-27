<?php

declare(strict_types=1);

namespace WorkBC\JobAlertNotifier\Email;

use GuzzleHttp\Client;
use Monolog\Logger;
use WorkBC\JobAlertNotifier\Config\AppConfig;

/**
 * Port of WorkBC.Shared.Services.SendGridEmailSender.
 *
 * Talks to the SendGrid v3 mail/send API directly (the C# SDK builds the
 * same request). Click tracking is disabled, matching msg.SetClickTracking(false, false).
 * Honours the forward proxy settings like the C# HttpClientHandler did.
 */
final class SendGridEmailSender implements EmailSenderInterface
{
    private Client $http;

    public function __construct(
        private readonly AppConfig $config,
        private readonly Logger $log,
    ) {
        $options = [
            'base_uri' => 'https://api.sendgrid.com/v3/',
            'timeout' => $config->httpTimeout,
        ];
        if (($proxy = $config->outboundProxy()) !== null) {
            $options['proxy'] = $proxy;
        }
        if ($config->proxyIgnoreSslErrors) {
            $options['verify'] = false;
        }
        $this->http = new Client($options);
    }

    public function sendEmail(string $email, string $subject, string $htmlMessage, string $textMessage): void
    {
        if (trim($email) === '' || trim($subject) === '') {
            return;
        }

        $fromEmail = $this->config->emailSendGridFromEmail !== ''
            ? $this->config->emailSendGridFromEmail
            : 'noreply@gov.bc.ca';
        $fromName = $this->config->emailFromName !== ''
            ? $this->config->emailFromName
            : 'WorkBC.ca No Reply';

        $payload = [
            'personalizations' => [
                ['to' => [['email' => $email]]],
            ],
            'from' => ['email' => $fromEmail, 'name' => $fromName],
            'subject' => $subject,
            'content' => [
                ['type' => 'text/plain', 'value' => $textMessage],
                ['type' => 'text/html', 'value' => $htmlMessage],
            ],
            'tracking_settings' => [
                'click_tracking' => ['enable' => false, 'enable_text' => false],
            ],
        ];

        $response = $this->http->post('mail/send', [
            'headers' => [
                'Authorization' => 'Bearer ' . $this->config->emailSendGridKey,
                'Content-Type' => 'application/json',
            ],
            'json' => $payload,
        ]);

        $this->log->debug('SendGrid response: HTTP ' . $response->getStatusCode());
    }
}
