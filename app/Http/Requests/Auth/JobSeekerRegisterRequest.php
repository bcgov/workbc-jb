<?php

namespace App\Http\Requests\Auth;

use Illuminate\Foundation\Http\FormRequest;

final class JobSeekerRegisterRequest extends FormRequest
{
    public function authorize(): bool
    {
        return true;
    }

    /**
     * @return array<string, mixed>
     */
    public function rules(): array
    {
        return [
            'email' => ['required', 'email:rfc', 'max:256'],
            'username' => ['nullable', 'email:rfc', 'max:256'],
            'password' => ['required', 'string', 'confirmed', 'min:8'],
        ];
    }
}
