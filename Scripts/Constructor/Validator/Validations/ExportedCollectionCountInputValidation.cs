using Services.LocalizationService;
using Zenject;

namespace Constructor.Validator.Validations
{
    public class ExportedCollectionCountInputValidation : CountInputValidation
    {
        [Inject] private readonly ILocalizationService localizationService;
        
        public override bool Validate(string inputText, out string textOut)
        {
            textOut = default;
            // before this validation, there was ParseValidation, that's why it returns true            
            if (!int.TryParse(inputText, out inputCount)) return true;

            maxGenerations = dataStorage.Characters.Count;
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