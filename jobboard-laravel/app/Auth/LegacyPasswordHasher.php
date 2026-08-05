<?php

namespace App\Auth;

use Illuminate\Support\Facades\Hash;

final class LegacyPasswordHasher
{
    public function verify(string $hashedPassword, string $providedPassword): LegacyHashVerificationResult
    {
        if ($hashedPassword === '' || $providedPassword === '') {
            return LegacyHashVerificationResult::Failed;
        }

        // After first successful legacy login we store modern Laravel hashes.
        if ($this->isLaravelHash($hashedPassword)) {
            if (! Hash::check($providedPassword, $hashedPassword)) {
                return LegacyHashVerificationResult::Failed;
            }

            return Hash::needsRehash($hashedPassword)
                ? LegacyHashVerificationResult::VerifiedNeedsRehash
                : LegacyHashVerificationResult::Verified;
        }

        $decoded = base64_decode($hashedPassword, true);
        if ($decoded === false || $decoded === '') {
            return LegacyHashVerificationResult::ForceReset;
        }

        $marker = ord($decoded[0]);

        return match ($marker) {
            0x01 => $this->verifyV3($decoded, $providedPassword),
            0x00 => $this->verifyV2($decoded, $providedPassword),
            0xF0 => $this->verifyMd5Marker($decoded, $providedPassword),
            default => LegacyHashVerificationResult::ForceReset,
        };
    }

    private function verifyV3(string $decoded, string $providedPassword): LegacyHashVerificationResult
    {
        if (strlen($decoded) < 13) {
            return LegacyHashVerificationResult::ForceReset;
        }

        $prf = $this->readNetworkUInt32(substr($decoded, 1, 4));
        $iterations = $this->readNetworkUInt32(substr($decoded, 5, 4));
        $saltLength = $this->readNetworkUInt32(substr($decoded, 9, 4));

        if ($iterations <= 0 || $saltLength <= 0) {
            return LegacyHashVerificationResult::ForceReset;
        }

        $payloadLength = strlen($decoded) - 13;
        if ($saltLength > $payloadLength) {
            return LegacyHashVerificationResult::ForceReset;
        }

        $salt = substr($decoded, 13, $saltLength);
        $subkey = substr($decoded, 13 + $saltLength);

        if ($salt === false || $subkey === false || $subkey === '') {
            return LegacyHashVerificationResult::ForceReset;
        }

        $algorithm = match ($prf) {
            0 => 'sha1',
            1 => 'sha256',
            2 => 'sha512',
            default => null,
        };

        if ($algorithm === null) {
            return LegacyHashVerificationResult::ForceReset;
        }

        $derived = hash_pbkdf2($algorithm, $providedPassword, $salt, $iterations, strlen($subkey), true);

        return hash_equals($subkey, $derived)
            ? LegacyHashVerificationResult::VerifiedNeedsRehash
            : LegacyHashVerificationResult::Failed;
    }

    private function verifyV2(string $decoded, string $providedPassword): LegacyHashVerificationResult
    {
        // ASP.NET Identity v2: [0x00][16-byte salt][32-byte subkey]
        if (strlen($decoded) !== 49) {
            return LegacyHashVerificationResult::ForceReset;
        }

        $salt = substr($decoded, 1, 16);
        $subkey = substr($decoded, 17, 32);

        if ($salt === false || $subkey === false) {
            return LegacyHashVerificationResult::ForceReset;
        }

        $derived = hash_pbkdf2('sha1', $providedPassword, $salt, 1000, 32, true);

        return hash_equals($subkey, $derived)
            ? LegacyHashVerificationResult::VerifiedNeedsRehash
            : LegacyHashVerificationResult::Failed;
    }

    private function verifyMd5Marker(string $decoded, string $providedPassword): LegacyHashVerificationResult
    {
        // ADR-007: flip marker 0xF0->0x01 and verify v3 against md5_hex(password).
        $decoded[0] = chr(0x01);

        return $this->verifyV3($decoded, $this->md5Hex($providedPassword));
    }

    private function md5Hex(string $value): string
    {
        return strtolower(bin2hex(hash('md5', $value, true)));
    }

    private function readNetworkUInt32(string $bytes): int
    {
        $unpacked = unpack('Nvalue', $bytes);

        return (int) ($unpacked['value'] ?? 0);
    }

    private function isLaravelHash(string $value): bool
    {
        return str_starts_with($value, '$2y$')
            || str_starts_with($value, '$2a$')
            || str_starts_with($value, '$2b$')
            || str_starts_with($value, '$argon2i$')
            || str_starts_with($value, '$argon2id$');
    }
}
