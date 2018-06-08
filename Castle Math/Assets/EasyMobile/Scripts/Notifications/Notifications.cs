using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EasyMobile.NotificationsInternal;

namespace EasyMobile
{
    [AddComponentMenu("")]
    public class Notifications : MonoBehaviour
    {
        // Singleton pattern.
        public static Notifications Instance { get; private set; }

        // Fill this if we need a common prefix for local notification IDs.
        private const string LOCAL_NOTIF_ID_PREFIX = "";
        private const string LOCAL_NOTIF_CURRENT_ID_PPKEY = "EM_LOCAL_NOTIF_CURRENT_ID";

        /// <summary>
        /// Gets the current push notification service.
        /// </summary>
        /// <value>The current push notification service.</value>
        public static PushNotificationProvider CurrentPushNotificationService
        {
            get { return EM_Settings.Notifications.PushNotificationService; }
        }

        /// <summary>
        /// Occurs when a remote notification is opened, either by the default open action,
        /// or by a custom notification action button.
        /// Note that if the notification arrives when the app is in foreground it won't be
        /// posted to the notification center. Instead, this event will be raised immediately
        /// as if the notification was opened by the user.
        /// </summary>
        public static event Action<RemoteNotification> RemoteNotificationOpened;

        /// <summary>
        /// Occurs when a local notification is opened, either by the default open action,
        /// or by a custom notification action button.
        /// Note that if the notification arrives when the app is in foreground it won't be
        /// posted to the notification center. Instead, this event will be raised immediately
        /// as if the notification was opened by the user.
        /// </summary>
        public static event Action<LocalNotification> LocalNotificationOpened;

        private static ILocalNotificationClient LocalNotificationClient
        {
            get
            { 
                if (sLocalNotificationClient == null)
                {
                    sLocalNotificationClient = GetLocalNotificationClient();
                }
                return sLocalNotificationClient;
            }
        }

        // Platform-dependent local notification client.
        private static ILocalNotificationClient sLocalNotificationClient;

        // Platform-dependent notification event listeners.
        private static INotificationListener sListener;

        #region Public API

        /// <summary>
        /// Initializes the notification service.
        /// </summary>
        public static void Init()
        {
            // Get the listener.
            sListener = GetNotificationListener();

            // Subscibe to internal notification events.
            if (sListener != null)
            {
                sListener.LocalNotificationOpened += InternalOnLocalNotificationOpened;
                sListener.RemoteNotificationOpened += InternalOnRemoteNotificationOpened;
            }

            // Initialize remote notification service.
            // On iOS, OneSignal's NotificationReceived & NotificationOpened will not really fire,
            // because all notification events are handled by us from native side.
            if (EM_Settings.Notifications.PushNotificationService == PushNotificationProvider.OneSignal)
            {
                #if EM_ONESIGNAL
                // The only required method you need to call to setup OneSignal to recieve push notifications.
                // Call before using any other methods on OneSignal.
                // Should only be called once when your game is loaded.
                OneSignal.StartInit(EM_Settings.Notifications.OneSignalAppId)
                .HandleNotificationReceived(sListener.OnOneSignalNotificationReceived)
                .HandleNotificationOpened(sListener.OnOneSignalNotificationOpened)
                .InFocusDisplaying(OneSignal.OSInFocusDisplayOption.None)
                .EndInit();

                #else
                Debug.LogError("SDK missing. Please import OneSignal plugin for Unity.");
                #endif
            }

            // Initialize local notification client.
            // We may need to override some configuration done by the initialization
            // of the remote notification service, so this should be done later.
            LocalNotificationClient.Init(EM_Settings.Notifications, sListener);
        }

        /// <summary>
        /// Determines if the service is initialized and notifications can be posted.
        /// </summary>
        /// <returns><c>true</c> if is initialized; otherwise, <c>false</c>.</returns>
        public static bool IsInitialized()
        {
            return LocalNotificationClient.IsInitialized();
        }

        /// <summary>
        /// Schedules a local notification to be posted at the specified time with no repeat.
        /// Note that the scheduled notification persists even if the device reboots, and it
        /// will be fired immediately after the reboot if <see cref="triggerDate"/> is past.
        /// </summary>
        /// <returns>The ID of the scheduled notification.</returns>
        /// <param name="triggerDate">Trigger date.</param>
        /// <param name="content">Notification content.</param>
        public static string ScheduleLocalNotification(DateTime triggerDate, NotificationContent content)
        {
            var id = NextLocalNotificationId();
            LocalNotificationClient.ScheduleLocalNotification(id, triggerDate, content);
            return id;
        }

        /// <summary>
        /// Schedules a local notification to be posted after the specified delay time with no repeat.
        /// Note that the scheduled notification persists even if the device reboots, and it
        /// will be fired immediately after the reboot if <see cref="delay"/> has passed.
        /// </summary>
        /// <returns>The ID of the scheduled notification.</returns>
        /// <param name="delay">Delay.</param>
        /// <param name="content">Notification content.</param>
        public static string ScheduleLocalNotification(TimeSpan delay, NotificationContent content)
        {
            var id = NextLocalNotificationId();
            LocalNotificationClient.ScheduleLocalNotification(id, delay, content);
            return id;
        }

        /// <summary>
        /// Schedules a local notification to be posted after the specified delay time,
        /// and repeat automatically after the interval specified by the repeat mode.
        /// Note that the scheduled notification persists even if the device reboots, and it
        /// will be fired immediately after the reboot if its latest scheduled fire time has passed.
        /// </summary>
        /// <returns>The ID of the scheduled notification.</returns>
        /// <param name="delay">Delay.</param>
        /// <param name="content">Notification content.</param>
        /// <param name="repeat">Repeat.</param>
        public static string ScheduleLocalNotification(TimeSpan delay, NotificationContent content, NotificationRepeat repeat)
        {
            var id = NextLocalNotificationId();
            LocalNotificationClient.ScheduleLocalNotification(id, delay, content, repeat);
            return id;
        }

        /// <summary>
        /// Gets all scheduled local notifications that haven't been posted.
        /// </summary>
        /// <param name="callback">The callback that is invoked when this operation finishes.
        /// This callback receives an array of all pending notification requests.
        /// If there's no pending notifications, an empty array will be returned.
        /// This callback will always execute on the main thread (game thread)./></param>
        public static void GetPendingLocalNotifications(Action<NotificationRequest[]> callback)
        {
            LocalNotificationClient.GetPendingLocalNotifications(callback);
        }

        /// <summary>
        /// Cancels the pending local notification with the specified ID.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public static void CancelPendingLocalNotification(string id)
        {
            LocalNotificationClient.CancelPendingLocalNotification(id);
        }

        /// <summary>
        /// Cancels all pending local notifications.
        /// </summary>
        public static void CancelAllPendingLocalNotifications()
        {
            LocalNotificationClient.CancelAllPendingLocalNotifications();   
        }

        /// <summary>
        /// Removes all previously shown notifications of this app from the notification center or status bar.
        /// </summary>
        public static void ClearAllDeliveredNotifications()
        {
            LocalNotificationClient.RemoveAllDeliveredNotifications();
        }

        #endregion

        #region Internal stuff

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            if (EM_Settings.Notifications.IsAutoInit)
            {
                StartCoroutine(CRAutoInit(EM_Settings.Notifications.AutoInitDelay));
            }
        }

        IEnumerator CRAutoInit(float delay)
        {
            yield return new WaitForSeconds(delay);            
            Init();
        }

        static INotificationListener GetNotificationListener()
        {
            #if UNITY_EDITOR
            return EditorNotificationListener.GetListener();
            #elif UNITY_IOS
            return iOSNotificationListener.GetListener();
            #elif UNITY_ANDROID
            return AndroidNotificationListener.GetListener();
            #else
            return UnsupportedNotificationListener.GetListener();
            #endif
        }

        static ILocalNotificationClient GetLocalNotificationClient()
        {
            #if UNITY_EDITOR
            return new EditorLocalNotificationClient();
            #elif UNITY_IOS
            return new iOSLocalNotificationClient();
            #elif UNITY_ANDROID
            return new AndroidLocalNotificationClient();
            #else
            return new UnsupportedLocalNotificationClient();
            #endif
        }

        static string NextLocalNotificationId()
        {
            int currentId = PlayerPrefs.GetInt(LOCAL_NOTIF_CURRENT_ID_PPKEY, 0);
            int nextId = currentId == int.MaxValue ? 1 : currentId + 1;
            PlayerPrefs.SetInt(LOCAL_NOTIF_CURRENT_ID_PPKEY, nextId);
            PlayerPrefs.Save();
            return LOCAL_NOTIF_ID_PREFIX + nextId.ToString();
        }

        static void InternalOnLocalNotificationOpened(LocalNotification delivered)
        {
            if (LocalNotificationOpened != null)
                LocalNotificationOpened(delivered);
        }

        static void InternalOnRemoteNotificationOpened(RemoteNotification delivered)
        {
            if (RemoteNotificationOpened != null)
                RemoteNotificationOpened(delivered);
        }

        #endregion
    }
}