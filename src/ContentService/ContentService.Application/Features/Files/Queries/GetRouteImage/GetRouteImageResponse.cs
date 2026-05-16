using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Queries.GetRouteImage
{
    public class GetRouteImageResponse
    {
        public Stream Stream { get; set; } = null!;

        public string ContentType { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;
    }
}
