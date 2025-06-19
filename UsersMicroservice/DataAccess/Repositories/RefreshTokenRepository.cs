using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(UserDbContext context)
            : base(context)
        {
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
