using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using ContentService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ContentDbContext _context;

        public TagRepository(ContentDbContext context)
        {
            _context = context;
        }

        public Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<Tag>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
