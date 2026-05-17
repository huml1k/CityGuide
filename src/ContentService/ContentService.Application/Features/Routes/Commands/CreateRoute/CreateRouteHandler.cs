using ContentService.Application.DTOs;
using ContentService.Application.Interfaces;
using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.CreateRoute
{
    public class CreateRouteHandler : IRequestHandler<CreateRouteCommand, CreateRouteResponse>
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKafkaEventPublisher _kafkaEventPublisher;

        public CreateRouteHandler(IRouteRepository routeRepository, IUnitOfWork unitOfWork, IKafkaEventPublisher kafkaEventPublisher)
        {
            _kafkaEventPublisher = kafkaEventPublisher;
            _routeRepository = routeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateRouteResponse> Handle(CreateRouteCommand request, CancellationToken cancellationToken)
        {
            var route = new Route
            {
                Id = Guid.NewGuid(),
                CreatorId = request.CreatorId,
                Title = request.Title,
                Description = request.Description,
                DurationMinutes = request.DurationMinutes,
                Price = request.Price,
                GoogleMapsUrl = request.GoogleMapsUrl,
                Status = "draft",
                CreatedAt = DateTime.UtcNow
            };

            await _routeRepository.AddAsync(route,cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _kafkaEventPublisher.PublishAsync("content.events", new ContentEventDto 
            {
                EventType = "approved",
                RouteId = route.Id,
                CreatorId = route.CreatorId,
                RouteTitle = route.Title,
                Timestamp = DateTime.UtcNow
            }, cancellationToken);

            return new CreateRouteResponse
            {
                Id = route.Id,
                Title = route.Title,
                CreatedAt = route.CreatedAt
            };
        }
    }
}
