using System;

namespace UI.Views
{
    [Serializable]
    public struct TopMenuButtonData
    {
        public bool IsActive;
    }

    [Serializable]
    public class TopMenuButtonViewStateData : ViewStateData<TopMenuButtonTarget, TopMenuButtonData>
    {
        public override void Apply(TopMenuButtonTarget target)
        {
            target.NormalButton.SetActive(!Value.IsActive);
            target.PressedButton.SetActive(Value.IsActive);
        }
    }

    [Serializable]
    public class
        TopMenuButtonViewStatesHolder : ViewStatesHolder<TopMenuButtonTarget, TopMenuButtonData, TopMenuButtonViewStateData>
    {
    }

    public class TopMenuButtonViewState : ViewState<TopMenuButtonViewStatesHolder>
    {
    }
}