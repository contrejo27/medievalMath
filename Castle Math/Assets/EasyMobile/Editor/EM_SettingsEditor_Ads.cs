using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using EasyMobile;

namespace EasyMobile.Editor
{
    // Partial editor class for Advertising module.
    public partial class EM_SettingsEditor
    {
        const string AdModuleLabel = "ADVERTISING";
        const string AdModuleIntro = "Advertising module provides easy-to-use API, auto ad-loading feature and supports a wide range of ad networks including AdMob, Chartboost, Heyzap and UnityAds.";
        const string AdColonyImportInstruction = "AdColony plugin not found. Please download and import it to show ads from AdColony.";
        const string AdColonyAvailMsg = "AdColony plugin was imported.";
        const string AdMobImportInstruction = "Google Mobile Ads (AdMob) plugin not found. Please download and import it to show ads from AdMob.";
        const string AdMobAvailMsg = "Google Mobile Ads (AdMob) plugin was imported.";
        const string ChartboostImportInstruction = "Chartboost plugin not found. Please download and import it to show ads from Chartboost.";
        const string ChartboostAvailMsg = "Chartboost plugin was imported.";
        const string HeyzapImportInstruction = "Heyzap plugin not found. Please download and import it to show ads from Heyzap.";
        const string HeyzapAvailMsg = "Heyzap plugin was imported.";
        const string UnityAdsUnvailableWarning = "Unity Ads service is disabled or not available for the current platform. To enable it go to Window > Services.";
        const string UnityAdsAvailableMsg = "Unity Ads service is enabled.";

        void AdModuleGUI()
        {            
            EditorGUILayout.BeginVertical(EM_GUIStyleManager.GetCustomStyle("Module Box"));

            EditorGUI.BeginChangeCheck();
            isAdModuleEnable.boolValue = EM_EditorGUI.ModuleToggle(isAdModuleEnable.boolValue, AdModuleLabel);
            if (EditorGUI.EndChangeCheck())
            {
                GameObject prefab = EM_EditorUtil.GetMainPrefab();

                if (!isAdModuleEnable.boolValue)
                {
                    EM_PluginManager.DisableAdModule(prefab);
                }
                else
                {
                    EM_PluginManager.EnableAdModule(prefab);
                }
            }

            // Now draw the GUI.
            if (!isAdModuleEnable.boolValue)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(AdModuleIntro, MessageType.Info);
            }
            else
            {
                // AdColony setup
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("ADCOLONY SETUP", EditorStyles.boldLabel);

                #if !EM_ADCOLONY
                EditorGUILayout.HelpBox(AdColonyImportInstruction, MessageType.Warning);
                EditorGUILayout.Space();
                if (GUILayout.Button("Download AdColony Plugin", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                {
                    EM_ExternalPluginManager.DownloadAdColonyPlugin();
                }
                #else
                EditorGUILayout.HelpBox(AdColonyAvailMsg, MessageType.Info);
                EditorGUILayout.Space();
                if (GUILayout.Button("Download AdColony Plugin", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                {
                    EM_ExternalPluginManager.DownloadAdColonyPlugin();
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("AdColony IDs", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(AdProperties.iosAdColonyConfig.property, AdProperties.iosAdColonyConfig.content, true);
                EditorGUILayout.PropertyField(AdProperties.androidAdColonyConfig.property, AdProperties.androidAdColonyConfig.content, true);
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Ad Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(AdProperties.adColonyShowRewardedAdPrePopup.property, AdProperties.adColonyShowRewardedAdPrePopup.content);
                EditorGUILayout.PropertyField(AdProperties.adColonyShowRewardedAdPostPopup.property, AdProperties.adColonyShowRewardedAdPostPopup.content);
                EditorGUILayout.PropertyField(AdProperties.adColonyAdOrientation.property, AdProperties.adColonyAdOrientation.content);
                #endif

                // AdMob setup
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("ADMOB SETUP", EditorStyles.boldLabel);

                #if !EM_ADMOB
                EditorGUILayout.HelpBox(AdMobImportInstruction, MessageType.Warning);
                EditorGUILayout.Space();
                if (GUILayout.Button("Download Google Mobile Ads Plugin", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                {
                    EM_ExternalPluginManager.DownloadGoogleMobileAdsPlugin();
                }
                #else
                EditorGUILayout.HelpBox(AdMobAvailMsg, MessageType.Info);
                EditorGUILayout.Space();
                if (GUILayout.Button("Download Google Mobile Ads Plugin", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                {
                    EM_ExternalPluginManager.DownloadGoogleMobileAdsPlugin();
                }

                // IDs.
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("AdMob IDs", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(AdProperties.iosAdMobConfig.property, AdProperties.iosAdMobConfig.content, true);
                EditorGUILayout.PropertyField(AdProperties.androidAdMobConfig.property, AdProperties.androidAdMobConfig.content, true);
                EditorGUI.indentLevel--;

                // Ad targeting settings.
                EditorGUILayout.Space();
                DrawAdMobTargetingSettings(AdProperties.admobTargeting.property, AdProperties.admobTargeting.content);

                // Test mode.
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Test Mode", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(AdProperties.admobEnableTestMode.property, AdProperties.admobEnableTestMode.content);
                if (AdProperties.admobEnableTestMode.property.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(AdProperties.admobTestDeviceIds.property, AdProperties.admobTestDeviceIds.content, true);
                    EditorGUI.indentLevel--;
                }
                else
                {
                    // Clear test device IDs when test mode is disable for safety.
                    AdProperties.admobTestDeviceIds.property.ClearArray();
                }
                #endif

                // Chartboost setup
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("CHARTBOOST SETUP", EditorStyles.boldLabel);

                #if !EM_CHARTBOOST
                EditorGUILayout.HelpBox(ChartboostImportInstruction, MessageType.Warning);
                EditorGUILayout.Space();
                if (GUILayout.Button("Download Chartboost Plugin", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                {
                    EM_ExternalPluginManager.DownloadChartboostPlugin();
                }
                #else
                EditorGUILayout.HelpBox(ChartboostAvailMsg, MessageType.Info);
                EditorGUILayout.Space();
                if (GUILayout.Button("Download Chartboost Plugin", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                {
                    EM_ExternalPluginManager.DownloadChartboostPlugin();
                }
                if (GUILayout.Button("Setup Chartboost", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                {
                    // Open Chartboost settings window.
                    ChartboostSDK.CBSettings.Edit();  
                }
                #endif

                // Heyzap setup
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("HEYZAP SETUP", EditorStyles.boldLabel);

                #if !EM_HEYZAP
                EditorGUILayout.HelpBox(HeyzapImportInstruction, MessageType.Warning);
                EditorGUILayout.Space();
                if (GUILayout.Button("Download Heyzap Plugin", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                {
                    EM_ExternalPluginManager.DownloadHeyzapPlugin();
                }
                #else
                EditorGUILayout.HelpBox(HeyzapAvailMsg, MessageType.Info);
                EditorGUILayout.Space();
                if (GUILayout.Button("Download Heyzap Plugin", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                {
                    EM_ExternalPluginManager.DownloadHeyzapPlugin();
                }
                EditorGUILayout.Space();
                AdProperties.heyzapPublisherId.property.stringValue = EditorGUILayout.TextField(AdProperties.heyzapPublisherId.content, AdProperties.heyzapPublisherId.property.stringValue);
                AdProperties.heyzapShowTestSuite.property.boolValue = EditorGUILayout.Toggle(AdProperties.heyzapShowTestSuite.content, AdProperties.heyzapShowTestSuite.property.boolValue);
                #endif

                // UnityAds setup
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("UNITY ADS SETUP", EditorStyles.boldLabel);

                #if !UNITY_ADS
                EditorGUILayout.HelpBox(UnityAdsUnvailableWarning, MessageType.Warning);
                #else
                EditorGUILayout.HelpBox(UnityAdsAvailableMsg, MessageType.Info);
                #endif

                // Ads auto-load setup
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("AUTO AD-LOADING CONFIG", EditorStyles.boldLabel);
                AdProperties.autoLoadDefaultAds.property.boolValue = EditorGUILayout.Toggle(AdProperties.autoLoadDefaultAds.content, AdProperties.autoLoadDefaultAds.property.boolValue);
                AdProperties.adCheckingInterval.property.floatValue = EditorGUILayout.FloatField(AdProperties.adCheckingInterval.content, AdProperties.adCheckingInterval.property.floatValue);
                AdProperties.adLoadingInterval.property.floatValue = EditorGUILayout.FloatField(AdProperties.adLoadingInterval.content, AdProperties.adLoadingInterval.property.floatValue);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("DEFAULT AD NETWORKS", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(AdProperties.iosDefaultAdNetworks.property, AdProperties.iosDefaultAdNetworks.content, true);
                EditorGUILayout.PropertyField(AdProperties.androidDefaultAdNetworks.property, AdProperties.androidDefaultAdNetworks.content, true);
                EditorGUI.indentLevel--;

                // Now check if there's any default ad network that doesn't have plugin imported and show warnings.
                AdSettings.DefaultAdNetworks iosDefault = EM_Settings.Advertising.IosDefaultAdNetworks;
                AdSettings.DefaultAdNetworks androidDefault = EM_Settings.Advertising.AndroidDefaultAdNetworks;
                List<AdNetwork> usedNetworks = new List<AdNetwork>();
                AddWithoutRepeat(usedNetworks, (AdNetwork)iosDefault.bannerAdNetwork);
                AddWithoutRepeat(usedNetworks, (AdNetwork)iosDefault.interstitialAdNetwork);
                AddWithoutRepeat(usedNetworks, (AdNetwork)iosDefault.rewardedAdNetwork);
                AddWithoutRepeat(usedNetworks, (AdNetwork)androidDefault.bannerAdNetwork);
                AddWithoutRepeat(usedNetworks, (AdNetwork)androidDefault.interstitialAdNetwork);
                AddWithoutRepeat(usedNetworks, (AdNetwork)androidDefault.rewardedAdNetwork);

                bool addedSpace = false;

                foreach (AdNetwork network in usedNetworks)
                {
                    if (!IsPluginAvail(network))
                    {
                        if (!addedSpace)
                        {
                            EditorGUILayout.Space();
                            addedSpace = true;
                        }
                        EditorGUILayout.HelpBox("Default ad network " + network.ToString() + " has no SDK. Please import its plugin.", MessageType.Warning);
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }

        void AddWithoutRepeat<T>(List<T> list, T element)
        {
            if (!list.Contains(element))
            {
                list.Add(element);
            }
        }

        bool IsPluginAvail(AdNetwork adNetwork)
        {
            switch (adNetwork)
            {
                case AdNetwork.AdColony:
                    #if EM_ADCOLONY
                    return true;
                    #else
                    return false;
                    #endif
                case AdNetwork.AdMob:
                    #if EM_ADMOB
                    return true;
                    #else
                    return false;
                    #endif
                case AdNetwork.Chartboost:
                    #if EM_CHARTBOOST
                    return true;
                    #else
                    return false;
                    #endif
                case AdNetwork.Heyzap:
                    #if EM_HEYZAP
                    return true;
                    #else
                    return false;
                    #endif
                case AdNetwork.UnityAds:
                    #if UNITY_ADS
                    return true;
                    #else
                    return false;
                    #endif
                case AdNetwork.None:
                    return true;
                default:
                    return false;
            }
        }

        #if EM_ADMOB
        void DrawAdMobTargetingSettings(SerializedProperty property, GUIContent label)
        {
            var gender = property.FindPropertyRelative("gender");
            var childDirected = property.FindPropertyRelative("tagForChildDirectedTreatment");
            var extras = property.FindPropertyRelative("extras");

            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(gender);
            EditorGUILayout.PropertyField(childDirected);
            DrawAdMobTargetingSettingsExtras(extras);
        }

        void DrawAdMobTargetingSettingsExtras(SerializedProperty extras)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(extras.displayName);
            extras.arraySize = EditorGUILayout.IntField(extras.arraySize, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
            for (int i = 0; i < extras.arraySize; i++)
            {
                EditorGUILayout.PropertyField(extras.GetArrayElementAtIndex(i));
            }
            EditorGUI.indentLevel--;
        }

        #endif
    }
}

