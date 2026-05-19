using MediatR;

namespace ContentService.Application.Features.RouteStats.Commands.DecrementFavoritesCount;

public class DecrementFavoritesCountCommand : IRequest
{
    public Guid RouteId { get; set; }
}