using MediatR;

namespace ContentService.Application.Features.Routes.Queries.GetMyRoutes;

public sealed class GetMyRoutesQuery : IRequest<IReadOnlyCollection<GetMyRoutesResponse>>
{
}
