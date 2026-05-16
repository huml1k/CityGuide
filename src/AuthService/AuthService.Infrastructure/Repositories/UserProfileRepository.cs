using AuthService.Application.Abstractions;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence;

namespace AuthService.Infrastructure.Repositories;

public sealed class UserProfileRepository : IUserProfileRepository
{
    private readonly UserDbContext _dbContext;

    public UserProfileRepository(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(UserProfile profile, CancellationToken cancellationToken)
    {
        await _dbContext.UserProfiles.AddAsync(profile, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
