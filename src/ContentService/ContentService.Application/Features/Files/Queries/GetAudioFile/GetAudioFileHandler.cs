using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Queries.GetAudioFile
{
    public class GetAudioFileHandler : IRequestHandler< GetAudioFileQuery, GetAudioFileResponse>
    {
        private readonly IAudioFileRepository _audioRepository;

        public GetAudioFileHandler( IAudioFileRepository audioRepository)
        {
            _audioRepository = audioRepository;
        }

        public async Task<GetAudioFileResponse> Handle( GetAudioFileQuery request, CancellationToken cancellationToken)
        {
            var audio = await _audioRepository.GetByIdAsync(request.AudioFileId, cancellationToken);

            if (audio is null)
            {
                throw new Exception("Audio file not found");
            }

            return new GetAudioFileResponse
            {
                Stream = new MemoryStream(),

                ContentType = $"audio/{audio.FileExtension}",

                FileName =
                    $"{audio.Id}.{audio.FileExtension}"
            };
        }
    }
}
