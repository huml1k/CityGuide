using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using ContentService.Application.Interfaces;

namespace ContentService.Application.Features.Files.Queries.GetAudioFile
{
    public class GetAudioFileHandler : IRequestHandler<GetAudioFileQuery, GetAudioFileResponse>
    {
        private readonly IRouteImageRepository _imageRepository;
        private readonly IFileStorageService _fileStorageService;
        
        public GetAudioFileHandler(
            IRouteImageRepository imageRepository,
            IFileStorageService fileStorageService)
        {
            _imageRepository = imageRepository;
            _fileStorageService = fileStorageService;
        }
        
        public async Task<GetAudioFileResponse> Handle(
            GetAudioFileQuery request,
            CancellationToken cancellationToken)
        {
            var image = await _imageRepository.GetByIdAsync(request.AudioFileId, cancellationToken);
            
            if (image is null)
                throw new KeyNotFoundException($"Image {request.AudioFileId} not found");
            
            if (string.IsNullOrWhiteSpace(image.Path))
                throw new InvalidOperationException("Image has no storage path");
            
            var expiry = TimeSpan.FromMinutes(request.ExpiryMinutes ?? 15);
            var url = await _fileStorageService.GetFileUrlAsync(image.Path, cancellationToken, expiry);
            
            return new GetAudioFileResponse
            {
                Url = url,
                ExpiresAt = DateTime.UtcNow.Add(expiry),
                FileName = $"{image.Id}.{image.FileExtension}",
                ContentType = $"image/{image.FileExtension}"
            };
        }
    }
}
