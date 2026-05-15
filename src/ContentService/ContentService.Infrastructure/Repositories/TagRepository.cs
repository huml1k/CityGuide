using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using ContentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ContentService.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ContentDbContext _context;

        public TagRepository(ContentDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Tags.AnyAsync(x => x.Name == name, cancellationToken);
        }

        public async Task<IReadOnlyCollection<Tag>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Tags.ToListAsync(cancellationToken);
        }

        public async Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Tags.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Tags.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        }
    }
}
