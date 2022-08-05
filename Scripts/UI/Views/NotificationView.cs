using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Michsky.UI.ModernUIPack;
using ModestTree;
using Services.LocalizationService;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Views
{
    public class NotificationView : MonoBehaviour, IInitializable
    {
        private const float MinTime = 2f;
        private const float MaxTime = 15f;
        private const float TimerDivider = 20f;
        private const float DelayBetweenNotifications = 0.5f;

        [SerializeField] private NotificationManager notificationManager;
        [SerializeField] private GameObject notificationIcon;
        [SerializeField] private GameObject notificationIconFilled;
        [SerializeField] private TMP_Text notificationCountText;
        [SerializeField] private Button notificationCloseButton;

        [Inject] private readonly ILocalizationService localizationService;
        
        private Queue<Notification> notifications = new Queue<Notification>();
        private bool canShowOnClick = true;
        private CancellationTokenSource notificationCancellationTokenSource;

        public void Initialize()
        {
            notificationIcon.SetActive(false);
            notificationIconFilled.SetActive(false);
            notificationCloseButton.OnClickAsObservable().Subscribe(x => CloseNotification())
                .AddTo(notificationCloseButton);
            Application.logMessageReceived += CatchLogError;
        }
        
        public void AddNotification(string text, LogType type)
        {
            var notification = new Notification
            {
                Type = type,
                Text = text
            };

            notifications.Enqueue(notification);
            UpdateNotificationCount();

            if (!canShowOnClick) return;
            canShowOnClick = false;
            ShowNotification(notifications.Dequeue());
        }
        
        private void CatchLogError(string condition, string stacktrace, LogType type)
        {
            if (type == LogType.Error) AddNotification(condition, type);
        }        

        private async void CloseNotification()
        {
            notificationCloseButton.interactable = false;
            notificationManager.CloseNotification();
            notificationCancellationTokenSource.Cancel();

            await UniTask.Delay(TimeSpan.FromSeconds(DelayBetweenNotifications));

            ShowNextNotification();
        }

        private void ShowNotification(Notification notification)
        {
            notificationCloseButton.interactable = true;
            UpdateNotificationCount();
            SetNotificationData(notification, out var displayNotificationTimer);
            notificationManager.OpenNotification();

            notificationCancellationTokenSource = new CancellationTokenSource();
            DelayForNotificationShowing(notificationCancellationTokenSource.Token, displayNotificationTimer).Forget();
        }

        private async UniTask DelayForNotificationShowing(CancellationToken token, float displayNotificationTimer)
        {
            token.ThrowIfCancellationRequested();
            await UniTask.Delay(TimeSpan.FromSeconds(displayNotificationTimer + DelayBetweenNotifications), false,
                PlayerLoopTiming.PostLateUpdate, token);

            ShowNextNotification();
        }

        private void ShowNextNotification()
        {
            if (notifications.Count > 0)
            {
                ShowNotification(notifications.Dequeue());
            }
            else
            {
                canShowOnClick = true;
                notificationIcon.SetActive(false);
            }
        }

        private void SetNotificationData(Notification notification, out float displayNotificationTimer)
        {
            displayNotificationTimer = MinTime + notification.Text.Length / TimerDivider;
            if (displayNotificationTimer > MaxTime) displayNotificationTimer = MaxTime;
            notificationManager.timer = displayNotificationTimer;

            notificationManager.title = localizationService.Localize(notification.Type.ToString());
            notificationManager.description = notification.Text;
            notificationManager.UpdateUI();
        }

        private void UpdateNotificationCount()
        {
            var hasNotifications = !notifications.IsEmpty();
            notificationIcon.SetActive(!hasNotifications);
            notificationIconFilled.SetActive(hasNotifications);
            notificationCountText.text = "+" + notifications.Count;
        }
    }

    public class Notification
    {
        public LogType Type;
        public string Text;
    }
}