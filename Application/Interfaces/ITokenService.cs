using Domain.entities;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);

        RefreshToken GenerateRefreshToken(Guid userId, string ipAddress);
    }
}
