using ContentService.Application.Features.Files.Commands.DeleteAudioFile;
using ContentService.Application.Features.Routes.Commands.DeleteRoute;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using ContentService.Application.Interfaces;
using ContentService.Domain.Entities;

namespace ContentService.Application.Features.Files.Commands.DeleteRouteImage
{
    public class DeleteRouteImageHandler : IRequestHandler<DeleteRouteImageCommand>
    {
        private readonly IRouteImageRepository _routeImageRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRouteImageHandler(IRouteImageRepository routeImageRepository, IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _routeImageRepository = routeImageRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }
        
        public async Task Handle(DeleteRouteImageCommand request, CancellationToken cancellationToken)
        {
            var audio = await _routeImageRepository.GetByIdAsync(request.ImageId, cancellationToken);
            if (audio is null)
                throw new KeyNotFoundException($"Audio file {request.ImageId} not found");
            
            var objectKey = GetObjectKey(audio);
            if (!string.IsNullOrWhiteSpace(objectKey))
            {
                await _fileStorageService.DeleteFileAsync(objectKey, cancellationToken);
            }
            _routeImageRepository.Delete(audio); // soft delete в БД
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        private static string GetObjectKey(RouteImage audio)
        {
            if (!string.IsNullOrWhiteSpace(audio.Path))
                return audio.Path;
            
            return $"{audio.Id}.{audio.FileExtension}";
        }
    }
}
