using ContentService.Application.Common.Exceptions;
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
                throw new RouteNotFoundException(request.RouteId);
            }

            if (route.Status == RouteStatus.rejected)
            {
                throw new BusinessRuleException(
                    "Route is already rejected.");
            }

            if (route.DeletedAt.HasValue)
            {
                throw new BusinessRuleException(
                    "Deleted route cannot be rejected.");
            }

            route.Status = RouteStatus.rejected;

            try
            {
                _routeRepository.Update(route);

                var saved = await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                if (saved <= 0)
                {
                    throw new BusinessRuleException(
                        "Failed to reject route.");
                }
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException(
                    "An error occurred while rejecting route.",
                    ex);
            }

            try
            {
                await _kafkaEventPublisher.PublishAsync(
                    "content.routes",
                    new ContentEventDto
                    {
                        EventId = Guid.NewGuid().ToString(),
                        EventType = "rejected",
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
                    "Route rejected but Kafka event publishing failed.",
                    ex);
            }

            return new RejectRouteResponse
            {
                RouteId = route.Id,
                Status = route.Status.ToString()
            };
        }
    }
}
