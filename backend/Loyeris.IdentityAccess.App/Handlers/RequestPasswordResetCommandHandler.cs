using System.Net.Mail;
using Loyeris.IdentityAccess.App.Commands;
using Loyeris.IdentityAccess.App.Notifications;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Security;
using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Handlers;

/// <summary>
/// Issues a short-lived password reset token while keeping account existence private.
/// </summary>
public class RequestPasswordResetCommandHandler(
    IAppUserAuthRepository userRepository,
    IAuthOneTimeTokenRepository tokenRepository,
    IAuthEventRepository eventRepository,
    IIdentityAccessUnitOfWork unitOfWork,
    IOneTimeTokenService tokenService,
    IPasswordResetSender passwordResetSender,
    TimeProvider timeProvider) : IRequestHandler<RequestPasswordResetCommand, Result<Unit>>
{
    private static readonly Error InvalidEmail = new(
        "identity.password_reset.invalid_email",
        "L'adresse email est invalide.",
        ErrorType.Validation);

    /// <inheritdoc />
    public async Task<Result<Unit>> Handle(
        RequestPasswordResetCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email?.Trim();
        if (!IsValidEmail(email))
            return Result<Unit>.Failure(InvalidEmail);

        var user = await userRepository.GetByNormalizedEmailAsync(email.ToUpperInvariant(), cancellationToken);

        // The same response is returned for missing, disabled, and unverified accounts to prevent enumeration.
        if (user is null || user.Status != UserStatus.Active)
            return Result<Unit>.NoContent();

        var now = timeProvider.GetUtcNow();
        var activeToken = await tokenRepository.GetActiveByUserAndPurposeAsync(
            user.Id,
            OneTimeTokenPurpose.PasswordReset,
            cancellationToken);
        var generatedToken = tokenService.Generate();
        
        var newToken = new AuthOneTimeToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            Purpose = OneTimeTokenPurpose.PasswordReset,
            TokenHash = generatedToken.Hash,
            SentToEmail = user.Email,
            CreatedAt = now,
            ExpiresAt = now.AddHours(1),
            RequestedByIp = Truncate(request.IpAddress, 64),
            UserAgent = Truncate(request.UserAgent, 512)
        };

        if (activeToken is not null)
        {
            activeToken.RevokedAt = now;
            tokenRepository.Update(activeToken);
        }

        var persisted = await unitOfWork.ExecuteInTransactionAsync(async transactionCancellationToken =>
        {
            // Revoke the previous token before inserting its replacement to satisfy the filtered unique index.
            if (activeToken is not null)
                await unitOfWork.SaveChangesAsync(transactionCancellationToken);

            await tokenRepository.AddAsync(newToken, transactionCancellationToken);
            await eventRepository.AddAsync(new AuthEvent
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                NormalizedEmail = user.NormalizedEmail,
                Type = AuthEventType.PasswordResetRequested,
                OccurredAt = now,
                IpAddress = Truncate(request.IpAddress, 64),
                UserAgent = Truncate(request.UserAgent, 512)
            }, transactionCancellationToken);
            await unitOfWork.SaveChangesAsync(transactionCancellationToken);
        }, cancellationToken);

        // A concurrent request still receives the neutral response and can use the winning email.
        if (!persisted)
            return Result<Unit>.NoContent();

        await passwordResetSender.SendAsync(
            user.Email,
            user.FirstName,
            generatedToken.PlainText,
            cancellationToken);

        return Result<Unit>.NoContent();
    }

    private static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email)
               && email.Length <= 320
               && MailAddress.TryCreate(email, out var parsed)
               && string.Equals(parsed.Address, email, StringComparison.OrdinalIgnoreCase);
    }

    private static string Truncate(string value, int maximumLength)
    {
        return string.IsNullOrEmpty(value) || value.Length <= maximumLength
            ? value
            : value[..maximumLength];
    }
}
