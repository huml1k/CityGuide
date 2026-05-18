using ContentService.Application.DTOs;
using ContentService.Application.Interfaces;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.DeleteRoute
{
    public class DeleteRouteHandler : IRequestHandler<DeleteRouteCommand>
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKafkaEventPublisher _kafkaEventPublisher;

        public DeleteRouteHandler(IRouteRepository routeRepository,IUnitOfWork unitOfWork, IKafkaEventPublisher kafkaEventPublisher)
        {
            _routeRepository = routeRepository;
            _unitOfWork = unitOfWork;
            _kafkaEventPublisher = kafkaEventPublisher;
        }

        public async Task Handle(DeleteRouteCommand request, CancellationToken cancellationToken)
        {
            var route = await _routeRepository.GetByIdAsync(request.RouteId,cancellationToken);

            if (route is null)
            {
                throw new Exception("Route not found");
            }

            _routeRepository.Delete(route);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _kafkaEventPublisher.PublishAsync("content.routes", new ContentEventDto
            {
                EventId = Guid.NewGuid().ToString(),
                EventType = "deleted",
                RouteId = route.Id,
                CreatorId = route.CreatorId,
                RouteTitle = route.Title,
                Timestamp = DateTime.UtcNow
            }, cancellationToken);
        }
    }
}
