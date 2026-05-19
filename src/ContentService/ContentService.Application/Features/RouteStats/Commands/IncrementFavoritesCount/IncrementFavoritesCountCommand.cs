using MediatR;

namespace ContentService.Application.Features.RouteStats.Commands.IncrementFavoritesCount;

public class IncrementFavoritesCountCommand : IRequest
{
    public Guid RouteId { get; set; }
}