using System.Collections.Generic;
using Constructor.Validator;
using Constructor.Validator.Validations;
using Cysharp.Threading.Tasks;
using Michsky.UI.ModernUIPack;
using UI.Factories;
using UI.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    [RequireComponent(typeof(ModalWindowManager))]
    public class ModalWindow : MonoBehaviour, IModalWindow
    {
        private ModalWindowManager modalWindowManager;
        
        private void Awake()
        {
            modalWindowManager = GetComponent<ModalWindowManager>();
        }

        public async UniTask<bool> Show(string windowTitle, string descriptionText)
        {
            var inputDone = false;
            var isConfirmed = false;
            modalWindowManager.titleText = windowTitle;
            modalWindowManager.descriptionText = descriptionText;
            modalWindowManager.OpenWindow();
            modalWindowManager.confirmButton.onClick
                .AsObservable()
                .Subscribe(_ =>
                {
                    isConfirmed = true;
                    inputDone = true;
                }).AddTo(this);
        
            modalWindowManager.cancelButton.onClick
                .AsObservable()
                .Subscribe(_ =>
                {
                    isConfirmed = false;
                    inputDone = true;
                }).AddTo(this);
        
            await UniTask.WaitUntil(() => inputDone);
            Hide();
            return isConfirmed;
        }

        public void Hide()
        {
            modalWindowManager.CloseWindow();
        }
    }
}
