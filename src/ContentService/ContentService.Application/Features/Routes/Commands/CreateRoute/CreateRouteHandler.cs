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

            await _routeRepository.AddAsync(route,cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateRouteResponse
            {
                Id = route.Id,
                Title = route.Title,
                CreatedAt = route.CreatedAt
            };
        }
    }
}
