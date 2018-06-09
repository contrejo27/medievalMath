using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EasyMobile.Editor
{
    public static class EM_ExternalPluginManager
    {
        // AdColony
        public const string AdColonyNameSpace = "AdColony";

        // AdMob
        public const string GoogleMobileAdsNameSpace = "GoogleMobileAds";

        // Chartboost
        public const string ChartboostNameSpace = "ChartboostSDK";
        public const string ChartboostClassName = "Chartboost";

        // Heyzap
        public const string HeyzapNameSpace = "Heyzap";

        // UnityIAP
        public const string UnityPurchasingAssemblyName = "UnityEngine.Purchasing";
        public const string UnityPurchasingNameSpace = "UnityEngine.Purchasing";
        public const string UnityPurchasingSecurityNameSpace = "UnityEngine.Purchasing.Security";
        public const string UnityPurchasingClassName = "UnityPurchasing";

        // Google Play Games
        public const string GPGSNameSpace = "GooglePlayGames";
        public const string GPGSClassName = "PlayGamesPlatform";

        // OneSignal
        public const string OneSignalClassName = "OneSignal";

        // PlayMaker Unity UI add-on
        public const string PlayMakerUguiAddOnClass = "PlayMakerUGuiSceneProxy";

        // 3rd party plugin download URLs
        public const string AdColonyDownloadURL = "https://github.com/AdColony/AdColony-Unity-SDK-3";
        public const string ChartboostDownloadURL = "https://answers.chartboost.com/en-us/articles/download";
        public const string HeyzapDownloadURL = "https://developers.heyzap.com/docs/unity_sdk_setup_and_requirements";
        public const string GoogleMobileAdsDownloadURL = "https://github.com/googleads/googleads-mobile-unity/releases";
        public const string GooglePlayGamesDownloadURL = "https://github.com/playgameservices/play-games-plugin-for-unity";
        public const string OneSignalDownloadURL = "https://github.com/OneSignal/OneSignal-Unity-SDK";

        /// <summary>
        /// Determines if AdColony plugin is available.
        /// </summary>
        /// <returns><c>true</c> if AdColony plugin available; otherwise, <c>false</c>.</returns>
        public static bool IsAdColonyAvail()
        {
            return EM_EditorUtil.NamespaceExists(AdColonyNameSpace);
        }

        /// <summary>
        /// Determines if AdMob plugin is available.
        /// </summary>
        /// <returns><c>true</c> if AdMob plugin available; otherwise, <c>false</c>.</returns>
        public static bool IsAdMobAvail()
        {
            return EM_EditorUtil.NamespaceExists(GoogleMobileAdsNameSpace);
        }

        /// <summary>
        /// Determines if Chartboost plugin is available.
        /// </summary>
        /// <returns><c>true</c> if Chartboost plugin available; otherwise, <c>false</c>.</returns>
        public static bool IsChartboostAvail()
        {
            System.Type chartboost = EM_EditorUtil.FindClass(ChartboostClassName, ChartboostNameSpace);

            return chartboost != null;
        }

        /// <summary>
        /// Determines if Heyzap plugin is available.
        /// </summary>
        /// <returns><c>true</c> if Heyzap plugin available; otherwise, <c>false</c>.</returns>
        public static bool IsHeyzapAvail()
        {
            return EM_EditorUtil.NamespaceExists(HeyzapNameSpace);
        }

        /// <summary>
        /// Determines if UnityIAP is enabled.
        /// </summary>
        /// <returns><c>true</c> if enabled; otherwise, <c>false</c>.</returns>
        public static bool IsUnityIAPAvail()
        {
            // Here we check for the existence of the Security namespace instead of UnityPurchasing class in order to
            // make sure that the plugin is actually imported (rather than the service just being enabled).
            return EM_EditorUtil.NamespaceExists(UnityPurchasingSecurityNameSpace);
        }

        /// <summary>
        /// Determines if GooglePlayGames plugin is available.
        /// </summary>
        /// <returns><c>true</c> if is GPGS avail; otherwise, <c>false</c>.</returns>
        public static bool IsGPGSAvail()
        {
            System.Type gpgs = EM_EditorUtil.FindClass(GPGSClassName, GPGSNameSpace);

            return gpgs != null;
        }

        /// <summary>
        /// Determines if OneSignal plugin is available.
        /// </summary>
        /// <returns><c>true</c> if is one signal avail; otherwise, <c>false</c>.</returns>
        public static bool IsOneSignalAvail()
        {
            System.Type oneSignal = EM_EditorUtil.FindClass(OneSignalClassName);

            return oneSignal != null;
        }

        /// <summary>
        /// Determines if PlayMaker is installed.
        /// </summary>
        /// <returns><c>true</c> if is play maker avail; otherwise, <c>false</c>.</returns>
        public static bool IsPlayMakerAvail()
        {
            #if PLAYMAKER
            return true;
            #else
            return false;
            #endif
        }

        public static bool IsPlayMakerUguiAddOnAvail()
        {
            System.Type uGui = EM_EditorUtil.FindClass(PlayMakerUguiAddOnClass);

            return uGui != null;
        }

        public static void DownloadGoogleMobileAdsPlugin()
        {
            Application.OpenURL(GoogleMobileAdsDownloadURL);
        }

        public static void DownloadGooglePlayGamesPlugin()
        {
            Application.OpenURL(GooglePlayGamesDownloadURL);
        }

        public static void DownloadOneSignalPlugin()
        {
            Application.OpenURL(OneSignalDownloadURL);
        }

        public static void DownloadChartboostPlugin()
        {
            Application.OpenURL(ChartboostDownloadURL);
        }

        public static void DownloadHeyzapPlugin()
        {
            Application.OpenURL(HeyzapDownloadURL);
        }

        public static void DownloadAdColonyPlugin()
        {
            Application.OpenURL(AdColonyDownloadURL);
        }
    }
}

