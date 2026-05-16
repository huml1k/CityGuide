using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using ContentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ContentService.Infrastructure.Repositories
{
    public class AudioFileRepository : IAudioFileRepository
    {
        private readonly ContentDbContext _context;

        public AudioFileRepository(ContentDbContext context)
        {
            _context = context; 

        }

        public async Task AddAsync(AudioFile audioFile, CancellationToken cancellationToken = default)
        {
            await _context.AudioFiles.AddAsync(audioFile, cancellationToken);
        }

        public void Delete(AudioFile audioFile)
        {
            audioFile.DeletedAt = DateTime.UtcNow;

            _context.AudioFiles.Update(audioFile);
        }

        public async Task<AudioFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.AudioFiles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyCollection<AudioFile>> GetByRouteIdAsync(Guid routeId, CancellationToken cancellationToken = default)
        {
            return await _context.AudioFiles.Where(x => x.RouteId == routeId).ToListAsync(cancellationToken);
        }
    }
}
