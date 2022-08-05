using System;
using Michsky.UI.ModernUIPack;
using UI.Models;
using UI.Views;
using UniRx;
using UnityEngine;

namespace UI
{
    public class UiBlocker : MonoBehaviour, IUIBlocker
    {
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private GameObject waitIcon;
        [SerializeField] private GameObject uiBlockPanel;
        [SerializeField] private ButtonView cancelButton;

        private readonly CompositeDisposable disposable = new();

        private void Awake()
        {
            disposable.AddTo(this);
        }

        public void Show(Subject<float> progressSubject, Action onCancel = null)
        {
            BindCancelButton(onCancel);
            uiBlockPanel.SetActive(true);
            waitIcon.SetActive(true);
            progressBar.gameObject.SetActive(false);
            progressSubject.Subscribe(UpdateProgressBar).AddTo(this);
        }

        public void Hide()
        {
            uiBlockPanel.SetActive(false);
            UnbindCancelButton();
        }

        private void UpdateProgressBar(float value)
        {
            if (value is < 0 or > 1)
                throw new ArgumentOutOfRangeException($"{nameof(value)} must be between 0 and 1.");

            waitIcon.SetActive(false);
            progressBar.gameObject.SetActive(true);
            uiBlockPanel.SetActive(true);
            progressBar.currentPercent = value * 100;
            if (value >= 1) Hide();
        }

        private void BindCancelButton(Action onCancel)
        {
            UnbindCancelButton();
            var model = new ButtonModel();
            model.AddTo(disposable);
            model.Click.Subscribe(_ => onCancel?.Invoke()).AddTo(disposable);
            cancelButton.Bind(model);
        }

        private void UnbindCancelButton()
        {
            disposable.Clear();
        }
    }
}