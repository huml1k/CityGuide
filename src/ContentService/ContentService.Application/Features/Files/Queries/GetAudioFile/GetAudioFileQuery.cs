using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Queries.GetAudioFile
{
    public class GetAudioFileQuery : IRequest<GetAudioFileResponse>
    {
        public Guid AudioFileId { get; set; }
    }
}
