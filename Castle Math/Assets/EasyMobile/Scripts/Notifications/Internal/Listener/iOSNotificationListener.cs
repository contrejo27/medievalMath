#if UNITY_IOS
using UnityEngine;
using System;
using System.Collections;
using AOT;
using EasyMobile.Internal;
using EasyMobile.NotificationsInternal.iOS;

namespace EasyMobile.NotificationsInternal
{
    // Dedicated MonoBehaviour for listening to notification events from iOS native.
    internal class iOSNotificationListener : MonoBehaviour, INotificationListener
    {
        private const string IOS_NOTIFICATION_LISTENER_GAMEOBJECT = "EM_iOSNotificationListener";

        // Singleton: we only need one listener object.
        private static iOSNotificationListener sInstance;

        /// <summary>
        /// Creates a gameobject for use with UnitySendMessage from native side.
        /// Must be called from Unity game thread.
        /// </summary>
        internal static iOSNotificationListener GetListener()
        {
            if (sInstance == null)
            {
                var go = new GameObject(IOS_NOTIFICATION_LISTENER_GAMEOBJECT);
                go.hideFlags = HideFlags.HideAndDontSave;
                sInstance = go.AddComponent<iOSNotificationListener>();
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

        public event Action<LocalNotification> LocalNotificationOpened;

        public event Action<RemoteNotification> RemoteNotificationOpened;

        public string Name
        {
            get { return gameObject.name; }
        }

        public NativeNotificationHandler NativeNotificationFromForegroundHandler
        { 
            get { return this._OnNativeNotificationFromForeground; }
        }

        public NativeNotificationHandler NativeNotificationFromBackgroundHandler
        { 
            get { return this._OnNativeNotificationFromBackground; }
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
        // Native Notification Event Handlers
        // Note that on iOS we'll handle both local and remote notification events.
        //--------------------------------------------------------

        private void _OnNativeNotificationFromForeground(string notifID)
        {
            InternalOnNativeNotificationEvent(true, notifID);
        }

        private void _OnNativeNotificationFromBackground(string notifID)
        {
            InternalOnNativeNotificationEvent(false, notifID);
        }

        #if EM_ONESIGNAL
        //--------------------------------------------------------
        // OneSignal Event Handlers
        // On iOS, OneSignal's NotificationReceived & NotificationOpened will not really fire,
        // because all notification events are handled by us from native side.
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

        #region Internal Stuff

        IEnumerator CRRaiseEvents(string actionId, NotificationRequest request, bool isForeground, bool isRemote)
        {
            // This could be called at app-launch-from-notification, so we'd better
            // check if the Helper is ready before asking it to schedule a job on main thread.
            while (!Helper.IsInitialized())
                yield return new WaitForSeconds(0.1f);
            
            Helper.RunOnMainThread(() =>
                {
                    if (isRemote)
                    {
                        var delivered = new RemoteNotification(
                                            request.id,
                                            actionId,
                                            request.content,
                                            isForeground,
                                            isForeground ? false : true
                                        );

                        if (RemoteNotificationOpened != null)
                            RemoteNotificationOpened(delivered);
                    }
                    else
                    {
                        // Local notification.
                        var delivered = new LocalNotification(
                                            request.id,
                                            actionId,
                                            request.content,
                                            isForeground,
                                            isForeground ? false : true // isOpened
                                        );

                        if (LocalNotificationOpened != null)
                            LocalNotificationOpened(delivered);
                    }
                });
        }

        void InternalOnNativeNotificationEvent(bool isForeground, string notifId)
        {
            InternalGetNotificationResponse(
                notifId,
                response =>
                {
                    if (response == null)
                    {
                        Debug.Log("Ignoring iOS notification due to invalid response data.");
                        return;
                    }

                    var actionID = response.ActionIdentifier;
                    var iOSRequest = response.GetRequest();

                    if (iOSRequest == null)
                    {
                        Debug.Log("Ignoring iOS notification due to NULL request data.");
                        return;
                    }

                    bool isRemote;
                    var request = iOSNotificationHelper.ToCrossPlatformNotificationRequest(iOSRequest, out isRemote);
                    StartCoroutine(CRRaiseEvents(actionID, request, isForeground, isRemote));
                });
        }

        private static void InternalGetNotificationResponse(string identifier, Action<iOSNotificationResponse> callback)
        {
            Helper.NullArgumentTest(callback);

            iOSNotificationNative._GetNotificationResponse(
                identifier,
                InternalGetNotificationResponseCallback,
                PInvokeCallbackUtil.ToIntPtr<iOSNotificationResponse>(
                    response =>
                    {
                        callback(response);
                    },
                    iOSNotificationResponse.FromPointer
                )
            );
        }

        [MonoPInvokeCallback(typeof(iOSNotificationNative.GetNotificationResponseCallback))]
        private static void InternalGetNotificationResponseCallback(IntPtr response, IntPtr callbackPtr)
        {
            PInvokeCallbackUtil.PerformInternalCallback(
                "iOSNotificationListener#InternalGetNotificationResponseCallback",
                PInvokeCallbackUtil.Type.Temporary,
                response,
                callbackPtr);
        }

        #endregion // Internal Stuff
    }
}

#endif