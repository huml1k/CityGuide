using ContentService.Domain.Interfaces.Repositories;
using MediatR;

namespace ContentService.Application.Features.RouteStats.Commands.DecrementFavoritesCount;

public class DecrementFavoritesCountCommandHandler
    : IRequestHandler<DecrementFavoritesCountCommand>
{
    private readonly IRouteStatsRepository _routeStatsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DecrementFavoritesCountCommandHandler(
        IRouteStatsRepository routeStatsRepository,
        IUnitOfWork unitOfWork)
    {
        _routeStatsRepository = routeStatsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        DecrementFavoritesCountCommand request,
        CancellationToken cancellationToken)
    {
        var stats = await _routeStatsRepository.GetByRouteIdAsync(
            request.RouteId,
            cancellationToken);

        if (stats is null)
        {
            throw new Exception("Route stats not found");
        }

        if (stats.FavoritesCount > 0)
        {
            stats.FavoritesCount--;
        }

        _routeStatsRepository.Update(stats);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}