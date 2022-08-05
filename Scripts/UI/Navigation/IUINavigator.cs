using UI.Views.Screens;

namespace UI.Navigation
{
    public interface IUINavigator
    {
        void OpenGenerationSettingsScreen();
        void OpenCollectionPreviewScreen();
        void OpenEditCharacterScreen(EditCharacterScreenArgs args);
    }
}