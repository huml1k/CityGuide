using ContentService.Application.Common.Exceptions;
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
                throw new RouteNotFoundException(request.RouteId);
            }

            if (route.DeletedAt.HasValue)
            {
                throw new BusinessRuleException(
                    "Route is already deleted.");
            }

            try
            {
                _routeRepository.Delete(route);

                var saved = await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                if (saved <= 0)
                {
                    throw new BusinessRuleException(
                        "Failed to delete route.");
                }
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException(
                    "An error occurred while deleting route.",
                    ex);
            }

            try
            {
                await _kafkaEventPublisher.PublishAsync(
                    "content.routes",
                    new ContentEventDto
                    {
                        EventId = Guid.NewGuid().ToString(),
                        EventType = "deleted",
                        RouteId = route.Id,
                        CreatorId = route.CreatorId,
                        RouteTitle = route.Title,
                        Timestamp = DateTime.UtcNow
                    },
                    cancellationToken);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException(
                    "Route deleted but Kafka event publishing failed.",
                    ex);
            }
        }
    }
}
