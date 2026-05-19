using ContentService.Application.Features.Routes.Commands.DeleteRoute;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using ContentService.Application.Interfaces;
using ContentService.Domain.Entities;

namespace ContentService.Application.Features.Files.Commands.DeleteAudioFile
{
    public class DeleteAudioFileHandler : IRequestHandler<DeleteAudioFileCommand>
    {
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;
        
        public DeleteAudioFileHandler(
            IAudioFileRepository audioFileRepository,
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorageService)
        {
            _audioFileRepository = audioFileRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }
        
        public async Task Handle(DeleteAudioFileCommand request, CancellationToken cancellationToken)
        {
            var audio = await _audioFileRepository.GetByIdAsync(request.AudioFileId, cancellationToken);
            if (audio is null)
                throw new KeyNotFoundException($"Audio file {request.AudioFileId} not found");
            var objectKey = GetObjectKey(audio);
            if (!string.IsNullOrWhiteSpace(objectKey))
            {
                await _fileStorageService.DeleteFileAsync(objectKey, cancellationToken);
            }
            _audioFileRepository.Delete(audio); // soft delete в БД
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        private static string GetObjectKey(AudioFile audio)
        {
            if (!string.IsNullOrWhiteSpace(audio.Path))
                return audio.Path;
            
            return $"{audio.Id}.{audio.FileExtension}";
        }
    }
}
