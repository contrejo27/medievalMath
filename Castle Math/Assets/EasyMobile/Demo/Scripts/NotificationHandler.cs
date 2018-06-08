using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using EasyMobile.MiniJSON;

namespace EasyMobile.Demo
{
    public class NotificationHandler : MonoBehaviour
    {
        public static NotificationHandler Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnEnable()
        {
            Notifications.LocalNotificationOpened += OnLocalNotificationOpened;
            Notifications.RemoteNotificationOpened += OnPushNotificationOpened;
        }

        void OnDisable()
        {
            Notifications.LocalNotificationOpened -= OnLocalNotificationOpened;
            Notifications.RemoteNotificationOpened -= OnPushNotificationOpened;
        }

        void OnLocalNotificationOpened(LocalNotification delivered)
        {
            DisplayNotification(delivered, false);
        }

        // Push notification opened handler
        void OnPushNotificationOpened(RemoteNotification delivered)
        {
            DisplayNotification(delivered, true);
        }

        void DisplayNotification(Notification delivered, bool isRemote)
        {
            var content = delivered.content;
            var sb = new StringBuilder();

            bool hasNewUpdate = content.userInfo.ContainsKey("newUpdate");

            if (hasNewUpdate)
            {
                sb.Append("A new version is available. Do you want to update now?\n");
            }

            sb.Append("----- NOTIFICATION DATA -----\n")
            .Append("ActionID: " + delivered.actionId + "\n")
            .Append("isAppInForeground: " + delivered.isAppInForeground + "\n")
                .Append("isOpened: " + delivered.isOpened + "\n")
                .Append("Title: " + content.title + "\n")
                .Append("Body: " + content.body + "\n")
                .Append("Badge: " + content.badge.ToString() + "\n")
                .Append("CategoryID: " + content.categoryId + "\n")
                .Append("UserInfo: " + Json.Serialize(content.userInfo));

            string popupTitle;
            if (isRemote)
                popupTitle = Notifications.CurrentPushNotificationService == PushNotificationProvider.OneSignal ?
                "OneSignal Push Notification Received" : "Remote Notification Received";
            else
                popupTitle = "Local Notification Received";

            StartCoroutine(CRWaitAndShowPopup(hasNewUpdate, popupTitle, sb.ToString()));

            // Print original OneSignal payload for debug purpose.
            if (isRemote && Notifications.CurrentPushNotificationService == PushNotificationProvider.OneSignal)
            {
                var oneSignalPayload = ((RemoteNotification)delivered).oneSignalPayload;

                if (oneSignalPayload == null)
                {
                    Debug.Log("Something wrong: using OneSignal service but oneSignalPayload was not initialized.");
                }
                else
                {
                    Debug.Log("----- START ONESIGNAL PAYLOAD -----");
                    Debug.Log("notificationID: " + oneSignalPayload.notificationID);
                    Debug.Log("sound: " + oneSignalPayload.sound);
                    Debug.Log("title: " + oneSignalPayload.title);
                    Debug.Log("body: " + oneSignalPayload.body);
                    Debug.Log("subtitle: " + oneSignalPayload.subtitle);
                    Debug.Log("launchURL: " + oneSignalPayload.launchURL);
                    Debug.Log("additionalData: " + Json.Serialize(oneSignalPayload.additionalData));
                    Debug.Log("actionButtons: " + Json.Serialize(oneSignalPayload.actionButtons));
                    Debug.Log("contentAvailable: " + oneSignalPayload.contentAvailable.ToString());
                    Debug.Log("badge: " + oneSignalPayload.badge);
                    Debug.Log("smallIcon: " + oneSignalPayload.smallIcon);
                    Debug.Log("largeIcon: " + oneSignalPayload.largeIcon);
                    Debug.Log("bigPicture: " + oneSignalPayload.bigPicture);
                    Debug.Log("smallIconAccentColor: " + oneSignalPayload.smallIconAccentColor);
                    Debug.Log("ledColor: " + oneSignalPayload.ledColor);
                    Debug.Log("lockScreenVisibility: " + oneSignalPayload.lockScreenVisibility);
                    Debug.Log("groupKey: " + oneSignalPayload.groupKey);
                    Debug.Log("groupMessage: " + oneSignalPayload.groupMessage);
                    Debug.Log("fromProjectNumber: " + oneSignalPayload.fromProjectNumber);
                    Debug.Log("----- END ONESIGNAL PAYLOAD -----");
                }
            }
        }

        IEnumerator CRWaitAndShowPopup(bool hasNewUpdate, string title, string message)
        {
            // Wait until no other alert is showing.
            while (NativeUI.IsShowingAlert())
                yield return new WaitForSeconds(0.1f);
            
            if (!hasNewUpdate)
                NativeUI.Alert(title, message);
            else
            {
                NativeUI.AlertPopup alert = NativeUI.ShowTwoButtonAlert(
                                                title,
                                                message,
                                                "Yes",
                                                "No");

                if (alert != null)
                {
                    alert.OnComplete += (int button) =>
                    {
                        if (button == 0)
                        {
                            NativeUI.Alert(
                                "Open App Store", 
                                "The user has opted to update! In a real app you would want to open the app store for them to download the new version.");
                        }
                    };
                }
            }
        }
    }
}