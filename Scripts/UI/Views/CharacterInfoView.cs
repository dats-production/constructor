using System.Collections.Generic;
using Constructor;
using Constructor.Validator;
using Constructor.Validator.Validations;
using Importer;
using Michsky.UI.ModernUIPack;
using Services.LocalizationService;
using TMPro;
using UI.Factories;
using UI.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    public class CharacterInfoView : MonoBehaviour, ICharacterInfoModel
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private TextMeshProUGUI charactersCountText;
        [SerializeField] private TextMeshProUGUI charactersCountTitleText;
        [SerializeField] private CharacterInfoSheetView characterInfoSheetView;
        [SerializeField] private TooltipContent warningTooltipContent;

        private ReactiveProperty<ICharacter> currentSelection = new();
        private ModelPreview modelPreview;
        private CharacterViewer characterViewer;
        private ICharacter character;
        private Validator<string, string> validator;
        private ILocalizationService localizationService;

        public IReadOnlyReactiveProperty<ICharacter> CurrentSelection => currentSelection;
        
        private void Awake()
        {
            currentSelection.AddTo(this);
        }

        [Inject]
        public void Construct(ModelPreview modelPreview, CharacterViewer characterViewer, 
            ValidatorFactory<string, string> validatorFactory, DiContainer diContainer, 
            ILocalizationService localizationService)
        {
            validator = validatorFactory.Create(new List<IValidation<string, string>>()
            {
                diContainer.Instantiate<CharacterNameValidation>()
            });
            
            this.characterViewer = characterViewer;
            this.modelPreview = modelPreview;
            this.localizationService = localizationService;

            nameInputField.onEndEdit
                .AsObservable()
                .Where(x => x != null)
                .Subscribe(SetName)
                .AddTo(nameInputField);
            nameInputField.onSelect
                .AsObservable()
                .Subscribe((x) =>
                {
                    if (character != null) nameInputField.text = character.Name.Value;
                })
                .AddTo(nameInputField);
        }
        
        public void Show(ICharacter character)
        {
            if (!characterInfoSheetView.gameObject.activeSelf)
            {
                characterInfoSheetView.gameObject.SetActive(true);
                nameInputField.transform.parent.gameObject.SetActive(true);
            }
            this.character = character;
            character.Name.Subscribe((x) => nameText.text = x).AddTo(this);
            characterInfoSheetView.SetSheet(character);
            modelPreview.SetModelTransform(characterViewer.AssembleCharacter(character));
            currentSelection.Value = character;
        }

        public void ShowCharactersCount(int count)
        {
            charactersCountText.fontSize = charactersCountTitleText.fontSize;
            charactersCountText.SetText(count.ToString());
        }

        public void Clear()
        {
            nameText.SetText(string.Empty);
            currentSelection.Value = null;
            characterInfoSheetView.ClearSheet();
            characterInfoSheetView.gameObject.SetActive(false);
            nameInputField.transform.parent.gameObject.SetActive(false);
        }
        
        private void SetName(string enteredName)
        {
            var isValidationPassed = validator.Validate(enteredName, out var validationFailDescriptions);
            if (isValidationPassed)
                character.Name.Value = enteredName;
            
            warningTooltipContent.gameObject.SetActive(!isValidationPassed);
            var warningString = localizationService.Localize("WARNING");
            warningTooltipContent.description = $"{warningString}: {string.Join($"\n{warningString}: ", validationFailDescriptions)}";
        }
    }
}