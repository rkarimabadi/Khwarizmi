using Khwarizmi.Domain.Entities;
using Khwarizmi.Domain.Events;
using Khwarizmi.Application.Interfaces;
using Khwarizmi.Application.DTOs;

namespace Khwarizmi.Application.Services
{
    public class PlayerAppService
    {
        private readonly IPlayerRepository _playerRepository;
        private Player? _cachedPlayer;

        public List<string> Notifications { get; private set; } = new();

        public PlayerAppService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<PlayerProfileDto> GetProfileAsync()
        {
            var player = await GetOrInitPlayerAsync();
            return MapToDto(player);
        }

        public async Task UpdatePlayerProgressAsync(PuzzleSolvedEvent solvedEvent)
        {
            var player = await GetOrInitPlayerAsync();
            
            player.ApplyPuzzleCompletion(solvedEvent);
           
            HandlePlayerEvents(player);
            
            await _playerRepository.SavePlayerAsync(player);
        }

        private void HandlePlayerEvents(Player player)
        {
            foreach (var domainEvent in player.DomainEvents)
            {
                switch (domainEvent)
                {
                    case LevelUpEvent levelUp:
                        Notifications.Add($"تبریک {levelUp.Username}! شما به سطح {levelUp.NewLevel} رسیدید.");
                        break;
                        
                }
            }
            
            player.ClearEvents();
        }
        public async Task<PlayerStatsDto> GetPlayerStatsAsync()
        {
            var player = await GetOrInitPlayerAsync();
            var (count, avgTime) = player.GetStatistics();
            
            int perfectCount = player.CompletedPuzzlesLog.Count(p => p.IsPerfectSquare);

            return new PlayerStatsDto(count, avgTime, perfectCount);
        }

        private async Task<Player> GetOrInitPlayerAsync()
        {
            if (_cachedPlayer != null) return _cachedPlayer;

            _cachedPlayer = await _playerRepository.GetCurrentPlayerAsync();

            if (_cachedPlayer == null)
            {
                _cachedPlayer = new Player("Al-Khwarizmi Student");
                await _playerRepository.SavePlayerAsync(_cachedPlayer);
            }

            return _cachedPlayer;
        }

        private PlayerProfileDto MapToDto(Player p)
        {
            return new PlayerProfileDto(
                p.Id,
                p.Username,
                p.TotalScore,
                p.Level,
                p.ExperiencePoints,
                p.CompletedPuzzlesLog.Count
            );
        }
    }
}