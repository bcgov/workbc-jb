# ADR-007: Verify legacy MD5-wrapped password hashes on login (amends ADR-003)

- **Status:** Accepted
- **Date:** 2026-07-24
- **Deciders:** Architecture owner
- **Amends:** ADR-003 (only the MD5 handling; every other ADR-003 decision stands)

## Context
ADR-003 decided: *"Do not port MD5 — before cutover, force a password reset for any account still on
the MD5 marker."* That was made before we had the real data. Measured against the restored
production database (`AspNetUsers`, 345,985 rows, none with a null hash):

| Stored `PasswordHash` format | Rows | Share |
|---|---|---|
| ASP.NET Identity **v3** (`0x01`, base64 `AQAAAA…`) | 131,947 | 38% |
| **MD5-marker** (`0xF0`, base64 `8AAAAA…`) | 214,038 | **62%** |

Reading the actual .NET hasher (`WorkBC.Web/Helpers/Md5PasswordHasher.cs`, based on the well-known
"safely migrating passwords in ASP.NET Core Identity" pattern) shows the `0xF0` format is **not**
unverifiable: it is a standard Identity **v3 PBKDF2-HMAC-SHA256** hash whose *input* is
`md5_hex(password)` rather than the password, with the version byte flipped `0x01 → 0xF0` as a
marker. To verify one: flip the marker back to `0x01`, compute `md5_hex(providedPassword)`, and run
the normal v3 check; on success the .NET code returns `SuccessRehashNeeded`.

So force-reset would log **~214,038 real users (62%) out of their own accounts** at cutover — a large,
avoidable, outward-facing harm — when those logins are in fact verifiable.

Security note: the stored secret is still **PBKDF2-SHA256** (strong, salted, iterated). MD5 is used
only as a one-time transform of the *input* before PBKDF2; it does not weaken the stored hash against
offline attack (an attacker still faces PBKDF2 and needs the password preimage). And it is
**transitional** — on the first successful login the hash is rehashed to bcrypt/argon2 of the
plaintext, so the MD5 path is exercised at most once per user and then disappears.

## Decision
**Verify the `0xF0` MD5-wrapped hashes on login instead of force-resetting them.** The custom
hasher/user-provider (FND-5) supports three stored formats:
- **v3** (`0x01`): PBKDF2-HMAC-SHA256, parameters read from the self-describing blob → verify.
- **v2** (`0x00`): PBKDF2-HMAC-SHA1, 1000 iters, 16-byte salt, 32-byte subkey → verify (implement for
  completeness even if absent in current data).
- **MD5-marker** (`0xF0`): flip byte 0 to `0x01`, verify the v3 blob against `md5_hex(providedPassword)`
  (lowercase hex of `MD5(UTF-8(password))`) → verify.

On **any** successful verification, **rehash to bcrypt/argon2** of the plaintext and overwrite
`PasswordHash` (and regenerate `SecurityStamp`). **MD5 is never used to create a new hash** — only to
verify a legacy one, once. Truly unrecognized formats (not `0x00`/`0x01`/`0xF0`, or an undecodable
blob) → treat as force-reset (unchanged from ADR-003).

## Consequences
- **Positive:** no mass password reset; all 345,985 users keep their logins and are transparently
  upgraded to bcrypt on next login. The MD5 verification code is self-retiring.
- **Trade-offs accepted:** a bounded, verification-only MD5 code path exists during the migration
  window (guarded, tested, never used for new hashes). ADR-003's cleaner "no MD5 code at all" posture
  is relaxed for this transitional period.
- **Follow-up:** FND-5 AC updated (verify `0xF0` + rehash, rather than force-reset it). Consider a
  later cleanup story to drop the MD5 path once legacy-format rows fall below a threshold.

## Alternatives Considered
- **Force-reset (original ADR-003):** rejected now that the scale is known — 62% of users, avoidably.
- **Implement MD5 as a general hasher:** rejected — MD5 is verification-only here; new hashes are
  always bcrypt/argon2.

## Compliance / Scope
Amends ADR-003's MD5 handling only; session auth, Keycloak admin, email-only reset, and no
non-expiring tokens are unchanged. Governs FND-5.
