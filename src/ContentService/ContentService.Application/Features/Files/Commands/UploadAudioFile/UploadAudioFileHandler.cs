using ContentService.Application.Common.Exceptions;
using ContentService.Application.Interfaces;
using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Commands.UploadAudioFile
{
    public class UploadAudioFileCommandHandler : IRequestHandler<UploadAudioFileCommand, UploadAudioFileResponse>
    {
        private readonly IAudioFileRepository _audioRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<UploadAudioFileCommandHandler> _logger;
        private readonly string _bucketName = "content-files";

        public UploadAudioFileCommandHandler(IAudioFileRepository audioRepository, IUnitOfWork unitOfWork, IFileStorageService fileStorageService, IRouteRepository  routeRepository, ILogger<UploadAudioFileCommandHandler> logger)
        {
            _audioRepository = audioRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
            _routeRepository = routeRepository;
            _logger = logger;
        }

        public async Task<UploadAudioFileResponse> Handle(UploadAudioFileCommand request, CancellationToken cancellationToken)
        {
            var extension = Path.GetExtension(request.File.FileName);

            if (request.File is null)
            {
                throw new ValidationException(
                    new Dictionary<string, string[]>
                    {
            {
                nameof(request.File),
                new[] { "Audio file is required." }
            }
                    });
            }

            if (request.File.Length == 0)
            {
                throw new ValidationException(
                    new Dictionary<string, string[]>
                    {
            {
                nameof(request.File),
                new[] { "Audio file is empty." }
            }
                    });
            }

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new InvalidFileException(
                    "Audio file extension is invalid.");
            }

            if (!await _routeRepository.ExistsAsync(request.RouteId, cancellationToken))
            {
                throw new RouteNotFoundException(
                    request.RouteId);
            }

            var audio = new AudioFile
            {
                Id = Guid.NewGuid(),
                RouteId = request.RouteId,
                FileExtension = extension.TrimStart('.'),
                CreatedAt = DateTime.UtcNow,
            };
            
            var objectKey = $"{audio.Id}{extension}";

            if (string.IsNullOrWhiteSpace(request.File.ContentType))
            {
                throw new InvalidFileException(
                    "Audio content type is invalid.");
            }

            try
            {
                await _fileStorageService.UploadFileAsync(
                    request.File.OpenReadStream(),
                    objectKey,
                    request.File.ContentType,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                throw new FileUploadException(
                    "Failed to upload audio file.",
                    ex);
            }

            audio.Path = objectKey;
            
            await _audioRepository.AddAsync(audio, cancellationToken);
            var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (saved <= 0)
            {
                throw new BusinessRuleException(
                    "Failed to save audio file metadata.");
            }

            return new UploadAudioFileResponse
            {
                Id = audio.Id,
                FileExtension = audio.FileExtension,
                CreatedAt = audio.CreatedAt
            };
        }
    }
}

