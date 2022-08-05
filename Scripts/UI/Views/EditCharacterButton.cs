using Services.LocalizationService;
using UI.Models;
using UI.Navigation;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    public class EditCharacterButton : MonoBehaviour
    {
        [SerializeField] private ButtonView button;

        [Inject]
        public void Construct(IUINavigator uiNavigator, ICharacterInfoModel characterInfoModel,
            ILocalizationService localizationService)
        {
            var model = new EditCharacterButtonModel(uiNavigator, characterInfoModel, localizationService);
            model.Button.Subscribe(button.Bind).AddTo(this);
        }
    }
}