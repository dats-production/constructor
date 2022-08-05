using System;
using UniRx;

namespace UI.Models
{
    public interface IUIBlocker
    {
        void Show(Subject<float> progressSubject, Action onCancel = null);
        void Hide();
    }
}