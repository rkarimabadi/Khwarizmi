using Khwarizmi.Application.Interfaces;
using Khwarizmi.Domain.Entities;

namespace Khwarizmi.Infrastructure.Repositories.Persistence
{
    public class InMemoryPuzzleRepository : IPuzzleRepository
    {
        private readonly List<Puzzle> _puzzles = new();

        public Task<Puzzle> GetByIdAsync(Guid puzzleId)
        {
            return Task.FromResult(_puzzles.FirstOrDefault(p => p.Id == puzzleId));
        }

        public Task<Puzzle> GetActivePuzzleByPlayerIdAsync(Guid playerId)
        {
            return Task.FromResult(_puzzles.FirstOrDefault(p => !p.CompletedAt.HasValue));
        }

        public Task SaveAsync(Puzzle puzzle)
        {
            var existing = _puzzles.FirstOrDefault(p => p.Id == puzzle.Id);
            if (existing != null) _puzzles.Remove(existing);
            _puzzles.Add(puzzle);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid puzzleId)
        {
            var puzzle = _puzzles.FirstOrDefault(p => p.Id == puzzleId);
            if (puzzle != null) _puzzles.Remove(puzzle);
            return Task.CompletedTask;
        }
    }
}