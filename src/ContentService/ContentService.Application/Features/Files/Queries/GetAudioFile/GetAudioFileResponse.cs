using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Queries.GetAudioFile
{
    public class GetAudioFileResponse
    {
        public string Url { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
