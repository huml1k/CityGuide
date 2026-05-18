using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using ContentService.Application.Interfaces;

namespace ContentService.Application.Features.Files.Commands.UploadAudioFile
{
    public class UploadAudioFileCommandHandler : IRequestHandler<UploadAudioFileCommand, UploadAudioFileResponse>
    {
        private readonly IAudioFileRepository _audioRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;
        private readonly string _bucketName = "content-files";

        public UploadAudioFileCommandHandler(IAudioFileRepository audioRepository, IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _audioRepository = audioRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<UploadAudioFileResponse> Handle(UploadAudioFileCommand request, CancellationToken cancellationToken)
        {
            var extension = Path
                .GetExtension(request.File.FileName)
                .Replace(".", "");
            
            await _fileStorageService.UploadFileAsync(request.File.OpenReadStream(), request.File.Name, extension, cancellationToken);
            var filePath = _bucketName + "/" + request.File.FileName;
            
            var audio = new AudioFile
            {
                Id = Guid.NewGuid(),
                RouteId = request.RouteId,
                FileExtension = extension,
                OriginalFilename = request.File.FileName,
                CreatedAt = DateTime.UtcNow,
                Path =  filePath
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

