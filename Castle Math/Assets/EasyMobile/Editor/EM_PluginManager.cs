using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using SgLib.Editor;

namespace EasyMobile.Editor
{
    [InitializeOnLoad]
    public class EM_PluginManager : AssetPostprocessor
    {
        #region Init

        // This static constructor will automatically run thanks to the InitializeOnLoad attribute.
        static EM_PluginManager()
        {
            EditorApplication.update += Initialize;
        }

        private static void Initialize()
        {
            EditorApplication.update -= Initialize;

            // Check if a new version has been imported and perform necessary updating jobs.
            VersionCheck();

            // Define a global symbol indicating the existence of EasyMobile
            GlobalDefineManager.SDS_AddDefines(EM_ScriptingSymbols.EasyMobile.Split(';'), EditorUserBuildSettings.selectedBuildTargetGroup);

            // Create the EM_Settings scriptable object if it doesn't exist.
            EM_BuiltinObjectCreator.CreateEMSettingsAsset();

            // Create the EasyMobile prefab if it doesn't exist.
            EM_BuiltinObjectCreator.CreateEasyMobilePrefab();

            // Regularly check for module prerequisites to avoid issues caused
            // by inadvertent changes, e.g remove components from prefab or delete scripting symbol.
            CheckModules();
        }

        #endregion

        #region Methods

        // Check if a *different* (maybe an older one is being imported!) version has been imported.
        // If yes, import the native package and update the version keys stored in settings file.
        internal static void VersionCheck()
        {
            int savedVersion = EM_ProjectSettings.Instance.GetInt(EM_Constants.PSK_EMVersionInt, -1);

            if (savedVersion != EM_Constants.versionInt)
            {
                // New version detected!
                EM_ProjectSettings.Instance.Set(EM_Constants.PSK_EMVersionString, EM_Constants.versionString);
                EM_ProjectSettings.Instance.Set(EM_Constants.PSK_EMVersionInt, EM_Constants.versionInt);

                // Import the Google Play Services Resolver
                ImportPlayServicesResolver(false);
            }
            else if (!IsPlayServicesResolverImported())
            {
                ImportPlayServicesResolver(false);
            }
        }

        internal static bool IsPlayServicesResolverImported()
        {
            return EM_ProjectSettings.Instance.GetBool(EM_Constants.PSK_ImportedPlayServicesResolver, false);
        }

        internal static void ImportPlayServicesResolver(bool interactive)
        {
            AssetDatabase.ImportPackage(EM_Constants.PlayServicersResolverPackagePath, interactive);
            EM_ProjectSettings.Instance.Set(EM_Constants.PSK_ImportedPlayServicesResolver, true);
        }

        internal static void InstallPlayMakerActions(bool interactive)
        {
            if (!EM_ExternalPluginManager.IsPlayMakerAvail())
            {
                if (EM_EditorUtil.DisplayDialog(
                        "Installing PlayMaker Actions",
                        "Looks like you haven't installed PlayMaker, please install it to use these actions. " +
                        "Note that you also need to install the Unity UI add-on for PlayMaker to run Easy Mobile's PlayMaker demo.",
                        "Continue Anyway",
                        "Cancel"))
                {
                    DoInstallPlayMakerActions(interactive);
                }                   
            }
            else
            {
                if (!EM_ExternalPluginManager.IsPlayMakerUguiAddOnAvail())
                {
                    if (EM_EditorUtil.DisplayDialog(
                            "Installing PlayMaker Actions",
                            "Looks like you haven't installed the Unity UI add-on for PlayMaker. " +
                            "Please install it if you want to run Easy Mobile's PlayMaker demo.",
                            "Continue Anyway",
                            "Cancel"))
                    {
                        DoInstallPlayMakerActions(interactive);
                    }
                }
                else
                {
                    DoInstallPlayMakerActions(interactive);
                }
            }
        }

        private static void DoInstallPlayMakerActions(bool interactive)
        {
            AssetDatabase.ImportPackage(EM_Constants.PlayMakerActionsPackagePath, interactive);
        }

        // Makes that everything is set up properly so that all modules function as expected.
        internal static void CheckModules()
        {
            GameObject mainPrefab = EM_EditorUtil.GetMainPrefab();

            // Advertising module.
            if (EM_Settings.IsAdModuleEnable)
            {
                EnableAdModule(mainPrefab);
            }
            else
            {
                DisableAdModule(mainPrefab);
            }

            // IAP module.
            if (EM_Settings.IsIAPModuleEnable)
            {
                EnableIAPModule(mainPrefab);
            }
            else
            {
                DisableIAPModule(mainPrefab);
            }

            // Game Service module.
            if (EM_Settings.IsGameServicesModuleEnable)
            {
                EnableGameServiceModule(mainPrefab);
            }
            else
            {
                DisableGameServiceModule(mainPrefab);
            }

            // Notification module
            if (EM_Settings.IsNotificationsModuleEnable)
            {
                EnableNotificationModule(mainPrefab);
            }
            else
            {
                DisableNotificationModule(mainPrefab);
            }
        }

        internal static void EnableAdModule(GameObject mainPrefab)
        {
            EM_EditorUtil.AddModuleToPrefab<Advertising>(mainPrefab);

            // Check ad network plugins' availability and define appropriate scripting symbols.
            // Note that UnityAds symbol is added automatically by Unity engine.
            List<string> symbols = new List<string>();

            // AdColony
            bool isAdColonyAvail = EM_ExternalPluginManager.IsAdColonyAvail();
            if (isAdColonyAvail)
            {
                symbols.Add(EM_ScriptingSymbols.AdColony);
            }

            // AdMob
            bool isAdMobAvail = EM_ExternalPluginManager.IsAdMobAvail();
            if (isAdMobAvail)
            {
                symbols.Add(EM_ScriptingSymbols.AdMob);
            }

            // Chartboost
            bool isChartboostAvail = EM_ExternalPluginManager.IsChartboostAvail();
            if (isChartboostAvail)
            {
                symbols.Add(EM_ScriptingSymbols.Chartboost);
            }

            // Heyzap
            bool isHeyzapAvail = EM_ExternalPluginManager.IsHeyzapAvail();
            if (isHeyzapAvail)
            {
                symbols.Add(EM_ScriptingSymbols.Heyzap);
            }

            GlobalDefineManager.SDS_AddDefines(symbols.ToArray(), EditorUserBuildSettings.selectedBuildTargetGroup);
        }

        internal static void DisableAdModule(GameObject mainPrefab)
        {
            EM_EditorUtil.RemoveModuleFromPrefab<Advertising>(mainPrefab);

            // Remove associated scripting symbols on all platforms if any was defined on that platform.
            GlobalDefineManager.SDS_RemoveDefinesOnAllPlatforms(
                new string[] { EM_ScriptingSymbols.AdColony, EM_ScriptingSymbols.AdMob, EM_ScriptingSymbols.Chartboost, EM_ScriptingSymbols.Heyzap }
            );
        }

        internal static void EnableIAPModule(GameObject mainPrefab)
        {
            EM_EditorUtil.AddModuleToPrefab<InAppPurchasing>(mainPrefab);

            // Check if UnityIAP is enable and act accordingly.
            bool isUnityIAPAvail = EM_ExternalPluginManager.IsUnityIAPAvail();
            if (isUnityIAPAvail)
            {
                // Generate dummy AppleTangle and GoogleTangle classes if they don't exist.
                // Note that AppleTangle and GooglePlayTangle only get compiled on following platforms,
                // therefore the compilational condition is needed, otherwise the code will repeat forever.
                #if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
                if (!EM_EditorUtil.AppleTangleClassExists())
                {
                    EM_EditorUtil.GenerateDummyAppleTangleClass();
                }

                if (!EM_EditorUtil.GooglePlayTangleClassExists())
                {
                    EM_EditorUtil.GenerateDummyGooglePlayTangleClass();
                }
                #endif

                GlobalDefineManager.SDS_AddDefine(EM_ScriptingSymbols.UnityIAP, EditorUserBuildSettings.selectedBuildTargetGroup);
            }
        }

        internal static void DisableIAPModule(GameObject mainPrefab)
        { 
            EM_EditorUtil.RemoveModuleFromPrefab<InAppPurchasing>(mainPrefab);

            // Remove associated scripting symbol on all platforms it was defined.
            GlobalDefineManager.SDS_RemoveDefineOnAllPlatforms(EM_ScriptingSymbols.UnityIAP);
        }

        internal static void EnableGameServiceModule(GameObject mainPrefab)
        {
            EM_EditorUtil.AddModuleToPrefab<GameServices>(mainPrefab);

            // Check if Google Play Games plugin is available.
            bool isGPGSAvail = EM_ExternalPluginManager.IsGPGSAvail();
            if (isGPGSAvail)
            {
                // We won't use Google Play Game Services on iOS, so we'll define NO_GPGS symbol to disable it.
                GlobalDefineManager.SDS_AddDefine(EM_ScriptingSymbols.NoGooglePlayGames, BuildTargetGroup.iOS);

                // Define EM_GPGS symbol on Android platform
                GlobalDefineManager.SDS_AddDefine(EM_ScriptingSymbols.GooglePlayGames, BuildTargetGroup.Android);
            }
        }

        internal static void DisableGameServiceModule(GameObject mainPrefab)
        {
            EM_EditorUtil.RemoveModuleFromPrefab<GameServices>(mainPrefab);

            // Removed associated scripting symbols if any was defined.
            // Note that we won't remove the NO_GPGS symbol automatically on iOS.
            // Rather we'll let the user delete it manually if they want.
            // This helps prevent potential build issues on iOS due to GPGS dependencies.
            GlobalDefineManager.SDS_RemoveDefineOnAllPlatforms(EM_ScriptingSymbols.GooglePlayGames);
        }

        internal static void EnableNotificationModule(GameObject mainPrefab)
        {
            EM_EditorUtil.AddModuleToPrefab<Notifications>(mainPrefab);

            // Check if OneSignal is available.
            bool isOneSignalAvail = EM_ExternalPluginManager.IsOneSignalAvail();
            if (isOneSignalAvail)
            {
                GlobalDefineManager.SDS_AddDefine(EM_ScriptingSymbols.OneSignal, EditorUserBuildSettings.selectedBuildTargetGroup);
            }
        }

        internal static void DisableNotificationModule(GameObject mainPrefab)
        {
            EM_EditorUtil.RemoveModuleFromPrefab<Notifications>(mainPrefab);

            // Remove associated scripting symbol on all platforms it was defined.
            GlobalDefineManager.SDS_RemoveDefineOnAllPlatforms(EM_ScriptingSymbols.OneSignal);
        }

        #endregion
    }
}
