using Services;
using Services.ExportCollection;
using Services.LocalizationService;
using UI.Factories;
using UI.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    public class ExportCollectionButton : MonoBehaviour
    {
        [SerializeField] private ButtonView button;

        [Inject]
        public void Construct(IFileBrowser fileBrowser, IExportCollectionService exportCollectionService, 
            IInputModalWindow inputModalWindow, ValidatorFactory<string, string> validatorFactory, 
            DiContainer diContainer, ILocalizationService localizationService)
        {
            var model = new ExportCollectionButtonModel(fileBrowser, exportCollectionService, inputModalWindow, 
                validatorFactory, diContainer, localizationService);
            model.Button.Subscribe(button.Bind).AddTo(this);
        }
    }
}
