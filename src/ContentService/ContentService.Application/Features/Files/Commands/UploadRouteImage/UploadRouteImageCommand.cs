using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Commands.UploadRouteImage
{
    public class UploadRouteImageCommand : IRequest<UploadRouteImageResponse>
    {
        public Guid RouteId { get; set; }

        public IFormFile File { get; set; } = null!;

        public bool IsCover { get; set; }

        public int OrderIndex { get; set; }
    }
}
