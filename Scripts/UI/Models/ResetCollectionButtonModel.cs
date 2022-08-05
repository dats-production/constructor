using System;
using Constructor.DataStorage;
using UI.Navigation;
using UniRx;

namespace UI.Models
{
    public interface IResetCollectionButtonModel
    {
        IObservable<IButtonModel> Button { get; }
    }

    public class ResetCollectionButtonModel : IResetCollectionButtonModel
    {
        private readonly IDataStorage dataStorage;
        private ICharactersCollectionInfoModel charactersCollectionInfoModel;
        private IUINavigator uiNavigator;

        public ResetCollectionButtonModel(IDataStorage dataStorage,
            ICharactersCollectionInfoModel charactersCollectionInfoModel,
            IUINavigator uiNavigator)
        {
            this.dataStorage = dataStorage;
            this.charactersCollectionInfoModel = charactersCollectionInfoModel;
            this.uiNavigator = uiNavigator;
        }

        public IObservable<IButtonModel> Button =>
            Observable.Create<IButtonModel>(observer =>
            {
                var disposable = new CompositeDisposable();
                var model = new ButtonModel();
                model.AddTo(disposable);
                model.Click.Subscribe(_ =>
                {
                    dataStorage.RemoveCharacters();
                    charactersCollectionInfoModel.Close();
                    uiNavigator.OpenGenerationSettingsScreen();
                }).AddTo(disposable);
                observer.OnNext(model);
                return disposable;
            });
    }
}