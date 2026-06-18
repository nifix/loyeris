using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IAuthOneTimeTokenRepository"/>.
/// </summary>
public class AuthOneTimeTokenRepository(IdentityAccessDbContext dbContext) : IAuthOneTimeTokenRepository
{
    /// <inheritdoc />
    public Task<AuthOneTimeToken> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
        => dbContext.AuthOneTimeTokens
            .Include(oneTimeToken => oneTimeToken.User)
            .SingleOrDefaultAsync(oneTimeToken => oneTimeToken.TokenHash == tokenHash, cancellationToken);

    /// <inheritdoc />
    public Task<AuthOneTimeToken> GetActiveByUserAndPurposeAsync(
        Guid userId,
        OneTimeTokenPurpose purpose,
        CancellationToken cancellationToken)
        => dbContext.AuthOneTimeTokens.SingleOrDefaultAsync(
            oneTimeToken => oneTimeToken.UserId == userId
                            && oneTimeToken.Purpose == purpose
                            && oneTimeToken.ConsumedAt == null
                            && oneTimeToken.RevokedAt == null,
            cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(AuthOneTimeToken oneTimeToken, CancellationToken cancellationToken)
        => await dbContext.AuthOneTimeTokens.AddAsync(oneTimeToken, cancellationToken);

    /// <inheritdoc />
    public void Update(AuthOneTimeToken oneTimeToken)
        => dbContext.AuthOneTimeTokens.Update(oneTimeToken);
}
