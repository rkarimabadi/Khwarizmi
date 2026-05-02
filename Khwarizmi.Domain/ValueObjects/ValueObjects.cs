namespace Khwarizmi.Domain.ValueObjects
{
    public record Coordinate(int X, int Y);

    public record BoardSize(int Columns, int Rows);

    public record Variable
    {
        public VariableType Type { get; init; }
        public int Value { get; init; }

        public Variable(VariableType type, int value)
        {
            if (type != VariableType.None && value <= 1)
                throw new ArgumentException("Variable value must be greater than 1 to avoid confusion with unit squares.");

            if (type == VariableType.None && value != 1)
                throw new ArgumentException("Variables of type None must always have a value of 1.");

            Type = type;
            Value = value;
        }
    }

    public record Dimensions
    {
        public Variable WidthVar { get; init; }
        public Variable HeightVar { get; init; }

        public int Width => WidthVar.Value;
        public int Height => HeightVar.Value;

        public Dimensions(Variable widthVar, Variable heightVar)
        {
            WidthVar = widthVar ?? throw new ArgumentNullException(nameof(widthVar));
            HeightVar = heightVar ?? throw new ArgumentNullException(nameof(heightVar));
        }

        public bool IsSquare => WidthVar.Type == HeightVar.Type && Width == Height;
    }

    public enum VariableType
    {
        None,
        X,
        Y,
        Z,
        W
    }

    public enum TileType
    {
        Square,
        Rectangle,
        Unit
    }

    public record PuzzleRecord
    {
        public Guid PuzzleId { get; init; }
        public TimeSpan SolveDuration { get; init; }
        public int DifficultyLevel { get; init; }
        public int ExtraUnitsUsed { get; init; }
        public bool IsPerfectSquare { get; init; }
        public DateTime Timestamp { get; init; }

        public PuzzleRecord(Guid puzzleId, TimeSpan duration, int difficulty, int extraUnits, bool isPerfect)
        {
            PuzzleId = puzzleId;
            SolveDuration = duration;
            DifficultyLevel = difficulty;
            ExtraUnitsUsed = extraUnits;
            IsPerfectSquare = isPerfect;
            Timestamp = DateTime.UtcNow;
        }
    }
}