using Khwarizmi.Domain.Entities;

namespace Khwarizmi.Application.Interfaces
{
    public interface IPuzzleRepository
    {
        Task<Puzzle> GetByIdAsync(Guid puzzleId);
        Task<Puzzle> GetActivePuzzleByPlayerIdAsync(Guid playerId);
        Task SaveAsync(Puzzle puzzle);
        Task DeleteAsync(Guid puzzleId);
    }
}