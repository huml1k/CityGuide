using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using ContentService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ContentService.Application.Features.Files.Commands.UploadRouteImage
{
    public class UploadRouteImageCommandHandler : IRequestHandler<UploadRouteImageCommand, UploadRouteImageResponse>
    {
        private readonly IRouteImageRepository _imageRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _bucketName = "content-files";
        private readonly ILogger<UploadRouteImageCommandHandler> _logger;

        public UploadRouteImageCommandHandler(
            IRouteImageRepository imageRepository,
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorageService,
            ILogger<UploadRouteImageCommandHandler> logger,
            IRouteRepository routeRepository)
        {
            _imageRepository = imageRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
            _logger = logger;
            _routeRepository = routeRepository;
        }

        public async Task<UploadRouteImageResponse> Handle(UploadRouteImageCommand request, CancellationToken cancellationToken)
        {
            var extension = Path
                .GetExtension(request.File.FileName)
                .Replace(".", "");
            
            if (!await _routeRepository.ExistsAsync(request.RouteId, cancellationToken))
            {
                _logger.LogError($"ERROR: Route with id: {request.RouteId} does not exist in DB");
                throw new Exception($"ERROR: Route with id: {request.RouteId} does not exist in DB");
            }
            
            await _fileStorageService.UploadFileAsync(request.File.OpenReadStream(), request.File.Name, extension, cancellationToken);
            var filePath = _bucketName + "/" + request.File.FileName;
            
            var image = new RouteImage
            {
                Id = Guid.NewGuid(),
                RouteId = request.RouteId,
                FileExtension = extension,
                IsCover = request.IsCover,
                OrderIndex = request.OrderIndex,
                CreatedAt = DateTime.UtcNow,
                Path = filePath
            };

            await _imageRepository.AddAsync(
                image,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return new UploadRouteImageResponse
            {
                Id = image.Id,
                FileExtension = image.FileExtension,
                CreatedAt = image.CreatedAt
            };
        }
    }
}
