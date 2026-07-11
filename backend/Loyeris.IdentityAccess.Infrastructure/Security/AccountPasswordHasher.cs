using Loyeris.IdentityAccess.App.Security;
using Loyeris.IdentityAccess.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Loyeris.IdentityAccess.Infrastructure.Security;

/// <summary>
/// Hashes account passwords with the ASP.NET Core Identity password hasher.
/// </summary>
public class AccountPasswordHasher : IAccountPasswordHasher
{
    private readonly PasswordHasher<AppUser> passwordHasher = new();

    /// <inheritdoc />
    public string HashPassword(AppUser user, string password)
        => passwordHasher.HashPassword(user, password);

    /// <inheritdoc />
    public bool VerifyPassword(AppUser user, string password)
        => passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password)
           != PasswordVerificationResult.Failed;
}
