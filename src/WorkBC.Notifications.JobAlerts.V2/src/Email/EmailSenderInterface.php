<?php

declare(strict_types=1);

namespace WorkBC\JobAlertNotifier\Email;

/** Port of WorkBC.Shared.Services.IEmailSender. */
interface EmailSenderInterface
{
    public function sendEmail(string $email, string $subject, string $htmlMessage, string $textMessage): void;
}
