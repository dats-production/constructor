using Importer;
using UI.Models.Screens;
using UI.Views;
using UI.Views.Screens;
using UnityEngine;
using Zenject;

namespace UI.Navigation
{
    public class UINavigator : MonoBehaviour, IUINavigator
    {
        [SerializeField] private GenerationSettingsScreen generationSettingsScreen;
        [SerializeField] private CollectionPreviewScreen collectionPreviewScreen;
        [SerializeField] private EditCharacterScreen editCharacterScreen;

        private IScreenModel currentScreen;
        private CharacterViewer characterViewer;
        private DetailInfoView detailInfoView;
        private ITopMenu topMenu;

        [Inject]
        private void Construct(CharacterViewer characterViewer,
            DetailInfoView detailInfoView, ITopMenu topMenu)
        {
            this.characterViewer = characterViewer;
            this.detailInfoView = detailInfoView;
            this.topMenu = topMenu;
        }

        private void Start()
        {
            OpenGenerationSettingsScreen();
        }

        public void OpenGenerationSettingsScreen()
        {
            SwitchScreen(generationSettingsScreen);
            generationSettingsScreen.Open();
            topMenu.ToggleTopMenuButtons(ScreenType.GenerationSettings);
        }

        public void OpenCollectionPreviewScreen()
        {
            SwitchScreen(collectionPreviewScreen);
            collectionPreviewScreen.Open();
            topMenu.ToggleTopMenuButtons(ScreenType.CollectionPreview);
        }

        public void OpenEditCharacterScreen(EditCharacterScreenArgs args)
        {
            SwitchScreen(editCharacterScreen);
            editCharacterScreen.Open(args);
        }

        private void SwitchScreen(IScreenModel newScreen)
        {
            if (currentScreen == newScreen)
                return;

            characterViewer.ClearContainer();
            detailInfoView.Hide();
            currentScreen?.Close();
            currentScreen = newScreen;
        }
    }
}