using ContentService.Application.Features.Files.Commands.DeleteAudioFile;
using ContentService.Application.Features.Routes.Commands.DeleteRoute;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Commands.DeleteRouteImage
{
    public class DeleteRouteImageHandler : IRequestHandler<DeleteRouteImageCommand>
    {
        private readonly IRouteImageRepository _routeImageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRouteImageHandler(IRouteImageRepository routeImageRepository, IUnitOfWork unitOfWork)
        {
            _routeImageRepository = routeImageRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(DeleteRouteImageCommand request, CancellationToken cancellationToken)
        {
            var route = await _routeImageRepository.GetByIdAsync(request.ImageId, cancellationToken);

            if (route is null)
            {
                throw new Exception("Image not found");
            }

            _routeImageRepository.Delete(route);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
