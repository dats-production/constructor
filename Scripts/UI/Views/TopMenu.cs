using UI.Models;
using UI.Models.Screens;
using UI.Navigation;
using UnityEngine;
using UniRx;
using Zenject;

namespace UI.Views
{
    public interface ITopMenu
    {
        void ToggleTopMenuButtons(ScreenType screenType);
    }

    public class TopMenu : MonoBehaviour, IInitializable, ITopMenu
    {
        [SerializeField] private ButtonView generationSettingsButton;
        [SerializeField] private ButtonView collectionPreviewButton;
        [SerializeField] private ButtonView quitButton;
        [SerializeField] private ViewStateSwitcher topMenuButtonStateSwitcher;

        private ButtonModel generationSettingsButtonModel;
        private ButtonModel collectionPreviewButtonModel;
        private ButtonModel quitButtonModel;

        private IUINavigator uiNavigator;

        [Inject]
        public void Construct(IUINavigator uiNavigator)
        {
            this.uiNavigator = uiNavigator;
        }

        public void Initialize()
        {
            BindGenerationSettingsButton();
            BindCollectionPreviewButton();
            BindExitButton();
        }

        private void BindGenerationSettingsButton()
        {
            generationSettingsButtonModel = new ButtonModel();
            generationSettingsButtonModel.AddTo(this);
            generationSettingsButtonModel.Click.Subscribe(_ => uiNavigator.OpenGenerationSettingsScreen()).AddTo(this);
            generationSettingsButtonModel.Interactable.Value = false;
            generationSettingsButton.Bind(generationSettingsButtonModel);
        }

        private void BindCollectionPreviewButton()
        {
            collectionPreviewButtonModel = new ButtonModel();
            collectionPreviewButtonModel.AddTo(this);
            collectionPreviewButtonModel.Click.Subscribe(_ => uiNavigator.OpenCollectionPreviewScreen()).AddTo(this);
            collectionPreviewButtonModel.Interactable.Value = false;
            collectionPreviewButton.Bind(collectionPreviewButtonModel);
        }
        
        private void BindExitButton()
        {
            quitButtonModel = new ButtonModel();
            quitButtonModel.AddTo(this);
            quitButtonModel.Click.Subscribe(_ =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                Application.Quit();
            }).AddTo(this);
            quitButton.Bind(quitButtonModel);
        }

        public void ToggleTopMenuButtons(ScreenType screenType) =>
            topMenuButtonStateSwitcher.SwitchState(screenType);
    }
}