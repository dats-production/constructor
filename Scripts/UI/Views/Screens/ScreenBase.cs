using UI.Models.Screens;
using UnityEngine;

namespace UI.Views.Screens
{
    public abstract class ScreenBase<T> : MonoBehaviour, IScreenModel<T> where T : IScreenArgs
    {
        [SerializeField] private ScreenAnimator screenAnimator;

        public abstract ScreenType Type { get; }

        public virtual void Open(T args = default)
        {
            screenAnimator.Open();
        }

        public virtual void Close()
        {
            screenAnimator.Close();
        }
    }
}