using Loyeris.Messaging.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Messaging.App.Queries;

/// <summary>
/// Query that lists outbox messages.
/// </summary>
public record GetOutboxMessagesQuery() : IRequest<Result<IReadOnlyList<OutboxMessageDto>>>;
