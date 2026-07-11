namespace Loyeris.IdentityAccess.App.Security;

/// <summary>
/// Generates and hashes authentication one-time tokens.
/// </summary>
public interface IOneTimeTokenService
{
    /// <summary>
    /// Generates a cryptographically secure token and its hash.
    /// </summary>
    GeneratedOneTimeToken Generate();

    /// <summary>
    /// Computes the stable persistence hash for a raw token.
    /// </summary>
    string Hash(string plainTextToken);
}
