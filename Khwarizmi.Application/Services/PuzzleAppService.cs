using Khwarizmi.Domain.Entities;
using Khwarizmi.Domain.Services;
using Khwarizmi.Domain.ValueObjects;
using Khwarizmi.Application.DTOs;
using Khwarizmi.Application.Interfaces;
using Khwarizmi.Domain.Events;

namespace Khwarizmi.Application.Services
{
    public class PuzzleAppService
    {
        private readonly PuzzleGenerator _generator;
        private readonly AlgebraicValidator _validator;
        private readonly DifficultyEvaluator _difficultyEvaluator;
        private readonly IPuzzleRepository _puzzleRepository;
        private readonly IPlayerRepository _playerRepository;

        public PuzzleAppService(
            PuzzleGenerator generator,
            AlgebraicValidator validator,
            DifficultyEvaluator difficultyEvaluator,
            IPuzzleRepository puzzleRepository,
            IPlayerRepository playerRepository)
        {
            _generator = generator;
            _validator = validator;
            _difficultyEvaluator = difficultyEvaluator;
            _puzzleRepository = puzzleRepository;
            _playerRepository = playerRepository;
        }
        public event Action<TilePlacedEvent> OnTilePlacedInUi;

        public async Task<PuzzleDto> PlaceTileAsync(Guid puzzleId, Guid tileId, int x, int y)
        {
            var puzzle = await _puzzleRepository.GetByIdAsync(puzzleId);

            puzzle.AddTileToBoard(tileId, new Coordinate(x, y));

            var placedEvent = puzzle.DomainEvents.OfType<TilePlacedEvent>().LastOrDefault();
            if (placedEvent != null)
            {
                OnTilePlacedInUi?.Invoke(placedEvent);
            }

            await _puzzleRepository.SaveAsync(puzzle);
            return MapToDto(puzzle);
        }

        public async Task<PuzzleDto> StartNewSessionAsync()
        {
            var player = await _playerRepository.GetCurrentPlayerAsync();
            if (player == null) throw new InvalidOperationException("بازیکن یافت نشد.");

            var existingPuzzle = await _puzzleRepository.GetActivePuzzleByPlayerIdAsync(player.Id);
            if (existingPuzzle != null)
            {
                return MapToDto(existingPuzzle);
            }

            int nextLevel = _difficultyEvaluator.SuggestNextDifficulty(
                player.CompletedPuzzlesLog,
                player.Level);

            var newPuzzle = _generator.GeneratePuzzle(nextLevel);

            await _puzzleRepository.SaveAsync(newPuzzle);

            return MapToDto(newPuzzle);
        }



        public async Task<PuzzleDto> RotateTileAsync(Guid puzzleId, Guid tileId)
        {
            var puzzle = await _puzzleRepository.GetByIdAsync(puzzleId);
            if (puzzle == null) throw new KeyNotFoundException("پازل یافت نشد.");

            puzzle.RotateTile(tileId);

            await _puzzleRepository.SaveAsync(puzzle);
            return MapToDto(puzzle);
        }

        public async Task<bool> CheckSolutionAsync(Guid puzzleId)
        {
            var puzzle = await _puzzleRepository.GetByIdAsync(puzzleId);
            var player = await _playerRepository.GetCurrentPlayerAsync();

            if (puzzle == null || player == null) return false;

            // ۱. بررسی صحت ریاضی چیدمان از طریق سرویس دامنه
            bool isValid = _validator.IsSolutionValid(
                puzzle.TargetEquation,
                puzzle.PlacedTiles);

            if (isValid)
            {
                bool isPerfect = _validator.IsPerfectGeometricalShape(puzzle.PlacedTiles);

                int extraUnits = CalculateExtraUnits(puzzle);

                puzzle.MarkAsCompleted(
                    player.Id,
                    player.Level,
                    extraUnits,
                    isPerfect);

                await ProcessDomainEventsAsync(puzzle, player);

                await _puzzleRepository.SaveAsync(puzzle);
            }

            return isValid;
        }

        private async Task ProcessDomainEventsAsync(Puzzle puzzle, Player player)
        {
            foreach (var domainEvent in puzzle.DomainEvents)
            {
                if (domainEvent is Khwarizmi.Domain.Events.PuzzleSolvedEvent solvedEvent)
                {
                    player.ApplyPuzzleCompletion(solvedEvent);
                }
            }

            puzzle.ClearEvents();
            await _playerRepository.SavePlayerAsync(player);
        }

        private int CalculateExtraUnits(Puzzle puzzle)
        {
            return puzzle.PlacedTiles.Count(t => t.Type == TileType.Unit);
        }

        private PuzzleDto MapToDto(Puzzle p)
        {
            return new PuzzleDto(
                p.Id,
                p.TargetEquation,
                p.InventoryTiles.Select(MapTile).ToList(),
                p.PlacedTiles.Select(MapTile).ToList(),
                p.GridSize.Columns,
                p.GridSize.Rows,
                p.CompletedAt.HasValue
            );
        }

        private TileDto MapTile(Tile t) => new TileDto(
            t.Id,
            t.Type.ToString(),
            t.Dimensions.Width,
            t.Dimensions.Height,
            t.Position?.X,
            t.Position?.Y,
            t.Dimensions.WidthVar.Type.ToString(),
            t.Dimensions.HeightVar.Type.ToString()
        );
    }
}