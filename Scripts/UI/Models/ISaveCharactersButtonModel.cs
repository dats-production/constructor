using System;
using System.Linq;
using Constructor.DataStorage;
using Services;
using Services.SaveLoad;
using Services.SaveLoad.Data;
using UniRx;

namespace UI.Models
{
    public interface ISaveCharactersButtonModel
    {
        IObservable<IButtonModel> Button { get; }
    }

    public class SaveCharactersButtonModel : ISaveCharactersButtonModel
    {
        private readonly IDataStorage dataStorage;
        private readonly IFileBrowser fileBrowser;
        private readonly ISaveLoadService saveLoadService;

        public SaveCharactersButtonModel(IDataStorage dataStorage, IFileBrowser fileBrowser,
            ISaveLoadService saveLoadService)
        {
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
                    var characters = dataStorage.Characters.Select(x => x.AsCharacterData()).ToList();
                    var path = fileBrowser.SaveFilePanel("Save file", null, null, null);
                    var data = new CharactersData(characters);
                    saveLoadService.SaveCharacters(data, path.Name);
                }).AddTo(disposable);
                observer.OnNext(model);
                return disposable;
            });
    }
}