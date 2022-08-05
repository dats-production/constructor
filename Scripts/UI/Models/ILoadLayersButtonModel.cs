using System;
using System.Linq;
using Constructor.DataStorage;
using ModestTree;
using Services;
using Services.LocalizationService;
using Services.SaveLoad;
using Services.SaveLoad.Data;
using UniRx;
using UnityEngine;

namespace UI.Models
{
    public interface ILoadLayersButtonModel
    {
        IObservable<IButtonModel> Button { get; }
    }

    public class LoadLayersButtonModel : ILoadLayersButtonModel
    {
        private readonly IDataStorage dataStorage;
        private readonly IFileBrowser fileBrowser;
        private readonly ISaveLoadService saveLoadService;
        private readonly ILocalizationService localizationService;

        public LoadLayersButtonModel(IDataStorage dataStorage, IFileBrowser fileBrowser,
            ISaveLoadService saveLoadService, ILocalizationService localizationService)
        {
            this.dataStorage = dataStorage;
            this.fileBrowser = fileBrowser;
            this.saveLoadService = saveLoadService;
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
                    if(dataStorage.Layers.IsEmpty())
                    {
                        Debug.LogError(localizationService.Localize("You need to import Layers first."));
                        return;
                    }
                    var path = fileBrowser.OpenFilePanel("Open file", null, null, false);
                    var data = saveLoadService.LoadLayers(path.First().Name);
                    data.ChangeLayersData(dataStorage);
                }).AddTo(disposable);
                observer.OnNext(model);
                return disposable;
            });
    }
}