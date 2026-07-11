using Loyeris.IdentityAccess.App.Commands;
using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.IdentityAccess.App.Notifications;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Security;
using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Handlers;

/// <summary>
/// Validates credentials, handles pending verification, and creates authenticated sessions.
/// </summary>
public class LoginCommandHandler(
    IAppUserAuthRepository userRepository,
    IAuthSessionRepository sessionRepository,
    IAuthOneTimeTokenRepository oneTimeTokenRepository,
    IAuthEventRepository eventRepository,
    IIdentityAccessUnitOfWork unitOfWork,
    IAccountPasswordHasher passwordHasher,
    IOneTimeTokenService tokenService,
    IAccessTokenService accessTokenService,
    IAuthenticationLifetimeProvider lifetimeProvider,
    IEmailVerificationSender emailVerificationSender,
    TimeProvider timeProvider) : IRequestHandler<LoginCommand, Result<AuthenticationSessionDto>>
{
    private static readonly Error InvalidCredentials = new(
        "identity.invalid_credentials",
        "L'adresse email ou le mot de passe est incorrect.",
        ErrorType.Unauthorized);

    private static readonly Error EmailNotVerified = new(
        "identity.email_not_verified",
        "L'adresse email doit être vérifiée avant la connexion.",
        ErrorType.Forbidden);

    /// <inheritdoc />
    public async Task<Result<AuthenticationSessionDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email?.Trim();

        if (string.IsNullOrWhiteSpace(email) || email.Length > 320 || string.IsNullOrEmpty(request.Password))
            return Result<AuthenticationSessionDto>.Failure(InvalidCredentials);

        var normalizedEmail = email.ToUpperInvariant();
        var user = await userRepository.GetByNormalizedEmailAsync(normalizedEmail, cancellationToken);
        var now = timeProvider.GetUtcNow();

        if (user is null)
            return await RecordFailureAsync(null, normalizedEmail, "invalid-credentials", false, now, request, cancellationToken);

        if (user.Status == UserStatus.Disabled || user.LockoutEndsAt > now)
            return await RecordFailureAsync(user, normalizedEmail, "account-unavailable", false, now, request, cancellationToken);

        if (!passwordHasher.VerifyPassword(user, request.Password))
            return await RecordFailureAsync(user, normalizedEmail, "invalid-credentials", true, now, request, cancellationToken);

        user.AccessFailedCount = 0;
        user.LockoutEndsAt = null;
        user.UpdatedAt = now;

        if (user.Status == UserStatus.Invited)
        {
            await ResendVerificationAsync(user, request, now, cancellationToken);
            return Result<AuthenticationSessionDto>.Failure(EmailNotVerified);
        }

        var sessionId = Guid.NewGuid();
        var refreshToken = tokenService.Generate();
        var refreshExpiresAt = lifetimeProvider.GetRefreshTokenExpiration(now, request.RememberMe);
        
        var session = new AuthSession
        {
            Id = sessionId,
            UserId = user.Id,
            User = user,
            Status = AuthSessionStatus.Active,
            DeviceLabel = "Web browser",
            UserAgent = Truncate(request.UserAgent, 512),
            IpAddress = Truncate(request.IpAddress, 64),
            CreatedAt = now,
            LastSeenAt = now,
            ExpiresAt = refreshExpiresAt
        };
        
        session.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            Session = session,
            TokenHash = refreshToken.Hash,
            CreatedAt = now,
            ExpiresAt = refreshExpiresAt,
            CreatedByIp = Truncate(request.IpAddress, 64)
        });
        
        session.AuthEvents.Add(new AuthEvent
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            SessionId = sessionId,
            Session = session,
            NormalizedEmail = normalizedEmail,
            Type = AuthEventType.LoginSucceeded,
            OccurredAt = now,
            IpAddress = Truncate(request.IpAddress, 64),
            UserAgent = Truncate(request.UserAgent, 512)
        });

        user.LastLoginAt = now;
        userRepository.Update(user);
        await sessionRepository.AddAsync(session, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = accessTokenService.Generate(user, sessionId);
        return Result<AuthenticationSessionDto>.Success(new AuthenticationSessionDto(
            accessToken.Token,
            accessToken.ExpiresAt,
            refreshToken.PlainText,
            refreshExpiresAt,
            request.RememberMe,
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName));
    }

    private async Task ResendVerificationAsync(
        AppUser user,
        LoginCommand request,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var activeToken = await oneTimeTokenRepository.GetActiveByUserAndPurposeAsync(
            user.Id,
            OneTimeTokenPurpose.EmailVerification,
            cancellationToken);

        if (activeToken is not null)
        {
            activeToken.RevokedAt = now;
            oneTimeTokenRepository.Update(activeToken);
        }

        var generatedToken = tokenService.Generate();
        await oneTimeTokenRepository.AddAsync(new AuthOneTimeToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Purpose = OneTimeTokenPurpose.EmailVerification,
            TokenHash = generatedToken.Hash,
            SentToEmail = user.Email,
            CreatedAt = now,
            ExpiresAt = now.AddHours(24),
            RequestedByIp = Truncate(request.IpAddress, 64),
            UserAgent = Truncate(request.UserAgent, 512)
        }, cancellationToken);
        await eventRepository.AddAsync(new AuthEvent
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            NormalizedEmail = user.NormalizedEmail,
            Type = AuthEventType.EmailVerificationResent,
            OccurredAt = now,
            IpAddress = Truncate(request.IpAddress, 64),
            UserAgent = Truncate(request.UserAgent, 512)
        }, cancellationToken);

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await emailVerificationSender.SendAsync(
            user.Email,
            user.FirstName,
            generatedToken.PlainText,
            cancellationToken);
    }

    private async Task<Result<AuthenticationSessionDto>> RecordFailureAsync(
        AppUser user,
        string normalizedEmail,
        string reason,
        bool incrementFailureCount,
        DateTimeOffset now,
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var eventType = AuthEventType.LoginFailed;

        if (user is not null && incrementFailureCount)
        {
            user.AccessFailedCount++;
            user.UpdatedAt = now;

            if (user.AccessFailedCount >= 5)
            {
                user.LockoutEndsAt = now.AddMinutes(15);
                eventType = AuthEventType.AccountLocked;
            }

            userRepository.Update(user);
        }

        await eventRepository.AddAsync(new AuthEvent
        {
            Id = Guid.NewGuid(),
            UserId = user?.Id,
            NormalizedEmail = normalizedEmail,
            Type = eventType,
            OccurredAt = now,
            IpAddress = Truncate(request.IpAddress, 64),
            UserAgent = Truncate(request.UserAgent, 512),
            FailureReason = reason
        }, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AuthenticationSessionDto>.Failure(InvalidCredentials);
    }

    private static string Truncate(string value, int maximumLength)
    {
        return string.IsNullOrEmpty(value) || value.Length <= maximumLength
            ? value
            : value[..maximumLength];
    }
}
