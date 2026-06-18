using Loyeris.RentCollection.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.RentCollection.App.Queries;

/// <summary>
/// Query that lists rent reminders.
/// </summary>
public record GetRentRemindersQuery() : IRequest<Result<IReadOnlyList<RentReminderDto>>>;
