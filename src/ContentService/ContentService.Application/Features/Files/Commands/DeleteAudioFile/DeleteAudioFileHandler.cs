using System.Security.Claims;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using ContentService.Application.Interfaces;
using ContentService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ContentService.Application.Features.Files.Commands.DeleteAudioFile
{
    public class DeleteAudioFileHandler : IRequestHandler<DeleteAudioFileCommand>
    {
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _accessor;
        private readonly IRouteRepository _routeRepository;
        
        public DeleteAudioFileHandler(
            IAudioFileRepository audioFileRepository,
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorageService,
            IHttpContextAccessor accessor,
            IRouteRepository routeRepository)
        {
            _audioFileRepository = audioFileRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
            _accessor = accessor;
            _routeRepository = routeRepository;
        }
        
        public async Task Handle(DeleteAudioFileCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();
            
            var audio = await _audioFileRepository.GetByIdAsync(request.AudioFileId, cancellationToken);
            
            if (audio is null)
                throw new KeyNotFoundException($"Image file {request.AudioFileId} not found");
            
            var currentRoute = await _routeRepository.GetByIdAsync(audio.RouteId, cancellationToken);
            
            if(currentRoute?.CreatorId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to delete this audio file");
            
            if (audio is null)
                throw new KeyNotFoundException($"Audio file {request.AudioFileId} not found");
            
            var objectKey = GetObjectKey(audio);
            if (!string.IsNullOrWhiteSpace(objectKey))
            {
                await _fileStorageService.DeleteFileAsync(objectKey, cancellationToken);
            }
            _audioFileRepository.Delete(audio);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        private static string GetObjectKey(AudioFile audio)
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
