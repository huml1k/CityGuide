using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Interfaces.Service;

namespace UserService.Api.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Важно: требует авторизации
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;
    
    public FavoritesController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }
    
    [HttpPost("routes/{routeId:guid}")]
    public async Task<ActionResult<Guid>> AddFavorite(Guid routeId)
    {
        try
        {
            var favoriteId = await _favoriteService.AddFavoriteAsync(routeId);
            return Ok(new { id = favoriteId, message = "Added to favorites" });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
    
    [HttpDelete("routes/{routeId:guid}")]
    public async Task<IActionResult> RemoveFavorite(Guid routeId)
    {
        try
        {
            await _favoriteService.RemoveFavoriteAsync(routeId);
            return Ok(new { message = "Removed from favorites" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
    
    [HttpGet("routes/{routeId:guid}/check")]
    public async Task<ActionResult<bool>> IsFavorite(Guid routeId)
    {
        var isFavorite = await _favoriteService.IsFavoriteAsync(routeId);
        return Ok(new { routeId, isFavorite });
    }
    
    [HttpGet("routes")]
    public async Task<ActionResult<IReadOnlyCollection<Guid>>> GetMyFavorites()
    {
        var favorites = await _favoriteService.GetUserFavoriteRouteIdsAsync();
        return Ok(favorites);
    }
    
    [HttpPost("routes/batch-check")]
    public async Task<ActionResult<Dictionary<Guid, bool>>> CheckMultipleFavorites([FromBody] List<Guid> routeIds)
    {
        var result = await _favoriteService.CheckMultipleFavoritesAsync(routeIds);
        return Ok(result);
    }
}