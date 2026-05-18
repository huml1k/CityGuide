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

        public RejectRouteHandler( IRouteRepository routeRepository,IUnitOfWork unitOfWork)
        {
            _routeRepository = routeRepository;
            _unitOfWork = unitOfWork;
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

            return new RejectRouteResponse
            {
                RouteId = route.Id,
                Status = route.Status.ToString()
            };
        }
    }
}
