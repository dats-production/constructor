using System.Linq;
using Constructor.DataStorage;
using Services.LocalizationService;
using Zenject;

namespace Constructor.Validator.Validations
{
    public class CharacterNameValidation : IStringValidation
    {
        [Inject] private readonly ILocalizationService localizationService;
        private string failedValidationName;
        private IDataStorage dataStorage;

        [Inject]
        public void Construct(IDataStorage dataStorage) =>
            this.dataStorage = dataStorage;
        
        public bool Validate(string enteredName, out string ouput)
        {
            ouput = default;
            if (dataStorage.Characters.All(character => character.Name.Value != enteredName)) return true;
            failedValidationName = enteredName;
            return false;
        }

        public string GetDescription()
        {
            localizationService.SetStringVariable("failedValidationName", failedValidationName);
            return localizationService.Localize("There is a character with the same name (). Enter another name.");
        }
    }
}