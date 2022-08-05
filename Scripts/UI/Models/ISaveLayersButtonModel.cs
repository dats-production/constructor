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
    public interface ISaveLayersButtonModel
    {
        IObservable<IButtonModel> Button { get; }
    }

    public class SaveLayersButtonModel : ISaveLayersButtonModel
    {
        private readonly IDataStorage dataStorage;
        private readonly IFileBrowser fileBrowser;
        private readonly ISaveLoadService saveLoadService;
        private ILocalizationService localizationService;

        public SaveLayersButtonModel(IDataStorage dataStorage,
            IFileBrowser fileBrowser,
            ISaveLoadService saveLoadService, ILocalizationService localizationService)
        {
            this.localizationService = localizationService;
            this.dataStorage = dataStorage;
            this.fileBrowser = fileBrowser;
            this.saveLoadService = saveLoadService;
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
                    var layers = dataStorage.Layers.Select(x => x.AsLayerData()).ToList();
                    var path = fileBrowser.SaveFilePanel("Save file", null, null, null);
                    var data = new LayersData(layers);
                    saveLoadService.SaveLayers(data, path.Name);
                }).AddTo(disposable);
                observer.OnNext(model);
                return disposable;
            });
    }
}