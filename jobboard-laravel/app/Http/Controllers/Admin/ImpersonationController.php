<?php

namespace App\Http\Controllers\Admin;

use App\Http\Controllers\Controller;
use App\Services\Admin\ImpersonationService;
use Illuminate\Http\RedirectResponse;

final class ImpersonationController extends Controller
{
    public function __construct(private readonly ImpersonationService $impersonationService) {}

    public function end(): RedirectResponse
    {
        $this->impersonationService->end();

        return redirect('/admin');
    }
}
