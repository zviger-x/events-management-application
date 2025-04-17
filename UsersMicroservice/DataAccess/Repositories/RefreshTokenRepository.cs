using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Repositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly ILogger<RefreshTokenRepository> _logger;

        public RefreshTokenRepository(UserDbContext context, ILogger<RefreshTokenRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task<RefreshToken> GetByRefreshTokenAsync(string refreshToken, CancellationToken token = default)
        {
            return await _context.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Token == refreshToken, token);
        }

        public async Task<RefreshToken> GetByUserIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.UserId == id, token);
        }
    }
}
