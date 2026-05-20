using ContentService.Application.Common.Exceptions;
using ContentService.Application.DTOs;
using ContentService.Application.Interfaces;
using ContentService.Domain.Enums;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.ApproveRoute
{
    public class ApproveRouteHandler : IRequestHandler<ApproveRouteCommand, ApproveRouteResponse>
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKafkaEventPublisher _kafkaEventPublisher;

        public ApproveRouteHandler(IRouteRepository routeRepository, IUnitOfWork unitOfWork, IKafkaEventPublisher kafkaEventPublisher)
        {
            _routeRepository = routeRepository;
            _unitOfWork = unitOfWork;
            _kafkaEventPublisher = kafkaEventPublisher;
        }


        public async Task<ApproveRouteResponse> Handle(
            ApproveRouteCommand request,
            CancellationToken cancellationToken)
        {
            var route = await _routeRepository
                .GetByIdAsync(
                    request.RouteId,
                    cancellationToken);

            if (route is null)
            {
                throw new RouteNotFoundException(request.RouteId);
            }

            if (route.Status == RouteStatus.approved)
            {
                throw new BusinessRuleException(
                    "Route is already approved.");
            }

            if (route.DeletedAt.HasValue)
            {
                throw new BusinessRuleException(
                    "Deleted route cannot be approved.");
            }


            route.Status = RouteStatus.approved;

            _routeRepository.Update(route);

            var saved = await _unitOfWork.SaveChangesAsync(
                cancellationToken);


            if (saved <= 0)
            {
                throw new BusinessRuleException(
                    "Failed to approve route.");
            }

            await _kafkaEventPublisher.PublishAsync("content.routes", new ContentEventDto
            {
                EventId = Guid.NewGuid().ToString(),
                EventType = "approved",
                RouteId = route.Id,
                CreatorId = route.CreatorId,
                RouteTitle = route.Title,
                Timestamp = DateTime.UtcNow
            }, cancellationToken);

            return new ApproveRouteResponse
            {
                RouteId = route.Id,
                Status = route.Status.ToString()
            };
        }
    }
}
