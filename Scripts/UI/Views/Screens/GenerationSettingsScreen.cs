using UI.Models.Screens;
using UI.Navigation;
using UnityEngine;

namespace UI.Views.Screens
{
    public class GenerationSettingsScreen : ScreenBase<IScreenArgs>
    {
        [SerializeField] private ViewStateSwitcher modelPreviewStateSwitcher;

        public override ScreenType Type => ScreenType.GenerationSettings;

        public override void Open(IScreenArgs args = default)
        {
            base.Open(args);
            modelPreviewStateSwitcher.SwitchState(ModelPreviewVisualState.Default);
        }
    }
}