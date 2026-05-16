using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Queries.GetAudioFile
{
    public class GetAudioFileResponse
    {
        public Stream Stream { get; set; } = null!;

        public string ContentType { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;
    }
}
