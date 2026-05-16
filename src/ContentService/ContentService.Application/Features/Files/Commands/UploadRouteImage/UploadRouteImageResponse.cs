using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Commands.UploadRouteImage
{
    public class UploadRouteImageResponse
    {
        public Guid Id { get; set; }

        public string FileExtension { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
