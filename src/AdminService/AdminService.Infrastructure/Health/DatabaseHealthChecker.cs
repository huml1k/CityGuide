using AdminService.Application.Interfaces;
using AdminService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Infrastructure.Health;

public sealed class DatabaseHealthChecker : IDatabaseHealthChecker
{
    private readonly AdminDbContext _dbContext;

    public DatabaseHealthChecker(AdminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> CanConnectAsync(CancellationToken cancellationToken = default) =>
        _dbContext.Database.CanConnectAsync(cancellationToken);
}
