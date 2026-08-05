<?php

namespace App\Notifications;

use Illuminate\Bus\Queueable;
use Illuminate\Notifications\Messages\MailMessage;
use Illuminate\Notifications\Notification;

final class JobSeekerEmailVerificationNotification extends Notification
{
    use Queueable;

    public function __construct(private readonly string $verificationUrl) {}

    /**
     * @return list<string>
     */
    public function via(object $notifiable): array
    {
        return ['mail'];
    }

    public function toMail(object $notifiable): MailMessage
    {
        return (new MailMessage)
            ->subject('Confirm your updated email address')
            ->line('Your WorkBC Job Board account email was changed. Please confirm this new address to continue using it.')
            ->action('Confirm email address', $this->verificationUrl)
            ->line('If you did not request this change, contact support immediately.');
    }
}