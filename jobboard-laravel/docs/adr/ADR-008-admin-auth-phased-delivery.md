# ADR-008: Build FND-6's admin-auth substance now; defer the Keycloak handshake

- **Status:** Accepted
- **Date:** 2026-07-26
- **Deciders:** Architecture owner
- **Amends:** ADR-003 (only the *sequencing/mechanism* of admin login; the target — Keycloak OIDC — is unchanged)

## Context
ADR-003 decided admin auth is **Keycloak OIDC** via Socialite, mapped to `AdminUsers` roles. No
Keycloak realm/client exists in this environment yet (nothing in `.env`, no IdP reachable locally) —
standing up real OIDC requires credentials only whoever runs Keycloak can issue.

The product owner asked to build the **admin panel (ADM-1…9)** next, ahead of finishing the
job-seeker ACCOUNT epic, for demo purposes. ADM-1 depends on FND-6.

Checked the real `AdminUsers` schema before designing a workaround (per copilot-instructions
constraint #9 — verify, don't invent): it has **no password/credential column at all** —
`SamAccountName`, `DisplayName`, `GivenName`, `Surname`, `AdminLevel`, `Guid`, plus lock/audit FKs.
This *confirms* ADR-003's own note that "`AdminUsers` exists for roles/management but is not used
for login (Keycloak is)." Admin identity has never been password-based in this system. Building a
local password-login form would invent a credential store that has never existed — exactly what
constraint #9 forbids.

## Decision
Split FND-6 into its **substance** (built now) and its **handshake** (deferred):

**Built now:**
- A new `admin` guard (session-based) with a provider bound to `App\Models\AdminUser` (FND-2).
- `AdminUser` implements `Authenticatable`, but the provider never checks a password — there is
  none to check. It only supports **direct session login** (`Auth::guard('admin')->login($adminUser)`),
  the same shape Laravel uses internally once a credential check would normally succeed.
- Filament's admin panel authenticates via the `admin` guard.
- Role-based access policies from `AdminUsers.AdminLevel` (`Disabled` / `Reporting` / `Admin` /
  `SuperAdmin`) gate navigation and resources, per ADM-1.
- The impersonation scaffold (ADM-4): a Filament action that starts an impersonated **seeker**
  session and writes an `ImpersonationLog` row (`Token`, `AspNetUserId`, `AdminUserId`,
  `DateTokenCreated`).

**Deferred (its own follow-up story, "FND-6b — Keycloak OIDC handshake"):**
- The actual Socialite ↔ Keycloak Authorization-Code exchange that resolves a Keycloak identity to
  an `AdminUsers` row and calls `Auth::guard('admin')->login($adminUser)`. This is the *only* piece
  that needs real IdP credentials — everything built now is unaffected by the swap.

**Local/demo access meanwhile:** the same pattern already used for the job-seeker portal — a
**gitignored, `local`-environment-guarded** route (`routes/dev-preview.php`) that logs in a chosen
`AdminUser` row directly. No schema change, no invented credential field, nothing committed.

## Consequences
- **Positive:** every ADM-* resource, policy, and the impersonation flow can be built and demoed
  today. When Keycloak credentials arrive, FND-6b only replaces the login *route* — the guard,
  provider, policies, and resources already built don't change.
- **Trade-off accepted:** local/demo admin access isn't real authentication (identical caveat to
  the job-seeker dev-preview route) — must not be mistaken for production readiness, and the
  gitignored file is never committed.
- **Follow-up:** track "FND-6b — Keycloak OIDC handshake" against FND-6 in `EPIC-FOUNDATION.md` once
  realm/client credentials exist.

## Alternatives Considered
- **Build a placeholder password login on `AdminUsers`.** Rejected — the table has no credential
  column; this would invent a field/behavior the real system has never had (constraint #9).
- **Block all admin-panel work until real Keycloak credentials exist.** Rejected — unnecessarily
  blocks a demo and 9 stories' worth of resource/policy work that doesn't depend on the IdP at all.

## Compliance / Scope
Governs FND-6 and ADM-1. Does not change ADR-003's production target (Keycloak OIDC remains the
only real admin login mechanism); it only phases the delivery.
