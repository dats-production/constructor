using Constructor.DataStorage;
using Services;
using Services.LocalizationService;
using Services.SaveLoad;
using UI.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    public class LoadLayersButton : MonoBehaviour
    {
        [SerializeField] private ButtonView button;

        [Inject]
        public void Construct(IDataStorage dataStorage, IFileBrowser fileBrowser, 
            ISaveLoadService saveLoadService, ILocalizationService localizationService)
        {
            var model = new LoadLayersButtonModel(dataStorage, fileBrowser, saveLoadService, localizationService);
            model.Button.Subscribe(button.Bind).AddTo(this);
        }
    }
}