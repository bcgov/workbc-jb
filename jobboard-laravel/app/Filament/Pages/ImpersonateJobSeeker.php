<?php

namespace App\Filament\Pages;

use App\Models\AdminUser;
use App\Models\Enums\AdminLevel;
use App\Models\JobSeeker;
use App\Services\Admin\ImpersonationService;
use Filament\Actions\Action;
use Filament\Forms\Components\TextInput;
use Filament\Notifications\Notification;
use Filament\Pages\Page;
use Filament\Support\Icons\Heroicon;
use Illuminate\Support\Facades\Auth;
use UnitEnum;

/**
 * ADM-4 scaffold (FND-6): the minimal, working entry point for admin
 * impersonation until ADM-2's Job Seeker resource exists to trigger it from a
 * table row instead. Looks a seeker up by email, then delegates to
 * {@see ImpersonationService} for the audit write + session switch.
 */
final class ImpersonateJobSeeker extends Page
{
    protected static string|\BackedEnum|null $navigationIcon = null;

    protected static string|UnitEnum|null $navigationGroup = 'Job seekers';

    protected static ?string $navigationLabel = 'Impersonate';

    protected string $view = 'filament.pages.impersonate-job-seeker';

    public static function canAccess(): bool
    {
        $admin = Auth::guard('admin')->user();

        return $admin instanceof AdminUser
            && in_array($admin->AdminLevel, [AdminLevel::Admin, AdminLevel::SuperAdmin], true);
    }

    public function getTitle(): string
    {
        return 'Impersonate a job seeker';
    }

    /**
     * @return array<Action>
     */
    protected function getHeaderActions(): array
    {
        return [
            Action::make('impersonate')
                ->label('Impersonate a job seeker')
                ->icon(Heroicon::UserCircle)
                ->form([
                    TextInput::make('email')
                        ->label('Job seeker email')
                        ->email()
                        ->required()
                        ->autofocus(),
                ])
                ->action(fn (array $data) => $this->startImpersonation((string) $data['email'])),
        ];
    }

    private function startImpersonation(string $email): void
    {
        $seeker = JobSeeker::query()
            ->where('NormalizedEmail', mb_strtoupper(trim($email), 'UTF-8'))
            ->orWhere('Email', $email)
            ->first();

        if ($seeker === null) {
            Notification::make()
                ->title('No job seeker found for that email.')
                ->danger()
                ->send();

            return;
        }

        /** @var AdminUser $admin */
        $admin = Auth::guard('admin')->user();

        app(ImpersonationService::class)->start($admin, $seeker);

        $this->redirect(route('account.dashboard'));
    }
}
