using System.Threading;
using System.Threading.Tasks;
using Echo.Domain.Models;

namespace Echo.Domain.Repositories
{
    public interface IDiceRepository
    {
        Task<DicePreset[]> GetDicePresetsAsync(ulong userId, CancellationToken cancellationToken = default);
    }
}
