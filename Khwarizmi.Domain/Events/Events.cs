using Khwarizmi.Domain.ValueObjects;

namespace Khwarizmi.Domain.Events
{
    public interface IDomainEvent { }

    public record PuzzleSolvedEvent(
        Guid PuzzleId,
        Guid PlayerId,
        TimeSpan Duration,
        int Difficulty,
        int ExtraUnitsUsed,
        bool IsPerfectSquare) : IDomainEvent;

    public record LevelUpEvent(
        Guid PlayerId,
        int NewLevel,
        string Username) : IDomainEvent;

    public record TilePlacedEvent(
        Guid TileId,
        Coordinate Position) : IDomainEvent;
}