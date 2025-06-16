using BrawlBuddy.Api.Models;

namespace BrawlBuddy.Api.Services
{
    public interface IBrawlApiService
    {
        Task<Player?> GetPlayerAsync(string playerTag);
        Task<BattleLog?> GetPlayerBattleLogAsync(string playerTag);
        Task<List<Brawler>?> GetBrawlersAsync();
    }
}
