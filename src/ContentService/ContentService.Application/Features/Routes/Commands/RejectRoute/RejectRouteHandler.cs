using ContentService.Application.DTOs;
using ContentService.Application.Interfaces;
using ContentService.Domain.Enums;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.RejectRoute
{
    public class RejectRouteHandler : IRequestHandler< RejectRouteCommand, RejectRouteResponse>
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKafkaEventPublisher _kafkaEventPublisher;

        public RejectRouteHandler( IRouteRepository routeRepository, IUnitOfWork unitOfWork, IKafkaEventPublisher kafkaEventPublisher)
        {
            _routeRepository = routeRepository;
            _unitOfWork = unitOfWork;
            _kafkaEventPublisher = kafkaEventPublisher;
        }

        public async Task<RejectRouteResponse> Handle( RejectRouteCommand request, CancellationToken cancellationToken)
        {
            var route = await _routeRepository.GetByIdAsync( request.RouteId,cancellationToken);

            if (route is null) 
            {
                throw new Exception("Route not found");
            }

            route.Status = RouteStatus.rejected;

            _routeRepository.Update(route);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _kafkaEventPublisher.PublishAsync("content.routes", new ContentEventDto
            {
                EventId = Guid.NewGuid().ToString(),
                EventType = "rejected",
                RouteId = route.Id,
                CreatorId = route.CreatorId,
                RouteTitle = route.Title,
                Timestamp = DateTime.UtcNow
            }, cancellationToken);

            return new RejectRouteResponse
            {
                RouteId = route.Id,
                Status = route.Status.ToString()
            };
        }
    }
}
