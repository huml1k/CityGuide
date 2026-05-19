using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using ContentService.Application.Features.Files.Queries.GetAudioFile;
using ContentService.Application.Interfaces;

namespace ContentService.Application.Features.Files.Queries.GetRouteImage
{
    public class GetRouteImageHandler : IRequestHandler<GetRouteImageQuery, GetRouteImageResponse>
    {
        private readonly IRouteImageRepository _imageRepository;
        private readonly IFileStorageService _fileStorageService;
        public GetRouteImageHandler(
            IRouteImageRepository imageRepository,
            IFileStorageService fileStorageService)
        {
            _imageRepository = imageRepository;
            _fileStorageService = fileStorageService;
        }
        
        public async Task<GetRouteImageResponse> Handle(
            GetRouteImageQuery request,
            CancellationToken cancellationToken)
        {
            var image = await _imageRepository.GetByIdAsync(request.ImageId, cancellationToken);
            if (image is null)
                throw new KeyNotFoundException($"Image {request.ImageId} not found");
            if (string.IsNullOrWhiteSpace(image.Path))
                throw new InvalidOperationException("Image has no storage path");
            var expiry = TimeSpan.FromMinutes(request.ExpiryMinutes ?? 15);
            var url = await _fileStorageService.GetFileUrlAsync(image.Path, cancellationToken, expiry);
            return new GetRouteImageResponse
            {
                Url = url,
                ExpiresAt = DateTime.UtcNow.Add(expiry),
                FileName = $"{image.Id}.{image.FileExtension}",
                ContentType = $"image/{image.FileExtension}"
            };
        }
    }
}
