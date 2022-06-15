using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Echo.Domain.Models;
using Echo.Domain.Repositories;

namespace Echo.AutoCompleters
{
    public class DiceAutoCompleter : AutocompleteHandler
    {
        private readonly IDiceRepository _diceRepository;

        public DiceAutoCompleter(IDiceRepository diceRepository)
        {
            _diceRepository = diceRepository;
        }

        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            DicePreset[] dicePresets = await _diceRepository.GetDicePresetsAsync(context.User.Id);

            return AutocompletionResult.FromSuccess(dicePresets
                .Select(p => new AutocompleteResult(p.Name, p.Name))
                .Take(25)
                .ToArray());
        }
    }
}
