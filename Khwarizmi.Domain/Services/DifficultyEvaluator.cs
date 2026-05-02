using Khwarizmi.Domain.ValueObjects;

namespace Khwarizmi.Domain.Services
{
    public class DifficultyEvaluator
    {
        private const double FastSolveThresholdSeconds = 45.0;
        private const double SlowSolveThresholdSeconds = 120.0;
        private const int MaxDifficultyLevel = 4;

        public int SuggestNextDifficulty(IReadOnlyCollection<PuzzleRecord> history, int currentLevel)
        {
            if (history == null || !history.Any())
                return 1;

            var recentRecords = history.OrderByDescending(r => r.Timestamp).Take(3).ToList();

            if (recentRecords.Count < 3)
                return currentLevel;

            double avgDuration = recentRecords.Average(r => r.SolveDuration.TotalSeconds);
            int perfectSolves = recentRecords.Count(r => r.IsPerfectSquare);
            int totalExtraUnits = recentRecords.Sum(r => r.ExtraUnitsUsed);

            if (avgDuration < FastSolveThresholdSeconds && perfectSolves >= 2 && totalExtraUnits < 5)
            {
                return Math.Min(currentLevel + 1, MaxDifficultyLevel);
            }

            if (avgDuration > SlowSolveThresholdSeconds || (totalExtraUnits > 20 && perfectSolves == 0))
            {
                return Math.Max(currentLevel - 1, 1);
            }

            return currentLevel;
        }
    }
}