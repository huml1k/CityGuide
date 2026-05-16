using ContentService.Application.Features.Routes.Queries.GetRoutes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Tags.Queries.GetTags
{
    public class GetTagsQuery : IRequest<IReadOnlyCollection<GetTagsResponse>>
    {
    }
}
