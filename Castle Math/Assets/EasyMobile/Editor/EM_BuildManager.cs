#if UNITY_ANDROID || UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace EasyMobile.Editor
{
    #if UNITY_5_6_OR_NEWER
    using UnityEditor.Build;

    public class EM_PreBuildProcessor : IPreprocessBuild
    {
        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildTarget target, string path)
        {
        EM_BuildProcessorUtil.PreBuildProcessing(target, path);
        }
    }

    //---------------------------------------------------------------------
    // UNCOMMENT IF WE NEED TO DO POST-BUILD PROCESSING ON UNITY >= 5.6
    //---------------------------------------------------------------------
    /*
    public class EM_PostBuildProcessor : IPostprocessBuild
    {
        public int callbackOrder { get { return 9999; } }

        public void OnPostprocessBuild(BuildTarget target, string path)
        {
            EM_BuildProcessorUtil.PostBuildProcessing(target, path);
        }
    }
    */
    #endif

    //---------------------------------------------------------------------
    // UNCOMMENT IF WE NEED TO DO POST-BUILD PROCESSING ON UNITY < 5.6
    //---------------------------------------------------------------------
    /*
    #if !UNITY_5_6_OR_NEWER
    using UnityEditor.Callbacks;

    public class EM_LegacyBuildProcessor
    {
        [PostProcessBuildAttribute(9999)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            EM_BuildProcessorUtil.PostBuildProcessing(target, path);
        }
    }
    #endif
    */

    public class EM_BuildProcessorUtil
    {
        public static void PreBuildProcessing(BuildTarget target, string path)
        {
            // Search through all enabled scenes in the BuildSettings to find the EasyMobile prefab instance.
            // Warn the user if none was found.
            GameObject prefab = EM_EditorUtil.GetMainPrefab();

            if (prefab != null)
            {
                string[] enabledScenePaths = EM_EditorUtil.GetScenePathInBuildSettings(true);
                if (!EM_EditorUtil.IsPrefabInstanceFoundInScenes(prefab, enabledScenePaths))
                {
                    string title = "EasyMobile Instance Missing";
                    string msg = "No root-level instance of the EasyMobile prefab was found in the enabled scene(s). " +
                                 "Please add one to the first scene of your game for the plugin to function properly.";
                    #if !UNITY_CLOUD_BUILD
                    EM_EditorUtil.Alert(title, msg);
                    #else   
                    Debug.LogWarning(msg);
                    #endif
                }
            }
        }

        public static void PostBuildProcessing(BuildTarget target, string path)
        {
            //---------------------------------------------------------------------
            // UNCOMMENT IF WE NEED TO DO POST-BUILD PROCESSING ON IOS
            //---------------------------------------------------------------------
            /*
            #if UNITY_IOS
            if (target == BuildTarget.iOS)
            {
                // Read.
                string pbxPath = PBXProject.GetPBXProjectPath(path);
                PBXProject project = new PBXProject();
                project.ReadFromString(File.ReadAllText(pbxPath));

                string targetName = PBXProject.GetUnityTargetName();
                string targetGUID = project.TargetGuidByName(targetName);

                // Add frameworks if needed.

                // Add required flags.
                project.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC");

                // Write.
                File.WriteAllText(pbxPath, project.WriteToString());
            }
            #endif
            */
        }
    }
}
#endif