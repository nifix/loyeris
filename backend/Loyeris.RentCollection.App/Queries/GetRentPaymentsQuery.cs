using Loyeris.RentCollection.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.RentCollection.App.Queries;

/// <summary>
/// Query that lists rent payments.
/// </summary>
public record GetRentPaymentsQuery() : IRequest<Result<IReadOnlyList<RentPaymentDto>>>;
