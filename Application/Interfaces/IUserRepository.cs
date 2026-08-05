using Core.Entities;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
}
