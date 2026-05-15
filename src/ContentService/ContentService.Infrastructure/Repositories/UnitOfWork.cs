using ContentService.Domain.Interfaces.Repositories;
using ContentService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ContentDbContext _context;

        public UnitOfWork(ContentDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
