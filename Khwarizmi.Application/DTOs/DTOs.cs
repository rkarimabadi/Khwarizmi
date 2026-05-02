namespace Khwarizmi.Application.DTOs
{
    public record TileDto(
        Guid Id,
        string Type,
        int Width,
        int Height,
        int? X,
        int? Y,
        string WidthVarType,
        string HeightVarType
    );

    public record PuzzleDto(
        Guid Id,
        string TargetEquation,
        List<TileDto> Inventory,
        List<TileDto> Board,
        int BoardColumns,
        int BoardRows,
        bool IsCompleted
    );

    public record PlayerProfileDto(
        Guid Id,
        string Username,
        int TotalScore,
        int Level,
        int ExperiencePoints,
        int SolvedPuzzlesCount
    );

    public record PlayerStatsDto(
        int SolvedCount,
        double AverageSolveTimeSeconds,
        int PerfectSquaresCount
    );
}