using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Queries.GetRouteImage
{
    public class GetRouteImageQuery : IRequest<GetRouteImageResponse>
    {
        public Guid ImageId { get; set; }
    }
}
