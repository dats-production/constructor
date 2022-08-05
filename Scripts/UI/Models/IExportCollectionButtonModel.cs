using System;
using System.Collections.Generic;
using Constructor.Validator;
using Constructor.Validator.Validations;
using Cysharp.Threading.Tasks;
using Services;
using Services.ExportCollection;
using Services.LocalizationService;
using UI.Factories;
using UniRx;
using Zenject;

namespace UI.Models
{
    public interface IExportCollectionButtonModel
    {
        IObservable<IButtonModel> Button { get; }
    }

    public class ExportCollectionButtonModel : IExportCollectionButtonModel
    {
        private readonly IFileBrowser fileBrowser;
        private readonly IExportCollectionService exportCollectionService;
        private readonly IInputModalWindow inputModalWindow;
        private readonly ILocalizationService localizationService;
        private Validator<string, string> validator;

        public ExportCollectionButtonModel(IFileBrowser fileBrowser, IExportCollectionService exportCollectionService, 
            IInputModalWindow inputModalWindow, ValidatorFactory<string, string> validatorFactory, 
            DiContainer diContainer, ILocalizationService localizationService)
        {
            this.fileBrowser = fileBrowser;
            this.exportCollectionService = exportCollectionService;
            this.inputModalWindow = inputModalWindow;
            this.localizationService = localizationService;

            validator = validatorFactory.Create(new List<IValidation<string, string>>()
            {
                diContainer.Instantiate<ExportedCollectionCountInputValidation>()
            });
        }

        public IObservable<IButtonModel> Button =>
            Observable.Create<IButtonModel>(observer =>
            {
                var disposable = new CompositeDisposable();
                var model = new ButtonModel();
                model.AddTo(disposable);
                model.Click.Subscribe(ExportImages).AddTo(disposable);
                observer.OnNext(model);
                return disposable;
            });

        private async void ExportImages(Unit _)
        {
            var enteredCountText = await inputModalWindow
                .Show(localizationService.Localize("Enter images count to export"));
            if (validator.Validate(enteredCountText, out var validationFailDescriptions))
            {
                inputModalWindow.Hide();
                if (!int.TryParse(enteredCountText, out var count)) return;
                
                var path = fileBrowser.OpenFolderPanel("Export folder", null, false)[0];
                if (string.IsNullOrEmpty(path.Name)) return;
                
                exportCollectionService.ExportCollection(path.Name + "/", count).Forget();                  
            }
            else
            {
                inputModalWindow.ToggleWarningTooltip(validationFailDescriptions);
                ExportImages(_);
            }
        }
    }
}

