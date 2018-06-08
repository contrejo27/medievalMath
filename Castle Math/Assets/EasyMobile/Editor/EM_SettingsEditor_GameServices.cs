using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID && EM_GPGS
using System;
using System.Reflection;
using GooglePlayGames.Editor;
#endif

namespace EasyMobile.Editor
{
    // Partial editor class for GameService module.
    public partial class EM_SettingsEditor
    {
        const string GameServiceModuleLabel = "GAME SERVICES";
        const string GameServiceModuleIntro = "Game Services module helps you quickly implement services like leaderboards and achievements for your game. It provides a cross-platform API that works with the Game Center network on iOS and Google Play Games services on Android.";
        const string GameServiceManualInitInstruction = "You can initialize manually from script by calling GameServices.ManagedInit() or GameServices.Init() method.";
        const string AndroidGPGSImportInstruction = "Google Play Games plugin is required. Please download and import it to use this module on Android.";
        const string AndroidGPGSAvailMsg = "Google Play Games plugin is imported and ready to use.";
        const string AndroidGPGPSSetupInstruction = "Paste in the Android XML Resources from the Play Console and hit the Setup button.";
        const string GameServiceConstantGenerationIntro = "Generate the static class " + EM_Constants.RootNameSpace + "." + EM_Constants.GameServicesConstantsClassName + " that contains the constants of leaderboard and achievement names." +
                                                          " Remember to regenerate if you make changes to these names.";

        // GameServiceItem property names.
        const string GameServiceItem_NameProperty = "_name";
        const string GameServiceItem_IOSIdProperty = "_iosId";
        const string GameServiceItem_AndroidIdProperty = "_androidId";

        #if !UNITY_ANDROID || (UNITY_ANDROID && EM_GPGS)
        // GPGS Web client ID.
        static string sGPGSWebClientId;

        // Foldout bools.
        static bool isLeadeboardsFoldout = false;
        static bool isAchievementsFoldout = false;
        #endif

        // GPGS generated IDs.
        static string[] gpgsIds;

        // Android resources text area scroll position.
        Vector2 androidResourcesTextAreaScroll;

        void GameServiceModuleGUI()
        {
            EditorGUILayout.BeginVertical(EM_GUIStyleManager.GetCustomStyle("Module Box"));

            EditorGUI.BeginChangeCheck();

            isGameServiceModuleEnable.boolValue = EM_EditorGUI.ModuleToggle(isGameServiceModuleEnable.boolValue, GameServiceModuleLabel);

            // Update the main prefab according to the toggle state.
            if (EditorGUI.EndChangeCheck())
            {
                GameObject prefab = EM_EditorUtil.GetMainPrefab();

                if (!isGameServiceModuleEnable.boolValue)
                {                 
                    EM_PluginManager.DisableGameServiceModule(prefab);
                }
                else
                { 
                    EM_PluginManager.EnableGameServiceModule(prefab);
                }
            }

            // Now draw the GUI.
            if (!isGameServiceModuleEnable.boolValue)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(GameServiceModuleIntro, MessageType.Info);
            }
            else
            {
                #if UNITY_ANDROID && !EM_GPGS
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(AndroidGPGSImportInstruction, MessageType.Error);
                EditorGUILayout.Space();
                if (GUILayout.Button("Download Google Play Games Plugin", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                {
                    EM_ExternalPluginManager.DownloadGooglePlayGamesPlugin();
                }
                #elif UNITY_ANDROID && EM_GPGS
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(AndroidGPGSAvailMsg, MessageType.Info);
                EditorGUILayout.Space();
                if (GUILayout.Button("Download Google Play Games Plugin", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                {
                    EM_ExternalPluginManager.DownloadGooglePlayGamesPlugin();
                }

                // Android Google Play Games setup
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("GOOGLE PLAY GAMES SETUP", EditorStyles.boldLabel);

                // GPGPS debug log
                GameServiceProperties.gpgsDebugLog.property.boolValue = EditorGUILayout.Toggle(GameServiceProperties.gpgsDebugLog.content, GameServiceProperties.gpgsDebugLog.property.boolValue);

                // GPGS (optional) Web App Client ID.
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Web App Client ID (Optional)", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("The web app client ID is needed to access the user's ID token and call other APIs on behalf of the user. It is not required for Game Services. " +
                    "Enter your oauth2 client ID below. To obtain this ID, generate a web linked app in Developer Console.\n" +
                    "Example: 123456789012-abcdefghijklm.apps.googleusercontent.com", MessageType.None);
                sGPGSWebClientId = EditorGUILayout.TextField("Web Client Id", sGPGSWebClientId);

                // Text area to input the Android Xml resource.
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(GameServiceProperties.gpgsXmlResources.content, EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(AndroidGPGPSSetupInstruction, MessageType.None);

                // Draw text area inside a scroll view.
                androidResourcesTextAreaScroll = GUILayout.BeginScrollView(androidResourcesTextAreaScroll, false, false, GUILayout.Height(EditorGUIUtility.singleLineHeight * 10));
                GameServiceProperties.gpgsXmlResources.property.stringValue = EditorGUILayout.TextArea(
                    GameServiceProperties.gpgsXmlResources.property.stringValue, 
                    GUILayout.Height(EditorGUIUtility.singleLineHeight * 100),
                    GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space();

                // Replicate the "Setup" button within the Android GPGS setup window.
                if (GUILayout.Button("Setup Google Play Games", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                { 
                    EditorApplication.delayCall += SetupAndroidGPGSButtonHandler;
                }
                #endif

                #if !UNITY_ANDROID || (UNITY_ANDROID && EM_GPGS)
                // Auto-init config
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("AUTO-INIT CONFIG", EditorStyles.boldLabel);
                GameServiceProperties.autoInit.property.boolValue = EditorGUILayout.Toggle(GameServiceProperties.autoInit.content, GameServiceProperties.autoInit.property.boolValue);

                EditorGUI.BeginDisabledGroup(!GameServiceProperties.autoInit.property.boolValue);
                GameServiceProperties.autoInitDelay.property.floatValue = EditorGUILayout.FloatField(GameServiceProperties.autoInitDelay.content, GameServiceProperties.autoInitDelay.property.floatValue);
                EditorGUI.EndDisabledGroup();

                GameServiceProperties.androidMaxLoginRequest.property.intValue = EditorGUILayout.IntField(GameServiceProperties.androidMaxLoginRequest.content, GameServiceProperties.androidMaxLoginRequest.property.intValue);
                if (!GameServiceProperties.autoInit.property.boolValue)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox(GameServiceManualInitInstruction, MessageType.Info);
                }

                // Saved Games config.
                #if EASY_MOBILE_PRO
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("SAVED GAMES CONFIG", EditorStyles.boldLabel);
                GameServiceProperties.enableSavedGames.property.boolValue = EditorGUILayout.Toggle(GameServiceProperties.enableSavedGames.content, GameServiceProperties.enableSavedGames.property.boolValue);

                if (GameServiceProperties.enableSavedGames.property.boolValue)
                {
                    EditorGUILayout.PropertyField(GameServiceProperties.autoConflictResolutionStrategy.property, 
                        GameServiceProperties.autoConflictResolutionStrategy.content);

                    EditorGUILayout.PropertyField(GameServiceProperties.gpgsDataSource.property, 
                        GameServiceProperties.gpgsDataSource.content);
                }
                #endif

                // Leaderboard setup.
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("LEADERBOARD SETUP", EditorStyles.boldLabel);
                DrawGameServiceItemArray("Leaderboard", GameServiceProperties.leaderboards, ref isLeadeboardsFoldout);

                // Achievement setup.
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("ACHIEVEMENT SETUP", EditorStyles.boldLabel);
                DrawGameServiceItemArray("Achievement", GameServiceProperties.achievements, ref isAchievementsFoldout);

                // Constant generation.
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("CONSTANTS GENERATION", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(GameServiceConstantGenerationIntro, MessageType.None);

                EditorGUILayout.Space();
                if (GUILayout.Button("Generate Constants Class", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                {
                    GenerateGameServiceConstants();
                }
                #endif
            }

            EditorGUILayout.EndVertical();
        }

        #if UNITY_ANDROID && EM_GPGS
        void SetupAndroidGPGSButtonHandler()
        {
            string webClientId = sGPGSWebClientId;          // Web ClientId, not required for Games Services.
            string folder = EM_Constants.GeneratedFolder;    // Folder to contain the generated id constant class.
            string className = EM_Constants.AndroidGPGSConstantClassName;    // Name of the generated id constant class.
            string resourceXmlData = GameServiceProperties.gpgsXmlResources.property.stringValue;    // The xml resources inputted.
            string nearbySvcId = null;  // Nearby Connection Id, not supported by us.
            bool requiresGooglePlus = false;    // Not required Google+ API.

            try
            {
                if (GPGSUtil.LooksLikeValidPackageName(className))
                {
                    SetupAndroidGPGS(webClientId, folder, className, resourceXmlData, nearbySvcId, requiresGooglePlus);
                }
            }
            catch (System.Exception e)
            {
                GPGSUtil.Alert(
                    GPGSStrings.Error,
                    "Invalid classname: " + e.Message);
            }
        }

        // Replicate the "DoSetup" method of the GPGSAndroidSetupUI class.
        void SetupAndroidGPGS(string webClientId, string folder, string className, string resourceXmlData, string nearbySvcId, bool requiresGooglePlus)
        {           
            // Create the folder to store the generated cs file if it doesn't exist.
            SgLib.Editor.FileIO.EnsureFolderExists(folder);

            // Invoke GPGSAndroidSetupUI's PerformSetup method via reflection.
            // In GPGPS versions below 0.9.37, this method has a trailing bool parameter (requiresGooglePlus),
            // while in version 0.9.37 and newer this parameter has been removed. So we need to use reflection
            // to detect the method's parameter list and invoke it accordingly.
            Type gpgsAndroidSetupClass = typeof(GPGSAndroidSetupUI);
            string methodName = "PerformSetup";
            bool isSetupSucceeded = false;

            // GPGS 0.9.37 and newer: PerformSetup has no trailing bool parameter
            MethodInfo newPerformSetup = gpgsAndroidSetupClass.GetMethod(methodName, 
                                             BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, 
                                             Type.DefaultBinder, 
                                             new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) },
                                             new ParameterModifier[0]);

            if (newPerformSetup != null)
            {
                isSetupSucceeded = (bool)newPerformSetup.Invoke(null, new object[] { webClientId, folder, className, resourceXmlData, nearbySvcId });
            }
            else
            {
                // GPGS 0.9.36 and older: PerformSetup has a trailing bool parameter
                MethodInfo oldPerformSetup = gpgsAndroidSetupClass.GetMethod(methodName, 
                                                 BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, 
                                                 Type.DefaultBinder, 
                                                 new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(bool) },
                                                 new ParameterModifier[0]);

                if (oldPerformSetup != null)
                {
                    isSetupSucceeded = (bool)oldPerformSetup.Invoke(null, new object[] { webClientId, folder, className, resourceXmlData, nearbySvcId, requiresGooglePlus });
                }
            }
                
            if (isSetupSucceeded)
            {
                GPGSAndroidSetupUI.CheckBundleId();

                EditorUtility.DisplayDialog(
                    GPGSStrings.Success,
                    GPGSStrings.AndroidSetup.SetupComplete,
                    GPGSStrings.Ok);

                GPGSProjectSettings.Instance.Set(GPGSUtil.ANDROIDSETUPDONEKEY, true);
            }
            else
            {
                GPGSUtil.Alert(
                    GPGSStrings.Error,
                    "Invalid or missing XML resource data.  Make sure the data is" +
                    " valid and contains the app_id element");
            }
        }
        #endif

        // Generate a static class containing constants of leaderboard and achievement names.
        void GenerateGameServiceConstants()
        {           
            // First create a hashtable containing all the names to be stored as constants.
            SerializedProperty ldbProp = GameServiceProperties.leaderboards.property;
            SerializedProperty acmProp = GameServiceProperties.achievements.property;

            // First check if there're duplicate names.
            string duplicateLdbName = EM_EditorUtil.FindDuplicateFieldInArrayProperty(ldbProp, GameServiceItem_NameProperty);
            if (!string.IsNullOrEmpty(duplicateLdbName))
            {
                EM_EditorUtil.Alert("Error: Duplicate Names", "Found duplicate leaderboard name of \"" + duplicateLdbName + "\".");
                return;
            }

            string duplicateAcmName = EM_EditorUtil.FindDuplicateFieldInArrayProperty(acmProp, GameServiceItem_NameProperty);
            if (!string.IsNullOrEmpty(duplicateAcmName))
            {
                EM_EditorUtil.Alert("Error: Duplicate Names", "Found duplicate achievement name of \"" + duplicateAcmName + "\".");
                return;
            }

            // Proceed with adding resource keys.
            Hashtable resourceKeys = new Hashtable();

            // Add the leaderboard names.
            for (int i = 0; i < ldbProp.arraySize; i++)
            {
                SerializedProperty element = ldbProp.GetArrayElementAtIndex(i);
                string name = element.FindPropertyRelative(GameServiceItem_NameProperty).stringValue;

                // Ignore all items with an empty name.
                if (!string.IsNullOrEmpty(name))
                {
                    string key = "Leaderboard_" + name;
                    resourceKeys.Add(key, name);
                }
            }

            // Add the achievement names.
            for (int j = 0; j < acmProp.arraySize; j++)
            {
                SerializedProperty element = acmProp.GetArrayElementAtIndex(j);
                string name = element.FindPropertyRelative(GameServiceItem_NameProperty).stringValue;

                // Ignore all items with an empty name.
                if (!string.IsNullOrEmpty(name))
                {
                    string key = "Achievement_" + name;
                    resourceKeys.Add(key, name);
                }
            }

            if (resourceKeys.Count > 0)
            {
                // Now build the class.
                EM_EditorUtil.GenerateConstantsClass(
                    EM_Constants.GeneratedFolder,
                    EM_Constants.RootNameSpace + "." + EM_Constants.GameServicesConstantsClassName,
                    resourceKeys,
                    true
                );
            }
            else
            {
                EM_EditorUtil.Alert("Constants Class Generation", "Please fill in required information for all leaderboards and achievements.");
            }
        }

        // Draw the array of leaderboards or achievements inside a foldout and the relevant buttons.
        void DrawGameServiceItemArray(string itemType, EMProperty myProp, ref bool isFoldout)
        {
            if (myProp.property.arraySize > 0)
            { 
                EditorGUI.indentLevel++;
                isFoldout = EditorGUILayout.Foldout(isFoldout, myProp.property.arraySize + " " + myProp.content.text);
                EditorGUI.indentLevel--;

                if (isFoldout)
                {
                    // Update the string array of Android GPGPS ids to display in the leaderboards and achievements.
                    gpgsIds = new string[gpgsIdDict.Count + 1];
                    gpgsIds[0] = EM_Constants.NoneSymbol;
                    gpgsIdDict.Keys.CopyTo(gpgsIds, 1);

                    System.Action<SerializedProperty> drawer;

                    if (itemType.Equals("Leaderboard"))
                        drawer = DrawGameServiceLeaderboard;
                    else if (itemType.Equals("Achievement"))
                        drawer = DrawGameServiceAchievement;
                    else
                        throw new System.Exception("Invalid itemType");

                    // Draw the array of achievements or leaderboards.
                    DrawArrayProperty(myProp.property, drawer);

                    // Detect duplicate names.
                    string duplicateName = EM_EditorUtil.FindDuplicateFieldInArrayProperty(myProp.property, GameServiceItem_NameProperty);
                    if (!string.IsNullOrEmpty(duplicateName))
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("Found duplicate name of \"" + duplicateName + "\".", MessageType.Warning);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No " + itemType + " added.", MessageType.None);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Add New " + itemType, GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
            {
                // Add new leaderboard.
                AddNewGameServiceItem(myProp.property);

                // Open the foldout if it's closed.
                isFoldout = true;
            }
        }

        void DrawGameServiceLeaderboard(SerializedProperty property)
        {
            DrawGameServiceItem(property, "Leaderboard");
        }

        void DrawGameServiceAchievement(SerializedProperty property)
        {
            DrawGameServiceItem(property, "Achievement");
        }

        // Draw leaderboard or achievement item.
        void DrawGameServiceItem(SerializedProperty property, string label)
        {
            SerializedProperty name = property.FindPropertyRelative(GameServiceItem_NameProperty);
            SerializedProperty iosId = property.FindPropertyRelative(GameServiceItem_IOSIdProperty);
            SerializedProperty androidId = property.FindPropertyRelative(GameServiceItem_AndroidIdProperty);

            EditorGUILayout.BeginVertical(EM_GUIStyleManager.GetCustomStyle("Item Box"));

            EditorGUILayout.LabelField(string.IsNullOrEmpty(name.stringValue) ? "New " + label : name.stringValue, EditorStyles.boldLabel);
            name.stringValue = EditorGUILayout.TextField("Name", name.stringValue);
            iosId.stringValue = EditorGUILayout.TextField("iOS Id", iosId.stringValue);
            // For Android Id, display a popup of Android leaderboards & achievements for the user to select
            // then assign its associated id to the property.
            EditorGUI.BeginChangeCheck();
            int currentIndex = Mathf.Max(System.Array.IndexOf(gpgsIds, EM_EditorUtil.GetKeyForValue(gpgsIdDict, androidId.stringValue)), 0);                           
            int newIndex = EditorGUILayout.Popup("Android Id", currentIndex, gpgsIds);
            if (EditorGUI.EndChangeCheck())
            {
                // Position 0 is [None].
                if (newIndex == 0)
                {
                    androidId.stringValue = string.Empty;
                }
                else
                {
                    // Record the new android Id.
                    string newName = gpgsIds[newIndex];
                    androidId.stringValue = gpgsIdDict[newName];
                }
            }

            EditorGUILayout.EndVertical();
        }

        void AddNewGameServiceItem(SerializedProperty property)
        {
            if (property.isArray)
            {
                property.InsertArrayElementAtIndex(property.arraySize);

                // Reset the fields of newly added element or it will take the values of the preceding one.
                SerializedProperty newProp = property.GetArrayElementAtIndex(property.arraySize - 1);
                SerializedProperty name = newProp.FindPropertyRelative(GameServiceItem_NameProperty);
                SerializedProperty iosId = newProp.FindPropertyRelative(GameServiceItem_IOSIdProperty);
                SerializedProperty androidId = newProp.FindPropertyRelative(GameServiceItem_AndroidIdProperty);
                name.stringValue = string.Empty;
                iosId.stringValue = string.Empty;
                androidId.stringValue = string.Empty;
            }
        }
    }
}

