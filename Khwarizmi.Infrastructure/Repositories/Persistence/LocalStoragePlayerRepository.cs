using System.Text.Json;
using System.Text.Json.Serialization;
using Khwarizmi.Domain.Entities;
using Khwarizmi.Application.Interfaces;
using Microsoft.JSInterop;

namespace Khwarizmi.Infrastructure.Persistence
{
    public class LocalStoragePlayerRepository : IPlayerRepository
    {
        private readonly IJSRuntime _jsRuntime;
        private const string PlayerKey = "khwarizmi_player";
        
        private readonly JsonSerializerOptions _options = new()
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public LocalStoragePlayerRepository(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<Player> GetCurrentPlayerAsync()
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", PlayerKey);
            if (string.IsNullOrEmpty(json)) return null;

            try 
            {
                return JsonSerializer.Deserialize<Player>(json, _options);
            }
            catch
            {
                return null;
            }
        }

        public async Task SavePlayerAsync(Player player)
        {
            var json = JsonSerializer.Serialize(player, _options);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", PlayerKey, json);
        }
    }

    public class LocalStoragePuzzleRepository : IPuzzleRepository
    {
        private readonly IJSRuntime _jsRuntime;
        private const string ActivePuzzleKey = "khwarizmi_active_puzzle";
        
        private readonly JsonSerializerOptions _options = new()
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public LocalStoragePuzzleRepository(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<Puzzle> GetByIdAsync(Guid puzzleId)
        {
            var puzzle = await GetActivePuzzleInternalAsync();
            return puzzle?.Id == puzzleId ? puzzle : null;
        }

        public async Task<Puzzle> GetActivePuzzleByPlayerIdAsync(Guid playerId)
        {
            return await GetActivePuzzleInternalAsync();
        }

        public async Task SaveAsync(Puzzle puzzle)
        {
            var json = JsonSerializer.Serialize(puzzle, _options);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", ActivePuzzleKey, json);
        }

        public async Task DeleteAsync(Guid puzzleId)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", ActivePuzzleKey);
        }

        private async Task<Puzzle> GetActivePuzzleInternalAsync()
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", ActivePuzzleKey);
            if (string.IsNullOrEmpty(json)) return null;

            try
            {
                var puzzle = JsonSerializer.Deserialize<Puzzle>(json, _options);
                return puzzle?.CompletedAt == null ? puzzle : null;
            }
            catch
            {
                return null;
            }
        }
    }
}