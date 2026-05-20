using ContentService.Application.Common.Exceptions;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;

namespace ContentService.Application.Features.RouteStats.Commands.IncrementFavoritesCount;

public class IncrementFavoritesCountCommandHandler
    : IRequestHandler<IncrementFavoritesCountCommand>
{
    private readonly IRouteStatsRepository _routeStatsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IncrementFavoritesCountCommandHandler(
        IRouteStatsRepository routeStatsRepository,
        IUnitOfWork unitOfWork)
    {
        _routeStatsRepository = routeStatsRepository;
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
            throw new NotFoundException(
                $"Route stats for route '{request.RouteId}' were not found.");
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

        return;
    }
}