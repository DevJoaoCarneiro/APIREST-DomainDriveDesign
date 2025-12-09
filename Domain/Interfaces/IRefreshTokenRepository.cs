using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<List<RefreshToken>> GetAllActiveByUserIdAsync(Guid UserId);

        Task UpdateAsync(RefreshToken refreshToken);

        Task SaveChangesAsync();

        Task AddAsync(RefreshToken refreshToken);
    }
}
