using Khwarizmi.Domain.Events;
using Khwarizmi.Domain.ValueObjects;

namespace Khwarizmi.Domain.Entities
{
    public class Puzzle
    {
        public Guid Id { get; private set; }
        public string TargetEquation { get; private set; }
        public List<Tile> InventoryTiles { get; private set; }
        public List<Tile> PlacedTiles { get; private set; } = [];
        public BoardSize GridSize { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public Puzzle(string targetEquation, List<Tile> tiles, BoardSize gridSize)
        {
            Id = Guid.NewGuid();
            TargetEquation = targetEquation;
            InventoryTiles = tiles;
            GridSize = gridSize ?? throw new ArgumentNullException(nameof(gridSize));
            StartedAt = DateTime.UtcNow;
        }

        public void RotateTile(Guid tileId)
        {
            var tile = FindTile(tileId);
            if (tile == null) throw new InvalidOperationException("قطعه یافت نشد.");

            if (tile.Position != null)
            {
                var rotatedDimensions = new Dimensions(tile.Dimensions.HeightVar, tile.Dimensions.WidthVar);
                ValidatePlacement(tile, tile.Position, rotatedDimensions);
            }

            tile.Rotate();
        }

        public void AddTileToBoard(Guid tileId, Coordinate position)
        {
            var tile = InventoryTiles.FirstOrDefault(t => t.Id == tileId);
            if (tile == null) throw new InvalidOperationException("قطعه در مخزن نیست.");

            ValidatePlacement(tile, position, tile.Dimensions);

            tile.SetPosition(position);
            InventoryTiles.Remove(tile);
            PlacedTiles.Add(tile);

            _domainEvents.Add(new TilePlacedEvent(tileId, position));
        }

        public void MarkAsCompleted(Guid playerId, int difficulty, int extraUnits, bool isPerfect)
        {
            if (CompletedAt.HasValue) return;
            CompletedAt = DateTime.UtcNow;

            _domainEvents.Add(new PuzzleSolvedEvent(
                Id, 
                playerId,
                GetSolvingDuration()!.Value, 
                difficulty, 
                extraUnits, 
                isPerfect));
        }

        private void ValidatePlacement(Tile tile, Coordinate position, Dimensions dimensions)
        {
            if (position.X < 0 || position.Y < 0 ||
                position.X + dimensions.Width > GridSize.Columns || 
                position.Y + dimensions.Height > GridSize.Rows)
                throw new InvalidOperationException("خارج از مرز بورد.");

            if (IsOverlappingWithOthers(tile.Id, position, dimensions))
                throw new InvalidOperationException("هم‌پوشانی با سایر قطعات.");
        }

        private bool IsOverlappingWithOthers(Guid currentTileId, Coordinate pos, Dimensions dim)
        {
            foreach (var other in PlacedTiles)
            {
                if (other.Id == currentTileId) continue;
                bool overlapX = pos.X < other.Position!.X + other.Dimensions.Width && pos.X + dim.Width > other.Position.X;
                bool overlapY = pos.Y < other.Position.Y + other.Dimensions.Height && pos.Y + dim.Height > other.Position.Y;
                if (overlapX && overlapY) return true;
            }
            return false;
        }

        private Tile FindTile(Guid tileId) => 
            PlacedTiles.FirstOrDefault(t => t.Id == tileId) ?? InventoryTiles.FirstOrDefault(t => t.Id == tileId);

        public TimeSpan? GetSolvingDuration() => 
            CompletedAt.HasValue ? CompletedAt.Value - StartedAt : null;

        public void ClearEvents() => _domainEvents.Clear();
    }
}