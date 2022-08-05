using Constructor.DataStorage;
using UI.Models;
using UI.Navigation;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    public class ResetCollectionButton : MonoBehaviour
    {
        [SerializeField] private ButtonView button;

        [Inject]
        public void Construct(IDataStorage dataStorage,
            ICharactersCollectionInfoModel charactersCollectionInfoModel,
            IUINavigator uiNavigator)
        {
            var model = new ResetCollectionButtonModel(dataStorage, charactersCollectionInfoModel, uiNavigator);
            model.Button.Subscribe(button.Bind).AddTo(this);
        }
    }
}