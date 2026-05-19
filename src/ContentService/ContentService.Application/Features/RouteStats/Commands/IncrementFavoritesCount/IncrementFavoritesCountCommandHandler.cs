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
            throw new Exception("Route stats not found");
        }

        stats.FavoritesCount++;

        _routeStatsRepository.Update(stats);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}