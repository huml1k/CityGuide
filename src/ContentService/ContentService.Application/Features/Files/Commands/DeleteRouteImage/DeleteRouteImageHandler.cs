using ContentService.Application.Features.Files.Commands.DeleteAudioFile;
using ContentService.Application.Features.Routes.Commands.DeleteRoute;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using ContentService.Application.Interfaces;
using ContentService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ContentService.Application.Features.Files.Commands.DeleteRouteImage
{
    public class DeleteRouteImageHandler : IRequestHandler<DeleteRouteImageCommand>
    {
        private readonly IRouteImageRepository _routeImageRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _accessor;

        public DeleteRouteImageHandler(
            IRouteImageRepository routeImageRepository, 
            IUnitOfWork unitOfWork, 
            IFileStorageService fileStorageService,
            IHttpContextAccessor accessor,
            IRouteRepository routeRepository)
        {
            _routeImageRepository = routeImageRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
            _accessor = accessor;
            _routeRepository = routeRepository;
        }
        
        public async Task Handle(DeleteRouteImageCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();
            
            var image = await _routeImageRepository.GetByIdAsync(request.ImageId, cancellationToken);
            if (image is null)
                throw new KeyNotFoundException($"Image file {request.ImageId} not found");
            
            var currentRoute = await _routeRepository.GetByIdAsync(image.RouteId, cancellationToken);
            
            if(currentRoute?.CreatorId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to delete this route image");
            
            var objectKey = GetObjectKey(image);
            if (!string.IsNullOrWhiteSpace(objectKey))
            {
                await _fileStorageService.DeleteFileAsync(objectKey, cancellationToken);
            }
            _routeImageRepository.Delete(image);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        private static string GetObjectKey(RouteImage audio)
        {
            if (!string.IsNullOrWhiteSpace(audio.Path))
                return audio.Path;
            
            return $"{audio.Id}.{audio.FileExtension}";
        }

        private Guid GetCurrentUserId()
        {
            var user = _accessor.HttpContext?.User;

            var userIdClaim =
                user?.FindFirst(JwtRegisteredClaimNames.Sub)
                ?? user?.FindFirst("sub")
                ?? user?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("User is not authenticated or invalid user ID");

            return userId;
        }
    }
}
