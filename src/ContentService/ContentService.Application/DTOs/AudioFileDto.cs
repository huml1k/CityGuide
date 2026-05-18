using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.DTOs
{
    public class AudioFileDto
    {
        public Guid Id { get; set; }

        public string FileExtension { get; set; } = string.Empty;

        public int? DurationSeconds { get; set; }

        public int OrderIndex { get; set; }

        public string? OriginalFilename { get; set; }
    }
}
