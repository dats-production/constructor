using UI.Models;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class ButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;

        public void Bind(IButtonModel model)
        {
            button.OnClickAsObservable().Subscribe(_ => model.Click.Execute(Unit.Default)).AddTo(this);
            model.Interactable.Subscribe(x => button.interactable = x).AddTo(this);
        }
    }
}