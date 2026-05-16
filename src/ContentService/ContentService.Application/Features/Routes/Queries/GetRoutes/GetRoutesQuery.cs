using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Queries.GetRoutes
{
    public class GetRoutesQuery : IRequest<IReadOnlyCollection<GetRoutesResponse>>
    {
    }
}
