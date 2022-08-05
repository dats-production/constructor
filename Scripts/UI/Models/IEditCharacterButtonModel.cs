using System;
using Services.LocalizationService;
using UI.Navigation;
using UI.Views.Screens;
using UniRx;
using UnityEngine;

namespace UI.Models
{
    public interface IEditCharacterButtonModel
    {
        IObservable<IButtonModel> Button { get; }
    }

    public class EditCharacterButtonModel : IEditCharacterButtonModel
    {
        private readonly IUINavigator uiNavigator;
        private readonly ICharacterInfoModel characterInfoModel;
        private readonly ILocalizationService localizationService;

        public EditCharacterButtonModel(IUINavigator uiNavigator, ICharacterInfoModel characterInfoModel,
            ILocalizationService localizationService)
        {
            this.uiNavigator = uiNavigator;
            this.characterInfoModel = characterInfoModel;
            this.localizationService = localizationService;
        }

        public IObservable<IButtonModel> Button =>
            Observable.Create<IButtonModel>(observer =>
            {
                var disposable = new CompositeDisposable();
                var model = new ButtonModel();
                model.AddTo(disposable);
                model.Click.Subscribe(_ =>
                    {
                        if (characterInfoModel.CurrentSelection.Value != null)
                            uiNavigator.OpenEditCharacterScreen(
                                new EditCharacterScreenArgs(characterInfoModel.CurrentSelection.Value));
                        else
                            Debug.LogError(localizationService.Localize("You need to select a Character first."));
                    })
                    .AddTo(disposable);
                observer.OnNext(model);
                return disposable;
            });
    }
}