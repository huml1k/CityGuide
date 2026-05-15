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
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRouteHandler(IRouteRepository routeRepository, IUnitOfWork unitOfWork)
        {
            _routeRepository = routeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateRouteResponse> Handle(UpdateRouteCommand request, CancellationToken cancellationToken)
        {
            var route = await _routeRepository.GetByIdAsync(request.RouteId, cancellationToken);

            if (route is null)
            {
                throw new Exception("Route not found");
            }

            route.Title = request.Title;
            route.Description = request.Description;
            route.DurationMinutes = request.DurationMinutes;
            route.Price = request.Price;
            route.GoogleMapsUrl = request.GoogleMapsUrl;
            route.UpdatedAt = DateTime.UtcNow;

            _routeRepository.Update(route);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateRouteResponse
            {
                Id = route.Id,
                UpdatedAt = route.UpdatedAt
            };
        }
    }
}
