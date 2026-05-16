using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Commands.DeleteAudioFile
{
    public class DeleteAudioFileCommand : IRequest
    {
        public Guid AudioFileId { get; set; }
    }
}
