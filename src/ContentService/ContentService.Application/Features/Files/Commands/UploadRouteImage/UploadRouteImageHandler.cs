using ContentService.Application.Common.Exceptions;
using ContentService.Application.Interfaces;
using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

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
            if (request.File is null)
            {
                throw new ValidationException(
                    new Dictionary<string, string[]>
                    {
                {
                    nameof(request.File),
                    new[] { "Image file is required." }
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
                    new[] { "Image file is empty." }
                }
                    });
            }

            if (!await _routeRepository.ExistsAsync(
                    request.RouteId,
                    cancellationToken))
            {
                throw new RouteNotFoundException(
                    request.RouteId);
            }

            var extension = Path.GetExtension(
                request.File.FileName);

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new InvalidFileException(
                    "Image file extension is invalid.");
            }

            if (string.IsNullOrWhiteSpace(request.File.ContentType))
            {
                throw new InvalidFileException(
                    "Image content type is invalid.");
            }

            if (request.OrderIndex < 0)
            {
                throw new ValidationException(
                    new Dictionary<string, string[]>
                    {
                {
                    nameof(request.OrderIndex),
                    new[] { "OrderIndex cannot be negative." }
                }
                    });
            }

            var image = new RouteImage
            {
                Id = Guid.NewGuid(),
                RouteId = request.RouteId,
                IsCover = request.IsCover,
                FileExtension = extension.TrimStart('.'),
                CreatedAt = DateTime.UtcNow,
                OrderIndex = request.OrderIndex,
            };

            var objectKey = $"{image.Id}{extension}";

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
                    "Failed to upload image file.",
                    ex);
            }

            image.Path = objectKey;

            try
            {
                await _imageRepository.AddAsync(
                    image,
                    cancellationToken);

                var saved = await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                if (saved <= 0)
                {
                    throw new BusinessRuleException(
                        "Failed to save image metadata.");
                }
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException(
                    "An error occurred while saving image metadata.",
                    ex);
            }

            return new UploadRouteImageResponse
            {
                Id = image.Id,
                FileExtension = image.FileExtension,
                CreatedAt = image.CreatedAt
            };
        }
    }
}
