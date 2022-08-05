using Constructor.DataStorage;
using UI.Models;
using UI.Models.Screens;
using UI.Navigation;
using UnityEngine;
using Zenject;

namespace UI.Views.Screens
{
    public class CollectionPreviewScreen : ScreenBase<IScreenArgs>
    {
        [SerializeField] private ViewStateSwitcher modelPreviewStateSwitcher;

        public override ScreenType Type => ScreenType.CollectionPreview;

        private IDataStorage dataStorage;
        private ICharacterInfoModel characterInfoView;
        private ICharactersCollectionInfoModel charactersCollectionInfoModel;

        [Inject]
        public void Construct(IDataStorage dataStorage, ICharacterInfoModel characterInfoView,
            ICharactersCollectionInfoModel charactersCollectionInfoModel)
        {
            this.dataStorage = dataStorage;
            this.characterInfoView = characterInfoView;
            this.charactersCollectionInfoModel = charactersCollectionInfoModel;
        }

        public override void Open(IScreenArgs args = default)
        {
            base.Open(args);
            charactersCollectionInfoModel.Open(dataStorage.Characters);
            characterInfoView.ShowCharactersCount(dataStorage.Characters.Count);
            modelPreviewStateSwitcher.SwitchState(ModelPreviewVisualState.Default);
        }

        public override void Close()
        {
            base.Close();
            charactersCollectionInfoModel.Close();
        }
    }
}