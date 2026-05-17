using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Exeptions;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories;

public class UserProfilesRepository : IUserProfilesRepository
{
    private UserDbContext _context;
    
    public UserProfilesRepository(UserDbContext context)
    {
        _context = context;
    }
    
    public async Task<IReadOnlyCollection<UserProfile>> GetAllAsync()
    {
        return await _context.UserProfiles.ToListAsync();
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id)
    {
        return await _context.UserProfiles.FindAsync(id);
    }

    public async Task<Guid> CreateAsync(UserProfile profile)
    {
        var res = await _context.UserProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        return res.Entity.UserId;
    }

    public async Task UpdateAsync(UserProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        
        var existingProfile = await GetByIdAsync(profile.UserId);
        if (existingProfile == null)
        {
            throw new NotFoundException(
                $"UserProfile with ID {profile.UserId} not found"
            );
        }
        
        existingProfile.FullName = profile.FullName ?? existingProfile.FullName;
        existingProfile.Bio = profile.Bio ?? existingProfile.Bio;
        existingProfile.Phone = profile.Phone ?? existingProfile.Phone;
        existingProfile.AvatarUrl = profile.AvatarUrl ?? existingProfile.AvatarUrl;
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}