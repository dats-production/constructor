using System.Collections.Generic;
using Constructor;
using Constructor.DataStorage;
using Constructor.Details;
using Services.LocalizationService;
using UI.Factories;
using UI.Models;
using UI.Navigation;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    public class GenerateCharactersButton : MonoBehaviour
    {
        [SerializeField] private ButtonView button;

        [Inject]
        public void Construct(IInputModalWindow inputModalWindow,
            IDataStorage dataStorage, IUIBlocker uiBlocker,  
            ValidatorFactory<Layer, List<Detail>> layerValidatorFactory,
            ValidatorFactory<string, string> stringValidatorFactory, 
            DiContainer diContainer, IUINavigator uiNavigator, ILocalizationService localizationService)
        {
            var model = new GenerateCharactersButtonModel(inputModalWindow, dataStorage, uiBlocker, 
                layerValidatorFactory, stringValidatorFactory, diContainer, uiNavigator, localizationService);
            model.Button.Subscribe(button.Bind).AddTo(this);
        }
    }
}