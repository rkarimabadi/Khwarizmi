using Khwarizmi.Domain.Entities;

namespace Khwarizmi.Application.Interfaces
{
    public interface IPlayerRepository
    {
        Task<Player> GetCurrentPlayerAsync();
        Task SavePlayerAsync(Player player);
    }
}