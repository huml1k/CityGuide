using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Dtos;
using UserService.Application.Interfaces.Service;
using UserService.Domain.Exeptions;

namespace UserService.Api.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserProfilesController : ControllerBase
{
    private readonly IUserProfileService _profileService;

    public UserProfilesController(IUserProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetMyProfile(CancellationToken ct)
    {
        try
        {
            return Ok(await _profileService.GetMyProfileAsync(ct));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpGet("{userId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserProfileDto>> GetById(Guid userId, CancellationToken ct)
    {
        try
        {
            return Ok(await _profileService.GetByIdAsync(userId, ct));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPut("me")]
    public async Task<IActionResult> Update([FromBody] UpdateUserProfileRequest request, CancellationToken ct)
    {
        try
        {
            await _profileService.UpdateMyProfileAsync(request, ct);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpDelete("me")]
    public async Task<IActionResult> Delete(CancellationToken ct)
    {
        try
        {
            await _profileService.DeleteMyProfileAsync(ct);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}