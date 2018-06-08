#if UNITY_ANDROID
using UnityEngine;
using System;
using System.Collections;
using EasyMobile.Internal;
using EasyMobile.NotificationsInternal.Android;

namespace EasyMobile.NotificationsInternal
{
    // Dedicated MonoBehaviour for listening to notification messages from Android native side.
    internal class AndroidNotificationListener : MonoBehaviour, INotificationListener
    {
        private const string ANDROID_NOTIFICATION_LISTENER_GAMEOBJECT = "EM_AndroidNotificationListener";

        // Singleton: we only need one listener object.
        private static AndroidNotificationListener sInstance;

        /// <summary>
        /// Creates a gameobject for use with UnitySendMessage from native side.
        /// Must be called from Unity game thread.
        /// </summary>
        internal static AndroidNotificationListener GetListener()
        {
            if (sInstance == null)
            {
                var go = new GameObject(ANDROID_NOTIFICATION_LISTENER_GAMEOBJECT);
                go.hideFlags = HideFlags.HideAndDontSave;
                sInstance = go.AddComponent<AndroidNotificationListener>();
                DontDestroyOnLoad(go);
            }
            return sInstance;
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void OnDisable()
        {
            if (sInstance == this)
            {
                sInstance = null;
            }
        }

        #region INotificationListener Implementation

        #pragma warning disable 0067
        public event Action<LocalNotification> LocalNotificationOpened;

        public event Action<RemoteNotification> RemoteNotificationOpened;
        #pragma warning restore 0067

        public string Name
        {
            get { return gameObject.name; }
        }

        public NativeNotificationHandler NativeNotificationFromForegroundHandler
        { 
            get { return this._OnLocalNotificationFromForeground; }
        }

        public NativeNotificationHandler NativeNotificationFromBackgroundHandler
        { 
            get { return this._OnLocalNotificationFromBackground; }
        }

        #if EM_ONESIGNAL
        public OneSignal.NotificationReceived OnOneSignalNotificationReceived
        { 
            get { return this.HandleOneSignalNotificationReceived; }
        }

        public OneSignal.NotificationOpened OnOneSignalNotificationOpened
        { 
            get { return this.HandleOneSignalNotificationOpened; }
        }
        #endif

        #endregion // INotificationListener Implementation

        #region Internal Notification Event Handlers

        //--------------------------------------------------------
        // Native Local Notification Event Handlers
        //--------------------------------------------------------

        private void _OnLocalNotificationFromForeground(string jsonResponse)
        {
            InternalOnLocalNotificationHandler(true, jsonResponse);
        }

        private void _OnLocalNotificationFromBackground(string jsonResponse)
        {
            InternalOnLocalNotificationHandler(false, jsonResponse);
        }

        void InternalOnLocalNotificationHandler(bool isForeground, String jsonResponse)
        {
            var response = AndroidNotificationResponse.FromJson(jsonResponse);

            if (response == null || response.request == null)
            {
                Debug.Log("Ignoring local Android notification due to invalid JSON response data.");
                return;
            }

            StartCoroutine(CRRaiseLocalNotificationEvent(response, isForeground)); 
        }

        IEnumerator CRRaiseLocalNotificationEvent(AndroidNotificationResponse response, bool isForeground)
        {
            // This could be called at app-launch-from-notification, so we'd better
            // check if the Helper is ready before asking it to schedule a job on main thread.
            while (!Helper.IsInitialized())
                yield return new WaitForSeconds(0.1f);

            Helper.RunOnMainThread(() =>
                {
                    string actionId = response.actionId;
                    var request = response.request.ToCrossPlatformNotificationRequest();

                    var delivered = new LocalNotification(
                                        request.id,
                                        actionId,
                                        request.content,
                                        isForeground,
                                        isForeground ? false : true // isOpened
                                    );

                    if (LocalNotificationOpened != null)
                        LocalNotificationOpened(delivered);
                });
        }

        #if EM_ONESIGNAL
        //--------------------------------------------------------
        // OneSignal Remote Notification Event Handlers
        //--------------------------------------------------------

        // Called when your app is in focus and a notification is recieved (no action taken by the user).
        private void HandleOneSignalNotificationReceived(OSNotification notification)
        {
            var delivered = OneSignalHelper.ToCrossPlatformRemoteNotification(null, notification);

            // Fire event
            if (RemoteNotificationOpened != null)
                RemoteNotificationOpened(delivered);
        }


        // Called when a notification is opened by the user.
        private void HandleOneSignalNotificationOpened(OSNotificationOpenedResult result)
        {
            var delivered = OneSignalHelper.ToCrossPlatformRemoteNotification(result);

            // Fire event
            if (RemoteNotificationOpened != null)
                RemoteNotificationOpened(delivered);

        }
        #endif

        #endregion // Internal Notification Event Handlers
    }
}
#endif
