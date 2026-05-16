using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Commands.UploadRouteImage
{
    public class UploadRouteImageCommandHandler : IRequestHandler<UploadRouteImageCommand, UploadRouteImageResponse>
    {
        private readonly IRouteImageRepository _imageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UploadRouteImageCommandHandler(
            IRouteImageRepository imageRepository,
            IUnitOfWork unitOfWork)
        {
            _imageRepository = imageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UploadRouteImageResponse> Handle(UploadRouteImageCommand request, CancellationToken cancellationToken)
        {
            var extension = Path
                .GetExtension(request.File.FileName)
                .Replace(".", "");

            var image = new RouteImage
            {
                Id = Guid.NewGuid(),
                RouteId = request.RouteId,
                FileExtension = extension,
                IsCover = request.IsCover,
                OrderIndex = request.OrderIndex,
                CreatedAt = DateTime.UtcNow
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
