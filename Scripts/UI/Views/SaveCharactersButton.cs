using Constructor.DataStorage;
using Services;
using Services.SaveLoad;
using UI.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    public class SaveCharactersButton : MonoBehaviour
    {
        [SerializeField] private ButtonView button;

        [Inject]
        public void Construct(IDataStorage dataStorage, IFileBrowser fileBrowser, ISaveLoadService saveLoadService)
        {
            var model = new SaveCharactersButtonModel(dataStorage, fileBrowser, saveLoadService);
            model.Button.Subscribe(button.Bind).AddTo(this);
        }
    }
}