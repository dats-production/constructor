using Constructor;
using Constructor.DataStorage;
using Services.LocalizationService;
using UI.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    public class UpdateLayersButton : MonoBehaviour
    {
        [SerializeField] private ButtonView button;

        [Inject]
        public void Construct(IDataStorage dataStorage, 
            ILayersDataProvider layersDataProvider, ILocalizationService localizationService)
        {
            var model = new UpdateLayersButtonModel(dataStorage, layersDataProvider, localizationService);
            model.Button.Subscribe(button.Bind).AddTo(this);
        }
    }
}