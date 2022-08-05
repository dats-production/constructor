using System.Collections.Generic;
using Constructor.Validator;
using Constructor.Validator.Validations;
using Cysharp.Threading.Tasks;
using Michsky.UI.ModernUIPack;
using Services.LocalizationService;
using UI.Factories;
using UI.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    [RequireComponent(typeof(ModalWindowManager))]
    public class InputModalWindow : MonoBehaviour, IInputModalWindow
    {
        [SerializeField] private CustomInputField uiManagerInputField;
        [SerializeField] private TooltipContent warningTooltipContent;
    
        private ModalWindowManager modalWindowManager;
        private Validator<string, string> validator;
        private ILocalizationService localizationService;

        [Inject]
        public void Constructor(ValidatorFactory<string, string> validatorFactory, DiContainer diContainer, 
            ILocalizationService localizationService)
        {
            this.localizationService = localizationService;
            validator = validatorFactory.Create(new List<IValidation<string, string>>()
            {
                diContainer.Instantiate<ParseIntValidation>()
            });
        }
        
        private void Awake()
        {
            modalWindowManager = GetComponent<ModalWindowManager>();
        }

        public async UniTask<string> Show(string windowTitle, string enteredText = "")
        {
            var inputDone = false;
            uiManagerInputField.inputText.text = enteredText;
            modalWindowManager.titleText = windowTitle;
            modalWindowManager.OpenWindow();
            modalWindowManager.confirmButton.onClick
                .AsObservable()
                .Subscribe(_ =>
                {
                    if (string.IsNullOrEmpty(uiManagerInputField.inputText.text))
                    {
                        var emptyInputDescription = new List<string>() {"Enter integer number first"};
                        ToggleWarningTooltip(emptyInputDescription);
                        return;
                    }
                    enteredText = uiManagerInputField.inputText.text;
                    if (validator.Validate(enteredText, out var validationFailDescriptions))
                    {
                        inputDone = true;
                    }
                    else
                    {
                        ToggleWarningTooltip(validationFailDescriptions);
                    }
                }).AddTo(this);
        
            modalWindowManager.cancelButton.onClick
                .AsObservable()
                .Subscribe(_ =>
                {
                    Hide();
                }).AddTo(this);
        
            await UniTask.WaitUntil(() => inputDone);
            return enteredText;
        }

        public void Hide()
        {
            warningTooltipContent.gameObject.SetActive(false);
            modalWindowManager.CloseWindow();
        }

        public void ToggleWarningTooltip(List<string> validationFailDescriptions)
        {
            warningTooltipContent.gameObject.SetActive(true);
            var warningString = localizationService.Localize("WARNING");
            warningTooltipContent.description = $"{warningString}: {string.Join($"\n{warningString}: ", validationFailDescriptions)}";
        }
    }
}
