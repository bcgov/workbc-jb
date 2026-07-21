# ADR-003: Session auth for job seekers; Keycloak OIDC for admin

- **Status:** Accepted
- **Date:** 2026-06-26
- **Deciders:** Architecture owner
- **Jira:** —

## Context
Current-state auth (verified in the .NET code):
- **Job seekers** (`WorkBC.Web`): ASP.NET Core **Identity** (`AddDefaultIdentity<JobSeeker>`)
  with **JWT bearer** tokens — HS256, shared `Secret`, **`AccessExpiration = 10080` min (7 days)**,
  **no refresh token**. The default password hasher is replaced by a custom **`Md5PasswordHasher`**
  that verifies legacy MD5 hashes and rehashes on login. (Committed default JWT secret; 7-day
  non-revocable token — both flagged as security debt.)
- **Admin** (`WorkBC.Admin`): **Keycloak OIDC** (Authorization-Code flow) + cookie session +
  refresh-token handling. `AdminUsers` exists for roles/management but is not used for login
  (Keycloak is). Keycloak is the BC Gov standard IdP.
- **Impersonation:** an admin acts as a seeker via a JWT carrying a `JobBoardAdmin` role +
  admin id; audited in `Impersonations`.

## Decision
- **Job seekers → Laravel session authentication** (server-rendered app; no SPA, so no JWT).
  Verify legacy ASP.NET Identity **v2/v3 PBKDF2** hashes on login and **rehash to bcrypt/argon2**;
  keep `NormalizedEmail`/`NormalizedUserName` while any .NET reader remains. **Do not port MD5** —
  before cutover, force a password reset for any account still on the MD5 marker.
- **Admin → Keycloak OIDC** via Laravel Socialite, bridged to the **Filament** panel; map the
  Keycloak identity to `AdminUsers` roles (SuperAdmin / Admin / Reporting).
- **Secrets** in AWS Secrets Manager (no committed signing secrets). **No non-expiring tokens.**
- **Impersonation** and its `Impersonations` audit trail are preserved (implemented via a
  server-side impersonated session, not a bearer token).

## Consequences
- **Positive:** removes the 7-day non-revocable JWT and the committed secret; sessions are
  revocable; Keycloak retained for admin per BC Gov standard; MD5 eliminated.
- **Negative:** a custom hasher/user-provider is needed to verify legacy Identity hashes; a
  one-time forced-reset pass for remaining MD5 accounts.
- **Follow-up:** implement the Identity-compatible verifier with test vectors from the live
  stack; the MD5-migration story; Socialite-OIDC → Filament bridge; email verification
  (`VerificationGuid`) and password-reset flows.

## Alternatives Considered
- **Keep JWT for seekers:** rejected — no SPA needs it; sessions are simpler and revocable.
- **Replace Keycloak for admin:** rejected — BC Gov standard; unnecessary risk.

## Password reset — decided
**Email-only** password reset (the current production behavior). The legacy `SecurityQuestions`
lookup and `AspNetUsers.SecurityAnswer` are **not used** for reset — treat them as legacy/inactive;
do **not** build a security-question flow. Use Laravel's email reset (`password_reset_tokens`), plus
the existing email-verification (`VerificationGuid`) flow for registration.

## Compliance / Scope
Standing rule once Accepted. Affects the Auth epic and the JobSeeker module.
