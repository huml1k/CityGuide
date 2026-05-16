using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Commands.UploadAudioFile
{
    public class UploadAudioFileCommandHandler : IRequestHandler<UploadAudioFileCommand, UploadAudioFileResponse>
    {
        private readonly IAudioFileRepository _audioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UploadAudioFileCommandHandler(IAudioFileRepository audioRepository, IUnitOfWork unitOfWork)
        {
            _audioRepository = audioRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UploadAudioFileResponse> Handle(UploadAudioFileCommand request, CancellationToken cancellationToken)
        {
            var extension = Path
                .GetExtension(request.File.FileName)
                .Replace(".", "");

            var audio = new AudioFile
            {
                Id = Guid.NewGuid(),
                RouteId = request.RouteId,
                FileExtension = extension,
                OriginalFilename = request.File.FileName,
                CreatedAt = DateTime.UtcNow
            };

            await _audioRepository.AddAsync(audio, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UploadAudioFileResponse
            {
                Id = audio.Id,
                FileExtension = audio.FileExtension,
                CreatedAt = audio.CreatedAt
            };
        }
    }
}

