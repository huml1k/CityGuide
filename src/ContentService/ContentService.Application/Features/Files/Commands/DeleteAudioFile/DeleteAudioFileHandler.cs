using ContentService.Application.Features.Routes.Commands.DeleteRoute;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Commands.DeleteAudioFile
{
    public class DeleteAudioFileHandler : IRequestHandler<DeleteAudioFileCommand>
    {
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAudioFileHandler(IAudioFileRepository audioFileRepository, IUnitOfWork unitOfWork)
        {
            _audioFileRepository = audioFileRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(DeleteAudioFileCommand request, CancellationToken cancellationToken)
        {
            var route = await _audioFileRepository.GetByIdAsync(request.AudioFileId, cancellationToken);

            if (route is null)
            {
                throw new Exception("Audio not found");
            }

            _audioFileRepository.Delete(route);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
