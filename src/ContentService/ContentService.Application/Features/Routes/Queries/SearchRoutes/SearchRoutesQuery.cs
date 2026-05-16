using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Queries.SearchRoutes
{
    public class SearchRoutesQuery: IRequest<IReadOnlyCollection<SearchRoutesResponse>>
    {
        public string Search { get; set; } = string.Empty;
    }
}
