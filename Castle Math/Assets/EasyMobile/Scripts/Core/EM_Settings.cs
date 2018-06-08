using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace EasyMobile
{
    public class EM_Settings : ScriptableObject
    {
        #region Public members

        public static EM_Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = LoadSettingsAsset();

                    if (_instance == null)
                    {
                        #if !UNITY_EDITOR
                        Debug.LogError("Easy Mobile settings not found! " +
                            "Please go to Tools>Easy Mobile>Settings to setup the plugin.");
                        #endif
                        _instance = CreateInstance<EM_Settings>();   // Create a dummy scriptable object for temporary use.
                    }
                }

                return _instance;
            }
        }

        public static EM_Settings LoadSettingsAsset()
        {
            return Resources.Load("EM_Settings") as EM_Settings;
        }

        // ----- Module settings
        public static AdSettings Advertising { get { return Instance._advertisingSettings; } }

        public static GameServicesSettings GameServices { get { return Instance._gameServiceSettings; } }

        public static IAPSettings InAppPurchasing { get { return Instance._inAppPurchaseSettings; } }

        public static NotificationsSettings Notifications { get { return Instance._notificationSettings; } }

        // Rating Request (Store Review) belongs to Utilities module
        public static RatingRequestSettings RatingRequest { get { return Instance._ratingRequestSettings; } }


        // ----- Module toggles
        public static bool IsAdModuleEnable { get { return Instance._isAdModuleEnable; } }

        public static bool IsIAPModuleEnable{ get { return Instance._isIAPModuleEnable; } }

        public static bool IsGameServicesModuleEnable{ get { return Instance._isGameServiceModuleEnable; } }

        public static bool IsNotificationsModuleEnable { get { return Instance._isNotificationModuleEnable; } }

        #endregion

        #region Private members

        private static EM_Settings _instance;

        [SerializeField]
        private AdSettings _advertisingSettings;
        [SerializeField]
        private GameServicesSettings _gameServiceSettings;
        [SerializeField]
        private IAPSettings _inAppPurchaseSettings;
        [SerializeField]
        private NotificationsSettings _notificationSettings;
        [SerializeField]
        private RatingRequestSettings _ratingRequestSettings;

        [SerializeField]
        private bool _isAdModuleEnable = false;
        [SerializeField]
        private bool _isIAPModuleEnable = false;
        [SerializeField]
        private bool _isGameServiceModuleEnable = false;
        [SerializeField]
        private bool _isNotificationModuleEnable = false;

        #endregion

        #region Editor stuff

        #if UNITY_EDITOR
        // Index of the active module on the toolbar.
        // This field is only used as a SerializedProperty in the editor scripts, hence the warning suppression.
        #pragma warning disable 0414
        [SerializeField]
        private int _activeModuleIndex = 0;
        #pragma warning restore 0414

        #endif

        #endregion
    }
}

