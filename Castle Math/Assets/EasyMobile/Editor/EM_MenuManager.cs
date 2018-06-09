using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using UnityEditor.Graphs;

namespace EasyMobile.Editor
{
    public static class EM_MenuManager
    {

        #region Menu items

        [MenuItem("Window/" + EM_Constants.ProductName + "/Settings", false, 1)]
        public static void MenuOpenSettings()
        {
            EM_Settings instance = EM_Settings.LoadSettingsAsset();

            if (instance == null)
            {
                instance = EM_BuiltinObjectCreator.CreateEMSettingsAsset();
            }

            Selection.activeObject = instance;
        }

        [MenuItem("Window/" + EM_Constants.ProductName + "/Create EasyMobile.prefab", false, 2)]
        public static void MenuCreateMainPrefab()
        {
            EM_BuiltinObjectCreator.CreateEasyMobilePrefab(true);
            EM_PluginManager.CheckModules();
        }

        [MenuItem("Window/" + EM_Constants.ProductName + "/Import Play Services Resolver", false, 4)]
        public static void MenuReimportNativePackage()
        {
            EM_PluginManager.ImportPlayServicesResolver(true);
        }

        #if EASY_MOBILE_PRO
        [MenuItem("Window/" + EM_Constants.ProductName + "/Install PlayMaker Actions", false, 3)]
        public static void MenuInstallPlayMakerActions()
        {
            EM_PluginManager.InstallPlayMakerActions(true);
        }
        #endif

        [MenuItem("Window/" + EM_Constants.ProductName + "/Documentation", false, 5)]
        public static void OpenDocumentation()
        {
            Application.OpenURL(EM_Constants.DocumentationURL);
        }

        [MenuItem("Window/" + EM_Constants.ProductName + "/About", false, 6)]
        public static void About()
        {
            EditorWindow.GetWindow<EM_About>(true);
        }

        #endregion

        #region Context menu items

        [MenuItem("GameObject/" + EM_Constants.ProductName + "/EasyMobile Instance", false, 10)]
        public static void CreateEasyMobilePrefabInstance(MenuCommand menuCommand)
        {
            GameObject prefab = EM_EditorUtil.GetMainPrefab();
        
            if (prefab == null)
                prefab = EM_BuiltinObjectCreator.CreateEasyMobilePrefab();
        
            // Stop if another instance already exists as a root object in this scene
            GameObject existingInstance = EM_EditorUtil.FindPrefabInstanceInScene(prefab, EditorSceneManager.GetActiveScene());
            if (existingInstance != null)
            {
                Selection.activeObject = existingInstance;
                return;
            }
        
            // Instantiate an EasyMobile prefab at scene root (parentless) because it's a singleton
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            AddGameObjectToScene(go);
        }

        #if EASY_MOBILE_PRO
        [MenuItem("GameObject/" + EM_Constants.ProductName + "/Clip Player", false, 10)]
        public static void CreateClipPlayer(MenuCommand menuCommand)
        {
            GameObject go = EM_BuiltinObjectCreator.CreateClipPlayer(menuCommand.context as GameObject);
            AddGameObjectToScene(go);
        }

        [MenuItem("GameObject/" + EM_Constants.ProductName + "/Clip Player (UI)", false, 10)]
        public static void CreateClipPlayerUI(MenuCommand menuCommand)
        {
            GameObject go = EM_BuiltinObjectCreator.CreateClipPlayerUI(menuCommand.context as GameObject);
            AddGameObjectToScene(go);
        }
        #endif

        #endregion

        #region Private Stuff

        // Register undo action for the game object and make it active selection.
        private static void AddGameObjectToScene(GameObject go)
        {
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        #endregion
    }
}