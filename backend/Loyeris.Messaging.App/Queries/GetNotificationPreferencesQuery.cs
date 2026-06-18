using Loyeris.Messaging.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Messaging.App.Queries;

/// <summary>
/// Query that lists notification preferences.
/// </summary>
public record GetNotificationPreferencesQuery() : IRequest<Result<IReadOnlyList<NotificationPreferenceDto>>>;
