using System;
using System.Linq;
using Constructor;
using Constructor.DataStorage;
using Cysharp.Threading.Tasks;
using ModestTree;
using Services.LocalizationService;
using UniRx;
using UnityEngine;

namespace UI.Models
{
    public interface IUpdateLayersButtonModel
    {
        IObservable<IButtonModel> Button { get; }
    }

    public class UpdateLayersButtonModel : IUpdateLayersButtonModel
    {
        private readonly IDataStorage dataStorage;
        private ILayersDataProvider layersDataProvider;
        private readonly ILocalizationService localizationService;

        public UpdateLayersButtonModel(IDataStorage dataStorage,
            ILayersDataProvider layersDataProvider, ILocalizationService localizationService)
        {
            this.dataStorage = dataStorage;
            this.layersDataProvider = layersDataProvider;
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
                    if (!dataStorage.Layers.IsEmpty())
                        layersDataProvider.ExportSheets(dataStorage.Layers.Select(x => x).ToList()).Forget();
                    else
                        Debug.LogError(localizationService.Localize("You need to import Layers first."));
                }).AddTo(disposable);
                observer.OnNext(model);
                return disposable;
            });
    }
}