using Khwarizmi.Domain.Entities;
using Khwarizmi.Domain.ValueObjects;

namespace Khwarizmi.Domain.Services
{
    public class PuzzleGenerator
    {
        private readonly Random _random = new();

        public Puzzle GeneratePuzzle(int difficultyLevel)
        {
            var (nx, ny, constant) = GetParametersForDifficulty(difficultyLevel);

            var varX = new Variable(VariableType.X, 4);
            var varY = new Variable(VariableType.Y, 6);
            var varUnit = new Variable(VariableType.None, 1);

            var inventory = new List<Tile>();

            for (int i = 0; i < nx * nx; i++) 
                inventory.Add(new Tile(TileType.Square, new Dimensions(varX, varX)));
            
            for (int i = 0; i < ny * ny; i++) 
                inventory.Add(new Tile(TileType.Square, new Dimensions(varY, varY)));

            int xyCount = 2 * nx * ny;
            for (int i = 0; i < xyCount; i++)
                inventory.Add(new Tile(TileType.Rectangle, new Dimensions(varX, varY)));

            int xUnits = 2 * nx * constant;
            for (int i = 0; i < xUnits; i++)
                inventory.Add(new Tile(TileType.Rectangle, new Dimensions(varX, varUnit)));

            int yUnits = 2 * ny * constant;
            for (int i = 0; i < yUnits; i++)
                inventory.Add(new Tile(TileType.Rectangle, new Dimensions(varY, varUnit)));

            int totalUnits = constant * constant;
            for (int i = 0; i < totalUnits; i++)
                inventory.Add(new Tile(TileType.Unit, new Dimensions(varUnit, varUnit)));

            string equation = BuildEquationString(nx, ny, constant);

            return new Puzzle(equation, inventory, new BoardSize(10, 10));
        }

        private (int nx, int ny, int constant) GetParametersForDifficulty(int level)
        {
            return level switch
            {
                1 => (1, 0, _random.Next(1, 4)), // (x+C)^2
                2 => (1, 1, 0),                 // (x+y)^2
                3 => (1, 1, _random.Next(1, 3)), // (x+y+C)^2
                4 => (2, 1, _random.Next(1, 3)), // (2x+y+C)^2
                _ => (1, 0, 1)
            };
        }

        private string BuildEquationString(int nx, int ny, int c)
        {
            return $"({(nx > 1 ? nx.ToString() : "")}x + {(ny > 0 ? (ny > 1 ? ny.ToString() : "") + "y + " : "")}{c})^2";
        }
    }
}