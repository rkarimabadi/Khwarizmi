using Khwarizmi.Domain.ValueObjects;
using Khwarizmi.Domain.Events;

namespace Khwarizmi.Domain.Entities
{
    public class Player
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public int TotalScore { get; private set; }
        public int Level { get; private set; }
        public int ExperiencePoints { get; private set; }
        
        private readonly List<PuzzleRecord> _completedPuzzlesLog = new();
        public IReadOnlyCollection<PuzzleRecord> CompletedPuzzlesLog => _completedPuzzlesLog.AsReadOnly();

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public Player(string username)
        {
            Id = Guid.NewGuid();
            Username = username;
            Level = 1;
        }

        public void ApplyPuzzleCompletion(PuzzleSolvedEvent solvedEvent)
        {
            var record = new PuzzleRecord(
                solvedEvent.PuzzleId, 
                solvedEvent.Duration, 
                solvedEvent.Difficulty, 
                solvedEvent.ExtraUnitsUsed, 
                solvedEvent.IsPerfectSquare);

            _completedPuzzlesLog.Add(record);
            
            int earnedPoints = CalculatePoints(record);
            TotalScore += earnedPoints;
            AddExperience(earnedPoints);
        }

        private int CalculatePoints(PuzzleRecord record)
        {
            int basePoints = record.DifficultyLevel * 50;
            int timePenalty = (int)(record.SolveDuration.TotalSeconds / 10);
            int efficiencyPenalty = record.ExtraUnitsUsed * 5;
            int perfectionBonus = record.IsPerfectSquare ? 25 : 0;

            return Math.Max(10, basePoints - timePenalty - efficiencyPenalty + perfectionBonus);
        }

        private void AddExperience(int points)
        {
            ExperiencePoints += points;
            int nextLevelThreshold = Level * 500; // مثال ساده برای حد آستانه

            if (ExperiencePoints >= nextLevelThreshold)
            {
                Level++;
                _domainEvents.Add(new LevelUpEvent(Id, Level, Username));
            }
        }

        public void ClearEvents() => _domainEvents.Clear();

        public (int count, double avgTime) GetStatistics()
        {
            throw new NotImplementedException();
        }
    }
}