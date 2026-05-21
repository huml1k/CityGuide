using ContentService.Application.Common.Exceptions;
using ContentService.Application.DTOs;
using ContentService.Application.Interfaces;
using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.UpdateRoute
{
    public class UpdateRouteHandler : IRequestHandler<UpdateRouteCommand, UpdateRouteResponse>
    {
        private readonly IRouteRepository _routeRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKafkaEventPublisher _kafkaEventPublisher;

        public UpdateRouteHandler(IRouteRepository routeRepository, ITagRepository tagRepository, IUnitOfWork unitOfWork, IKafkaEventPublisher kafkaEventPublisher)
        {
            _kafkaEventPublisher = kafkaEventPublisher;
            _tagRepository = tagRepository;
            _routeRepository = routeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateRouteResponse> Handle(UpdateRouteCommand request, CancellationToken cancellationToken)
        {
            var route = await _routeRepository.GetByIdAsync(request.RouteId, cancellationToken);
            if (route is null)
            {
                throw new RouteNotFoundException(request.RouteId);
            }

            if (route.DeletedAt.HasValue)
            {
                throw new BusinessRuleException(
                    "Deleted route cannot be updated.");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ValidationException(
                    new Dictionary<string, string[]>
                    {
                {
                    nameof(request.Title),
                    new[] { "Title is required." }
                }
                    });
            }

            if (request.DurationMinutes <= 0)
            {
                throw new ValidationException(
                    new Dictionary<string, string[]>
                    {
                {
                    nameof(request.DurationMinutes),
                    new[] { "DurationMinutes must be greater than zero." }
                }
                    });
            }

            var tags = await _tagRepository.GetByIdsAsync(request.TagIds, cancellationToken);

            if (tags.Count != request.TagIds.Count)
            {
                throw new ValidationException(
                    new Dictionary<string, string[]>
                    {
            {
                    nameof(request.TagIds),
                    new[] { "One or more tags do not exist." }
            }
                    });
            }

            route.RouteTags.Clear();

            route.RouteTags = request.TagIds
                .Select(tagId => new RouteTag
                {
                    RouteId = route.Id,
                    TagId = tagId
                })
                .ToList();

            route.Title = request.Title;
            route.Description = request.Description;
            route.DurationMinutes = request.DurationMinutes;
            route.GoogleMapsUrl = request.GoogleMapsUrl;
            route.UpdatedAt = DateTime.UtcNow;

            try
            {
                _routeRepository.Update(route);

                var saved = await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                if (saved <= 0)
                {
                    throw new BusinessRuleException(
                        "Failed to update route.");
                }
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException(
                    "An error occurred while updating route.",
                    ex);
            }

            try
            {
                await _kafkaEventPublisher.PublishAsync(
                    "content.routes",
                    new ContentEventDto
                    {
                        EventId = Guid.NewGuid().ToString(),
                        EventType = "updated",
                        RouteId = request.RouteId,
                        CreatorId = route.CreatorId,
                        RouteTitle = route.Title,
                        Timestamp = DateTime.UtcNow
                    },
                    cancellationToken);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException(
                    "Route updated but Kafka event publishing failed.",
                    ex);
            }

            return new UpdateRouteResponse
            {
                Id = route.Id,
                UpdatedAt = route.UpdatedAt
            };
        }
    }
}
