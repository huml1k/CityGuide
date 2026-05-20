using ContentService.Application.Common.Exceptions;
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

        public CreateRouteHandler(IRouteRepository routeRepository, IUnitOfWork unitOfWork)
        {
            _routeRepository = routeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateRouteResponse> Handle(CreateRouteCommand request, CancellationToken cancellationToken)
        {
            if (request.CreatorId == Guid.Empty)
            {
                throw new ValidationException(new Dictionary<string, string[]>
            {
                {
                    nameof(request.CreatorId),
                    new[] { "CreatorId is required." }
                }
            });
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ValidationException(new Dictionary<string, string[]>
            {
                {
                    nameof(request.Title),
                    new[] { "Title is required." }
                }
            });
            }

            if (request.DurationMinutes <= 0)
            {
                throw new ValidationException(new Dictionary<string, string[]>
            {
                {
                    nameof(request.DurationMinutes),
                    new[] { "DurationMinutes must be greater than zero." }
                }
            });
            }

            var route = new Route
            {
                Id = Guid.NewGuid(),
                CreatorId = request.CreatorId,
                Title = request.Title,
                Description = request.Description,
                DurationMinutes = request.DurationMinutes,
                GoogleMapsUrl = request.GoogleMapsUrl,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _routeRepository.AddAsync(
                    route,
                    cancellationToken);

                var saved = await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                if (saved <= 0)
                {
                    throw new BusinessRuleException(
                        "Failed to create route.");
                }
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException(
                    "An error occurred while creating route.",
                    ex);
            }

            return new CreateRouteResponse
            {
                Id = route.Id,
                Title = route.Title,
                CreatedAt = route.CreatedAt
            };
        }
    }
}
