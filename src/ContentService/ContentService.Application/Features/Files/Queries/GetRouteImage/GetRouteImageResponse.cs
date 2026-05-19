using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Queries.GetRouteImage
{
    public class GetRouteImageResponse
    {
        public string Url { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
