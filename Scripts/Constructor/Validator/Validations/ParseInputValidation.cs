using Services.LocalizationService;
using Zenject;

namespace Constructor.Validator.Validations
{
    public class ParseIntValidation : IStringValidation
    {
        [Inject] private readonly ILocalizationService localizationService;

        public bool Validate(string inputText, out string textOut)
        {
            textOut = default;
            return (long.TryParse(inputText, out var inputCount));
        }

        public string GetDescription()
        {
            return localizationService.Localize("Enter only integer numbers");
        }
    }
}