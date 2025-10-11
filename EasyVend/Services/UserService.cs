using EasyVend.Data;
using EasyVend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EasyVend.Services
{
    public class UserService(AppDbContext db, ILogger<UserService> logger)
    {
        private readonly AppDbContext _db = db;
        private readonly ILogger<UserService> _logger = logger;

        // -----------------------------
        // Public API
        // -----------------------------

        /// <summary>
        /// Ensure there's a local User row for the signed-in principal.
        /// Creates a new record on first login using claims.
        /// </summary>
        public async Task<User> EnsureUserExistsAsync(ClaimsPrincipal principal, CancellationToken ct = default)
        {
            string? key = null;
            try
            {
                _logger.LogDebug("EnsureUserExistsAsync: starting for principal '{Name}'", principal.Identity?.Name);

                key = GetStableKey(principal);
                if (string.IsNullOrEmpty(key))
                {
                    _logger.LogWarning("EnsureUserExistsAsync: missing stable identifier (oid/sub) in token for principal '{Name}'", principal.Identity?.Name);
                    throw new InvalidOperationException("No stable identifier (oid/sub) in token.");
                }

                var user = await _db.Users.FirstOrDefaultAsync(u => u.EntraObjectId == key, ct);
                if (user != null)
                {
                    _logger.LogDebug("EnsureUserExistsAsync: found existing user {UserId} for key {Key}", user.Id, key);
                    return user;
                }

                // Seed from claims
                var email = GetEmailFromClaims(principal);
                var name = (principal.Identity?.Name ?? string.Empty).Trim();

                _logger.LogInformation("EnsureUserExistsAsync: creating new user for key {Key}, email '{Email}', name '{Name}'", key, email, name);

                user = new User
                {
                    Id = Guid.NewGuid(),
                    EntraObjectId = key,
                    Email = email,
                    DisplayName = name,
                    CreatedAt = DateTime.UtcNow,
                    Memberships = { }
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("EnsureUserExistsAsync: created user {UserId} for key {Key}", user.Id, key);
                return user;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("EnsureUserExistsAsync: operation canceled for key {Key}", key);
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "EnsureUserExistsAsync: database update failed for key {Key}", key);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EnsureUserExistsAsync: unexpected error for key {Key}", key);
                throw;
            }
        }

        /// <summary>
        /// Fetch the current User row for the signed-in principal (null if not found).
        /// </summary>
        public async Task<User?> GetCurrentAsync(ClaimsPrincipal principal, CancellationToken ct = default)
        {
            var key = GetStableKey(principal);
            if (string.IsNullOrEmpty(key)) return null;
            return await _db.Users.FirstOrDefaultAsync(u => u.EntraObjectId == key, ct);
        }

        /// <summary>
        /// minimal completeness rule: DisplayName + contact email set, and onboarding flag true.
        /// </summary>
        public bool IsProfileComplete(User u)
        {
            var contact = string.IsNullOrWhiteSpace(u.PreferredContactEmail) ? u.Email : u.PreferredContactEmail;
            return !string.IsNullOrWhiteSpace(u.DisplayName)
                && !string.IsNullOrWhiteSpace(contact)
                && u.OnboardingComplete;
        }

        /// <summary>
        /// Update required profile fields; optionally mark onboarding complete.
        /// </summary>
        public async Task<(bool Ok, string Msg)> UpdateProfileAsync(
            Guid userId,
            string displayName,
            string preferredContactEmail,
            bool completeOnboarding,
            CancellationToken ct = default)
        {
            var user = await _db.Users.FindAsync([userId], ct);
            if (user == null) return (false, "User not found.");

            user.DisplayName = (displayName ?? string.Empty).Trim();
            user.PreferredContactEmail = (preferredContactEmail ?? string.Empty).Trim();

            if (completeOnboarding) user.OnboardingComplete = true;

            await _db.SaveChangesAsync(ct);
            return (true, "Saved.");
        }

        public async Task<(bool Ok, string Msg)> UpdateProfileAsync(
            Guid userId,
            string preferredContactEmail,
            string companyName,
            string addressLine1,
            string addressLine2,
            string city,
            string region,
            string postalCode,
            string country,
            string[] marketplaces,
            bool completeOnboarding,
            CancellationToken ct = default)
        {
            var user = await _db.Users.FindAsync([userId], ct);
            if (user == null) return (false, "User not found.");

            user.PreferredContactEmail = (preferredContactEmail ?? string.Empty).Trim();
            user.CompanyName = (companyName ?? string.Empty).Trim();
            user.AddressLine1 = (addressLine1 ?? string.Empty).Trim();
            user.AddressLine2 = (addressLine2 ?? string.Empty).Trim();
            user.City = (city ?? string.Empty).Trim();
            user.Region = (region ?? string.Empty).Trim();
            user.PostalCode = (postalCode ?? string.Empty).Trim();
            user.Country = (country ?? string.Empty).Trim();

            var normMarkets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (marketplaces != null)
            {
                foreach (var s in marketplaces)
                {
                    var t = (s ?? string.Empty).Trim();
                    if (!string.IsNullOrWhiteSpace(t)) normMarkets.Add(t);
                }
            }
            user.PreferredMarketplaces = string.Join(",", normMarkets);

            if (completeOnboarding) user.OnboardingComplete = true;

            await _db.SaveChangesAsync(ct);
            return (true, "Saved.");
        }

        // -----------------------------
        // Helpers
        // -----------------------------

        /// <summary>
        /// Builds a stable cross-provider key:
        ///  - AAD:   oid (or AAD objectidentifier)
        ///  - MSA/others: issuer|sub
        /// </summary>
        private static string? GetStableKey(ClaimsPrincipal principal)
        {
            var oid = principal.FindFirstValue("oid")
                      ?? principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");

            if (!string.IsNullOrEmpty(oid)) return oid;

            var sub = principal.FindFirstValue("sub");
            if (string.IsNullOrEmpty(sub)) return null;

            var iss = principal.FindFirstValue("iss") ?? string.Empty;
            return $"{iss}|{sub}";
        }

        /// <summary>
        /// Tries to extract a usable email from common claim types and normalizes case.
        /// </summary>
        private static string GetEmailFromClaims(ClaimsPrincipal principal)
        {
            var email =
                principal.FindFirstValue(ClaimTypes.Email) ??
                principal.FindFirstValue("preferred_username") ??
                string.Empty;

            return email.Trim().ToLowerInvariant();
        }
    }
}
