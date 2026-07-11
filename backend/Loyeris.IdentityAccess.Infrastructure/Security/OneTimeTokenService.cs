using System.Security.Cryptography;
using System.Text;
using Loyeris.IdentityAccess.App.Security;
using Microsoft.AspNetCore.WebUtilities;

namespace Loyeris.IdentityAccess.Infrastructure.Security;

/// <summary>
/// Generates URL-safe random tokens and hashes them with SHA-256.
/// </summary>
public class OneTimeTokenService : IOneTimeTokenService
{
    /// <inheritdoc />
    public GeneratedOneTimeToken Generate()
    {
        var plainText = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        return new GeneratedOneTimeToken(plainText, Hash(plainText));
    }

    /// <inheritdoc />
    public string Hash(string plainTextToken)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(plainTextToken)));
}
