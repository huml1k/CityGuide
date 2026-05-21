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

            var content = await _fileStorageService.DownloadFileAsync(image.Path, cancellationToken);
            var extension = image.FileExtension.TrimStart('.').ToLowerInvariant();
            var contentType = extension switch
            {
                "jpg" or "jpeg" => "image/jpeg",
                "png" => "image/png",
                "webp" => "image/webp",
                "gif" => "image/gif",
                _ => $"image/{extension}"
            };

            return new GetRouteImageResponse
            {
                Content = content,
                FileName = $"{image.Id}.{image.FileExtension}",
                ContentType = contentType
            };
        }
    }
}
