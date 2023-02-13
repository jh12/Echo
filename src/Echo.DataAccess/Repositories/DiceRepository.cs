using System.Threading;
using System.Threading.Tasks;
using Echo.Domain.Models;
using Echo.Domain.Repositories;

namespace Echo.DataAccess.Repositories
{
    public class DiceRepository : IDiceRepository
    {
        public Task<DicePreset[]> GetDicePresetsAsync(ulong userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new[]
            {
                new DicePreset("6 sides", "The dice landed on: `{0}`", new[] { "1", "2", "3", "4", "5", "6" }),
                new DicePreset("Games", "You are gonna play: `{0}`", new[] { "Overwatch", "Back 4 Blood", "Sea Of Thieves" }),
                new DicePreset("Overwatch Roles", "You are gonna play: `{0}`", new[] { "Tank", "Damage", "Support" })
            });
        }
    }
}
