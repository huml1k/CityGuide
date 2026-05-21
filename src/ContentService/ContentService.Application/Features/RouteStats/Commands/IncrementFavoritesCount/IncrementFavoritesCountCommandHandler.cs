using ContentService.Application.Common.Exceptions;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;

namespace ContentService.Application.Features.RouteStats.Commands.IncrementFavoritesCount;

public class IncrementFavoritesCountCommandHandler
    : IRequestHandler<IncrementFavoritesCountCommand>
{
    private readonly IRouteStatsRepository _routeStatsRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IncrementFavoritesCountCommandHandler(
        IRouteStatsRepository routeStatsRepository,
        IRouteRepository routeRepository,
        IUnitOfWork unitOfWork)
    {
        _routeStatsRepository = routeStatsRepository;
        _routeRepository = routeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        IncrementFavoritesCountCommand request,
        CancellationToken cancellationToken)
    {
        var stats = await _routeStatsRepository.GetByRouteIdAsync(
            request.RouteId,
            cancellationToken);

        if (stats is null)
        {
            if (!await _routeRepository.ExistsAsync(request.RouteId, cancellationToken))
            {
                throw new RouteNotFoundException(request.RouteId);
            }

            stats = new Domain.Entities.RouteStats
            {
                RouteId = request.RouteId,
                FavoritesCount = 1
            };

            await _routeStatsRepository.AddAsync(stats, cancellationToken);

            var created = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (created <= 0)
            {
                throw new BusinessRuleException("Failed to update favorites count.");
            }

            return;
        }

        if (stats.FavoritesCount < 0)
        {
            throw new BusinessRuleException(
                "Favorites count cannot be negative.");
        }

        stats.FavoritesCount++;

        _routeStatsRepository.Update(stats);

        var saved = await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        if (saved <= 0)
        {
            throw new BusinessRuleException(
                "Failed to update favorites count.");
        }
    }
}