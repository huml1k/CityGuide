using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Commands.UploadAudioFile
{
    public class UploadAudioFileCommand : IRequest<UploadAudioFileResponse>
    {
        public Guid RouteId { get; set; }

        public IFormFile File { get; set; } = null!;

    }
}
