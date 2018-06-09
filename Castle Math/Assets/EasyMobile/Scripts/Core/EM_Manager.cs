using UnityEngine;
using System.Collections;
using System;
using EasyMobile.Internal;

namespace EasyMobile
{
    public class EM_Manager : MonoBehaviour
    {
        public static EM_Manager Instance { get; private set; }

        private const string APP_INSTALLATION_TIMESTAMP_PPKEY = "EM_APP_INSTALLATION_TIMESTAMP";

        private static IAppLifecycleHandler AppLifecycleHandler
        { 
            get
            { 
                if (sAppLifecycleHandler == null)
                    sAppLifecycleHandler = GetPlatformAppLifecycleHandler();
                return sAppLifecycleHandler;
            }
        }

        private static IAppLifecycleHandler sAppLifecycleHandler;

        #region MonoBehavior Events

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                // Initialize Helper.
                Helper.Init();

                // Store installation timestamp.
                if (Helper.GetTime(APP_INSTALLATION_TIMESTAMP_PPKEY, Helper.UnixEpoch) == Helper.UnixEpoch)
                {
                    // No timestamp was stored previously. Store the current local time as installation time.
                    Helper.StoreTime(APP_INSTALLATION_TIMESTAMP_PPKEY, DateTime.Now);
                }
            }
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        void OnApplicationFocus(bool isFocus)
        {
            AppLifecycleHandler.OnApplicationFocus(isFocus);
        }

        void OnApplicationPause(bool isPaused)
        {
            AppLifecycleHandler.OnApplicationPause(isPaused);
        }

        void OnApplicationQuit()
        {
            AppLifecycleHandler.OnApplicationQuit();
        }

        #endregion

        #region Public API

        /// <summary>
        /// Gets the installation timestamp of this app (in local timezone).
        /// </summary>
        /// <returns>The installation timestamp.</returns>
        public DateTime GetAppInstallationTimestamp()
        {
            return Helper.GetTime(APP_INSTALLATION_TIMESTAMP_PPKEY, Helper.UnixEpoch);
        }

        #endregion

        #region Internal Stuff

        private static void SetLogEnabled(bool isEnabled)
        {
            #if UNITY_2017_1_OR_NEWER
            Debug.unityLogger.logEnabled = isEnabled;
            #else
            Debug.logger.logEnabled = isEnabled;
            #endif
        }

        private static IAppLifecycleHandler GetPlatformAppLifecycleHandler()
        {
            #if UNITY_EDITOR
            return new DummyAppLifecycleHandler();
            #elif UNITY_IOS
            return new DummyAppLifecycleHandler();
            #elif UNITY_ANDROID
            return new AndroidAppLifecycleHandler();
            #else
            return new DummyAppLifecycleHandler();
            #endif
        }

        #endregion
    }
}

