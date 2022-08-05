using Services.LocalizationService;
using Zenject;

namespace Constructor.Validator.Validations
{
    public class GeneratedCharactersCountInputValidation : CountInputValidation
    {
        [Inject] private readonly ILocalizationService localizationService;
        
        public override bool Validate(string inputText, out string textOut)
        {
            textOut = default;
            // before this validation, there was ParseValidation, that's why it returns true            
            if (!int.TryParse(inputText, out inputCount)) return true;

            var layers = dataStorage.Layers;
            maxGenerations = 1;
            foreach (var layer in layers)
                maxGenerations *= layer.Details.Count;
            return (inputCount <= maxGenerations && inputCount >= minGenerations);
        }

        public override string GetDescription()
        {
            localizationService.SetStringVariable("minGenerations", minGenerations.ToString());
            localizationService.SetStringVariable("maxGenerations", maxGenerations.ToString());
            return localizationService.Localize("Enter integer number between () and ().");
        }
    }
}