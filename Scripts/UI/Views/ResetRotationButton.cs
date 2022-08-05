using UI.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    public class ResetRotationButton : MonoBehaviour
    {
        [SerializeField] private ButtonView button;

        [Inject]
        public void Construct(ModelPreview fbxView)
        {
            var model = new ResetRotationButtonModel(fbxView);
            model.Button.Subscribe(button.Bind).AddTo(this);
        }
    }
}