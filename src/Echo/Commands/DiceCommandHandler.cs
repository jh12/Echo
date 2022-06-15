using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Interactions;
using Echo.AutoCompleters;
using Echo.Domain.Models;
using Echo.Domain.Repositories;

namespace Echo.Commands
{
    [Discord.Interactions.Group("dice", "Throw a dice and get a random result")]
    public class DiceCommandHandler : InteractionModuleBase
    {
        private readonly IDiceRepository _diceRepository;

        private static readonly Random Rng = new();

        public DiceCommandHandler(IDiceRepository diceRepository)
        {
            _diceRepository = diceRepository;
        }

        [SlashCommand("throw", "Throw a dice")]
        public async Task ThrowCommand([Autocomplete(typeof(DiceAutoCompleter))] string preset)
        {
            DicePreset[] dicePresets = await _diceRepository.GetDicePresetsAsync(Context.User.Id);

            DicePreset? dicePreset = dicePresets.SingleOrDefault(p => string.Equals(p.Name, preset, StringComparison.CurrentCultureIgnoreCase));

            if (dicePreset == null)
            {
                await RespondAsync("Preset not found");
                return;
            }

            int presetIndex = Rng.Next(dicePreset.Values.Length);
            string result = dicePreset.Values[presetIndex];

            await RespondAsync(string.Format(dicePreset.SelectionText, result));
        }
    }
}
