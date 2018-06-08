using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SgLib.Editor;
using EasyMobile;

namespace EasyMobile.Editor
{
    [CustomEditor(typeof(EM_Settings))]
    public partial class EM_SettingsEditor : UnityEditor.Editor
    {
        #region Modules

        public enum Module
        {
            Advertising = 0,
            InAppPurchase = 1,
            GameService = 2,
            Notification = 3,
            Utility = 4
        }

        static Module activeModule = Module.Advertising;

        #endregion

        #region Target properties

        // Module toggles
        SerializedProperty isAdModuleEnable;
        SerializedProperty isIAPModuleEnable;
        SerializedProperty isGameServiceModuleEnable;
        SerializedProperty isNotificationModuleEnable;

        // Active module (currently selected on the toolbar)
        SerializedProperty activeModuleIndex;

        public class EMProperty
        {
            public SerializedProperty property;
            public GUIContent content;

            public EMProperty(SerializedProperty p, GUIContent c)
            {
                property = p;
                content = c;
            }
        }

        // Ad module properties
        private static class AdProperties
        {
            public static SerializedProperty mainProperty;
            public static EMProperty iosAdColonyConfig = new EMProperty(null, new GUIContent("iOS"));
            public static EMProperty androidAdColonyConfig = new EMProperty(null, new GUIContent("Android"));
            public static EMProperty adColonyAdOrientation = new EMProperty(null, new GUIContent("Ad Orientation"));
            public static EMProperty adColonyShowRewardedAdPrePopup = new EMProperty(null, new GUIContent("Show Rewarded Ad PrePopup", "Show popup before the rewarded video starts"));
            public static EMProperty adColonyShowRewardedAdPostPopup = new EMProperty(null, new GUIContent("Show Rewarded Ad PostPopup", "Show popup after the rewarded video has finished"));
            public static EMProperty iosAdMobConfig = new EMProperty(null, new GUIContent("iOS"));
            public static EMProperty androidAdMobConfig = new EMProperty(null, new GUIContent("Android"));
            public static EMProperty admobTargeting = new EMProperty(null, new GUIContent("Ad Targeting", "AdMob ad targeting settings"));
            public static EMProperty admobEnableTestMode = new EMProperty(null, new GUIContent("Enable Test Mode", "Enable test ads on devices registered in the Test Device Ids list"));
            public static EMProperty admobTestDeviceIds = new EMProperty(null, new GUIContent("Test Device Ids", "Hashed Ids of testing devices"));
            public static EMProperty heyzapPublisherId = new EMProperty(null, new GUIContent("Heyzap Publisher Id"));
            public static EMProperty heyzapShowTestSuite = new EMProperty(null, new GUIContent("Show Heyzap Test Suite"));
            public static EMProperty autoLoadDefaultAds = new EMProperty(null, new GUIContent("Auto-Load Default Ads", "Automatically load ads from default ad networks"));
            public static EMProperty adCheckingInterval = new EMProperty(null, new GUIContent("Ad Checking Interval", "Time (seconds) between 2 checks (to see if ads were loaded)"));
            public static EMProperty adLoadingInterval = new EMProperty(null, new GUIContent("Ad Loading Interval", "Minimum time (seconds) between two ad-loading requests (to restrict the number of requests sent to ad networks)"));
            public static EMProperty iosDefaultAdNetworks = new EMProperty(null, new GUIContent("iOS"));
            public static EMProperty androidDefaultAdNetworks = new EMProperty(null, new GUIContent("Android"));
        }

        // In App Purchase module properties
        private static class IAPProperties
        {
            public static SerializedProperty mainProperty;
            public static EMProperty targetAndroidStore = new EMProperty(null, new GUIContent("Target Android Store", "Target Android store"));
            public static EMProperty validateAppleReceipt = new EMProperty(null, new GUIContent("Validate Apple Receipt", "Validate receipts from Apple App stores"));
            public static EMProperty validateGooglePlayReceipt = new EMProperty(null, new GUIContent("Validate Google Play Receipt", "Validate receipts from Google Play store"));
            public static EMProperty products = new EMProperty(null, new GUIContent("Products"));
        }
            
        // Game Service module properties
        private static class GameServiceProperties
        {
            public static SerializedProperty mainProperty;
            public static EMProperty gpgsDebugLog = new EMProperty(null, new GUIContent("GPGS Debug Log", "Show debug log from Google Play Games plugin"));
            public static EMProperty gpgsXmlResources = new EMProperty(null, new GUIContent("Android XML Resources", "The XML resources exported from Google Play Console"));
            public static EMProperty autoInit = new EMProperty(null, new GUIContent("Auto Init", "Should the service automatically initialize on start"));
            public static EMProperty autoInitDelay = new EMProperty(null, new GUIContent("Auto Init Delay", "Delay time (seconds) after Start() that the service is automatically initialized"));
            public static EMProperty androidMaxLoginRequest = 
                new EMProperty(null, 
                    new GUIContent("[Android] Max Login Requests", 
                        "[Auto-init and ManagedInit only] The total number of times the login popup can be displayed if the user has not logged in. " +
                        "When this value is reached, the init process will stop thus not showing the login popup anymore (avoid annoying the user). " +
                        "Put -1 to ignore this limit."));
            public static EMProperty gpgsDataSource = new EMProperty(null, new GUIContent("[Android] Data Source", "The source from where saved game data should be fetched when using Google Play Games on Android"));
            public static EMProperty leaderboards = new EMProperty(null, new GUIContent("Leaderboards"));
            public static EMProperty achievements = new EMProperty(null, new GUIContent("Achievements"));

            // Saved Games is a PRO feature
            #if EASY_MOBILE_PRO
            public static EMProperty enableSavedGames = new EMProperty(null, new GUIContent("Enable Saved Games", "Enable the Saved Games service"));
            public static EMProperty autoConflictResolutionStrategy = new EMProperty(null, new GUIContent("Conflict Resolution Strategy", "The default strategy used for automatic resolution of saved game conflicts"));
            #endif
        }

        // Push Notification module properties
        private static class NotificationProperties
        {
            public static SerializedProperty mainProperty;
            public static EMProperty autoInit = new EMProperty(null, new GUIContent("Auto Init", "Should the service automatically initialize on start"));
            public static EMProperty autoInitDelay = new EMProperty(null, new GUIContent("Auto Init Delay", "Delay time (seconds) after Start() that the service is automatically initialized"));
            public static EMProperty iosAuthOptions = new EMProperty(null, new GUIContent("[iOS] Authorization Options", "The types of notification interaction your app requests on iOS"));
            public static EMProperty pushNotificationService = new EMProperty(null, new GUIContent("Push Notification Service", "The service used for push (remote) notifications"));
            public static EMProperty oneSignalAppId = new EMProperty(null, new GUIContent("OneSignal App Id", "The app Id obtained from OneSignal dashboard"));
            public static EMProperty categoryGroups = new EMProperty(null, new GUIContent("Category Groups", "Optional groups for notification categories"));
            public static EMProperty defaultCategory = new EMProperty(null, new GUIContent("Default Category", "The default notification category. Your app mush have at least one category, you can modify this default category but not remove it."));
            public static EMProperty userCategories = new EMProperty(null, new GUIContent("User Categories", "Custom notification categories for your app"));
        }

        // Utility module consists other sub-module properties
        // RatingRequestSettings properties
        private static class RatingRequestProperties
        {
            public static SerializedProperty mainProperty;
            public static EMProperty defaultRatingDialogContent = new EMProperty(null, new GUIContent("Default Dialog Content", "Content of the rating dialog used on Android and iOS older than 10.3"));
            public static EMProperty minimumAcceptedStars = new EMProperty(null, new GUIContent("Minimum Accepted Rating", "[Android only] The lowest rating/stars accepted. If fewer stars are given, we'll suggest the user to give feedback instead. Set this to 0 to disable the feedback feature"));
            public static EMProperty supportEmail = new EMProperty(null, new GUIContent("Support Email", "The email address to receive feedback"));
            public static EMProperty iosAppId = new EMProperty(null, new GUIContent("iOS App Id", "App Id on the Apple App Store"));
            public static EMProperty annualCap = new EMProperty(null, new GUIContent("Annual Cap", "Maximum number of requests allowed each year, note that on iOS 10.3+ this value is governed by the OS and is always set to 3"));
            public static EMProperty delayAfterInstallation = new EMProperty(null, new GUIContent("Delay After Installation", "Waiting time (in days) after app installation before the first rating request can be made"));
            public static EMProperty coolingOffPeriod = new EMProperty(null, new GUIContent("Cooling-Off Period", "The mininum interval required (in days) between two consecutive requests."));
            public static EMProperty ignoreConstraintsInDevelopment = new EMProperty(null, new GUIContent("Ignore Constraints In Development", "Ignore all display constraints so the rating popup can be shown every time in development builds, unless it was disabled before"));
        }

        #endregion

        #region Variables

        // List of Android leaderboard and achievement ids constructed from the generated GPGSIds class.
        private SortedDictionary<string, string> gpgsIdDict;

        #if EM_UIAP
        // Booleans indicating whether AppleTangle and GooglePlayTangle are not dummy classes.
        bool isAppleTangleValid;
        bool isGooglePlayTangleValid;
        #endif

        #endregion

        #region GUI

        void OnEnable()
        {
            // Module-control properties.
            isAdModuleEnable = serializedObject.FindProperty("_isAdModuleEnable");
            isIAPModuleEnable = serializedObject.FindProperty("_isIAPModuleEnable");
            isGameServiceModuleEnable = serializedObject.FindProperty("_isGameServiceModuleEnable");
            isNotificationModuleEnable = serializedObject.FindProperty("_isNotificationModuleEnable");

            activeModuleIndex = serializedObject.FindProperty("_activeModuleIndex");

            if (System.Enum.IsDefined(typeof(Module), activeModuleIndex.intValue))
            {
                activeModule = (Module)activeModuleIndex.intValue;
            }

            // Ad module properties.
            AdProperties.mainProperty = serializedObject.FindProperty("_advertisingSettings");
            AdProperties.iosAdColonyConfig.property = AdProperties.mainProperty.FindPropertyRelative("_iosAdColonyConfig");
            AdProperties.androidAdColonyConfig.property = AdProperties.mainProperty.FindPropertyRelative("_androidAdColonyConfig");
            AdProperties.adColonyAdOrientation.property = AdProperties.mainProperty.FindPropertyRelative("_adColonyAdOrientation");
            AdProperties.adColonyShowRewardedAdPrePopup.property = AdProperties.mainProperty.FindPropertyRelative("_adColonyShowRewardedAdPrePopup");
            AdProperties.adColonyShowRewardedAdPostPopup.property = AdProperties.mainProperty.FindPropertyRelative("_adColonyShowRewardedAdPostPopup");
            AdProperties.iosAdMobConfig.property = AdProperties.mainProperty.FindPropertyRelative("_iosAdMobConfig");
            AdProperties.androidAdMobConfig.property = AdProperties.mainProperty.FindPropertyRelative("_androidAdMobConfig");
            AdProperties.admobTargeting.property = AdProperties.mainProperty.FindPropertyRelative("_admobTargeting");
            AdProperties.admobEnableTestMode.property = AdProperties.mainProperty.FindPropertyRelative("_admobEnableTestMode");
            AdProperties.admobTestDeviceIds.property = AdProperties.mainProperty.FindPropertyRelative("_admobTestDeviceIds");
            AdProperties.heyzapPublisherId.property = AdProperties.mainProperty.FindPropertyRelative("_heyzapPublisherId");
            AdProperties.heyzapShowTestSuite.property = AdProperties.mainProperty.FindPropertyRelative("_heyzapShowTestSuite");
            AdProperties.autoLoadDefaultAds.property = AdProperties.mainProperty.FindPropertyRelative("_autoLoadDefaultAds");
            AdProperties.adCheckingInterval.property = AdProperties.mainProperty.FindPropertyRelative("_adCheckingInterval");
            AdProperties.adLoadingInterval.property = AdProperties.mainProperty.FindPropertyRelative("_adLoadingInterval");
            AdProperties.iosDefaultAdNetworks.property = AdProperties.mainProperty.FindPropertyRelative("_iosDefaultAdNetworks");
            AdProperties.androidDefaultAdNetworks.property = AdProperties.mainProperty.FindPropertyRelative("_androidDefaultAdNetwork");

            // In App Purchase module properties.
            IAPProperties.mainProperty = serializedObject.FindProperty("_inAppPurchaseSettings");
            IAPProperties.targetAndroidStore.property = IAPProperties.mainProperty.FindPropertyRelative("_targetAndroidStore");
            IAPProperties.validateAppleReceipt.property = IAPProperties.mainProperty.FindPropertyRelative("_validateAppleReceipt");
            IAPProperties.validateGooglePlayReceipt.property = IAPProperties.mainProperty.FindPropertyRelative("_validateGooglePlayReceipt");
            IAPProperties.products.property = IAPProperties.mainProperty.FindPropertyRelative("_products");

            // Game Service module properties.
            GameServiceProperties.mainProperty = serializedObject.FindProperty("_gameServiceSettings");
            GameServiceProperties.gpgsDebugLog.property = GameServiceProperties.mainProperty.FindPropertyRelative("_gpgsDebugLog");
            GameServiceProperties.gpgsXmlResources.property = GameServiceProperties.mainProperty.FindPropertyRelative("_androidXmlResources");
            GameServiceProperties.autoInit.property = GameServiceProperties.mainProperty.FindPropertyRelative("_autoInit");
            GameServiceProperties.autoInitDelay.property = GameServiceProperties.mainProperty.FindPropertyRelative("_autoInitDelay");
            GameServiceProperties.androidMaxLoginRequest.property = GameServiceProperties.mainProperty.FindPropertyRelative("_androidMaxLoginRequests");
            GameServiceProperties.gpgsDataSource.property = GameServiceProperties.mainProperty.FindPropertyRelative("_gpgsDataSource");
            GameServiceProperties.leaderboards.property = GameServiceProperties.mainProperty.FindPropertyRelative("_leaderboards");
            GameServiceProperties.achievements.property = GameServiceProperties.mainProperty.FindPropertyRelative("_achievements");

            #if EASY_MOBILE_PRO
            GameServiceProperties.enableSavedGames.property = GameServiceProperties.mainProperty.FindPropertyRelative("_enableSavedGames");
            GameServiceProperties.autoConflictResolutionStrategy.property = GameServiceProperties.mainProperty.FindPropertyRelative("_autoConflictResolutionStrategy");
            #endif

            // Notification module properties.
            NotificationProperties.mainProperty = serializedObject.FindProperty("_notificationSettings");
            NotificationProperties.autoInit.property = NotificationProperties.mainProperty.FindPropertyRelative("_autoInit");
            NotificationProperties.autoInitDelay.property = NotificationProperties.mainProperty.FindPropertyRelative("_autoInitDelay");
            NotificationProperties.iosAuthOptions.property = NotificationProperties.mainProperty.FindPropertyRelative("_iosAuthOptions");
            NotificationProperties.pushNotificationService.property = NotificationProperties.mainProperty.FindPropertyRelative("_pushNotificationService");
            NotificationProperties.oneSignalAppId.property = NotificationProperties.mainProperty.FindPropertyRelative("_oneSignalAppId");
            NotificationProperties.categoryGroups.property = NotificationProperties.mainProperty.FindPropertyRelative("_categoryGroups");
            NotificationProperties.defaultCategory.property = NotificationProperties.mainProperty.FindPropertyRelative("_defaultCategory");
            NotificationProperties.userCategories.property = NotificationProperties.mainProperty.FindPropertyRelative("_userCategories");

            // Utility module consists of other sub-module properties.
            // RatingRequest properties.
            RatingRequestProperties.mainProperty = serializedObject.FindProperty("_ratingRequestSettings");
            RatingRequestProperties.defaultRatingDialogContent.property = RatingRequestProperties.mainProperty.FindPropertyRelative("_defaultRatingDialogContent");
            RatingRequestProperties.minimumAcceptedStars.property = RatingRequestProperties.mainProperty.FindPropertyRelative("_minimumAcceptedStars");
            RatingRequestProperties.supportEmail.property = RatingRequestProperties.mainProperty.FindPropertyRelative("_supportEmail");
            RatingRequestProperties.iosAppId.property = RatingRequestProperties.mainProperty.FindPropertyRelative("_iosAppId");
            RatingRequestProperties.annualCap.property = RatingRequestProperties.mainProperty.FindPropertyRelative("_annualCap");
            RatingRequestProperties.delayAfterInstallation.property = RatingRequestProperties.mainProperty.FindPropertyRelative("_delayAfterInstallation");
            RatingRequestProperties.coolingOffPeriod.property = RatingRequestProperties.mainProperty.FindPropertyRelative("_coolingOffPeriod");
            RatingRequestProperties.ignoreConstraintsInDevelopment.property = RatingRequestProperties.mainProperty.FindPropertyRelative("_ignoreContraintsInDevelopment");

            // Get the sorted list of GPGS leaderboard and achievement ids.
            gpgsIdDict = new SortedDictionary<string, string>(EM_EditorUtil.GetGPGSIds());

            #if EM_UIAP
            // Determine if AppleTangle and GooglePlayTangle classes are valid ones (generated by UnityIAP's receipt validation obfuscator).
            isAppleTangleValid = EM_EditorUtil.IsValidAppleTangleClass();
            isGooglePlayTangleValid = EM_EditorUtil.IsValidGooglePlayTangleClass();
            #endif
        }

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            // Draw the module-select toolbar.
            EditorGUILayout.BeginHorizontal();
            EM_EditorGUI.ToolbarButton(new GUIContent(null, EM_GUIStyleManager.AdIcon, "Advertising"), Module.Advertising, ref activeModule, EditorGUIUtility.isProSkin ? EM_GUIStyleManager.ModuleToolbarButtonLeft : EM_GUIStyleManager.ModuleToolbarButton);
            EM_EditorGUI.ToolbarButton(new GUIContent(null, EM_GUIStyleManager.IAPIcon, "In-App Purchasing"), Module.InAppPurchase, ref activeModule, EditorGUIUtility.isProSkin ? EM_GUIStyleManager.ModuleToolbarButtonMid : EM_GUIStyleManager.ModuleToolbarButton);
            EM_EditorGUI.ToolbarButton(new GUIContent(null, EM_GUIStyleManager.GameServiceIcon, "Game Services"), Module.GameService, ref activeModule, EditorGUIUtility.isProSkin ? EM_GUIStyleManager.ModuleToolbarButtonMid : EM_GUIStyleManager.ModuleToolbarButton);
            EM_EditorGUI.ToolbarButton(new GUIContent(null, EM_GUIStyleManager.NotificationIcon, "Notifications"), Module.Notification, ref activeModule, EditorGUIUtility.isProSkin ? EM_GUIStyleManager.ModuleToolbarButtonMid : EM_GUIStyleManager.ModuleToolbarButton);
            EM_EditorGUI.ToolbarButton(new GUIContent(null, EM_GUIStyleManager.UtilityIcon, "Utilities"), Module.Utility, ref activeModule, EditorGUIUtility.isProSkin ? EM_GUIStyleManager.ModuleToolbarButtonRight : EM_GUIStyleManager.ModuleToolbarButton);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Store the toolbar index value to the serialized settings file.
            activeModuleIndex.intValue = (int)activeModule;

            switch (activeModule)
            {
                case Module.Advertising:
                    AdModuleGUI();
                    break;
                case Module.InAppPurchase:
                    IAPModuleGUI();
                    break;
                case Module.GameService:
                    GameServiceModuleGUI();
                    break;
                case Module.Notification:
                    NotificationModuleGUI();
                    break;
                case Module.Utility:
                    UtilityModuleGUI();
                    break;
            }

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}
