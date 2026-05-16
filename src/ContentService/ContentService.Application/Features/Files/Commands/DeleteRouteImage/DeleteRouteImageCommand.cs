using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Commands.DeleteRouteImage
{
    public class DeleteRouteImageCommand : IRequest
    {
        public Guid ImageId { get; set; }
    }
}
