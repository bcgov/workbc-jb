<?php

declare(strict_types=1);

namespace WorkBC\JobAlertNotifier\Email;

use Monolog\Logger;
use PHPMailer\PHPMailer\PHPMailer;
use WorkBC\JobAlertNotifier\Config\AppConfig;

/**
 * Port of WorkBC.Shared.Services.SmtpEmailSender.
 *
 * Plain (unauthenticated, non-TLS) SMTP relay with a text body and an HTML
 * alternate view, matching the C# System.Net.Mail.SmtpClient usage.
 */
final class SmtpEmailSender implements EmailSenderInterface
{
    public function __construct(
        private readonly AppConfig $config,
        private readonly Logger $log,
    ) {
    }

    public function sendEmail(string $email, string $subject, string $htmlMessage, string $textMessage): void
    {
        if (trim($email) === '' || trim($subject) === '') {
            return;
        }

        $fromEmail = $this->config->emailFromEmail !== '' ? $this->config->emailFromEmail : 'noreply@gov.bc.ca';
        $fromName = $this->config->emailFromName !== '' ? $this->config->emailFromName : 'WorkBC.ca No Reply';

        try {
            $mailer = new PHPMailer(true);
            $mailer->isSMTP();
            $mailer->Host = $this->config->emailSmtpServer;
            $mailer->Port = $this->config->emailSmtpPort;
            $mailer->SMTPAuth = false;
            $mailer->SMTPAutoTLS = false;
            $mailer->CharSet = PHPMailer::CHARSET_UTF8;

            $mailer->setFrom($fromEmail, $fromName);
            $mailer->addAddress($email);
            $mailer->Subject = $subject;
            $mailer->isHTML(true);
            $mailer->Body = $htmlMessage;
            $mailer->AltBody = $textMessage;

            $mailer->send();
        } catch (\Throwable $e) {
            $this->log->error('SmtpEmailSender::sendEmail failed: ' . $e->getMessage());
            throw $e;
        }
    }
}
