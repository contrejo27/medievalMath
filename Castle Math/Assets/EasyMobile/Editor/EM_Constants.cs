using UnityEngine;
using System.Collections;

namespace EasyMobile.Editor
{
    public static class EM_Constants
    {
        // Product name. DO NOT CHANGE: used in Window > Easy Mobile menu.
        public const string ProductName = "Easy Mobile";
        public const string Copyright = "© 2017-2018 SgLib Games LLC. All Rights Reserved.";

        // Current version
        public const string versionString = "1.3.0";
        public const int versionInt = 0x010300;

        // Folder
        public const string RootPath = "Assets/EasyMobile";
        public const string EditorFolder = RootPath + "/Editor";
        public const string TemplateFolder = EditorFolder + "/Templates";
        public const string GeneratedFolder = RootPath + "/Generated";
        public const string MainPrefabFolder = RootPath;
        public const string MaterialsFolder = RootPath + "/Materials";
        public const string PackagesFolder = RootPath + "/Packages";
        public const string SkinFolder = RootPath + "/GUISkins";
        public const string SkinTextureFolder = SkinFolder + "/Textures";
        public const string ResourcesFolder = RootPath + "/Resources";
        public const string ScriptsFolder = RootPath + "/Scripts";
        public const string ReceiptValidationFolder = "Assets/Plugins/UnityPurchasing/generated";
        public const string AssetsPluginsAndroidFolder = "Assets/Plugins/Android";
        public const string AssetsPluginsIOSFolder = "Assets/Plugins/iOS";

        // Asset and stuff
        public const string SettingsAssetName = "EM_Settings";
        public const string SettingsAssetExtension = ".asset";
        public const string SettingsAssetPath = ResourcesFolder + "/EM_Settings.asset";
        public const string MainPrefabName = "EasyMobile";
        public const string PrefabExtension = ".prefab";
        public const string MainPrefabPath = MainPrefabFolder + "/EasyMobile.prefab";
        public const string PluginSettingsFilePath = EditorFolder + "/EasyMobileSettings.txt";
        public const string ClipPlayerMaterialPath = MaterialsFolder + "/ClipPlayerMat.mat";

        // UnityPackages
        public const string PlayServicersResolverPackagePath = PackagesFolder + "/play-services-resolver.unitypackage";
        public const string PlayMakerActionsPackagePath = PackagesFolder + "/PlayMakerActions.unitypackage";

        // Android native package names.
        public const string AndroidNativePackageName = "com.sglib.easymobile.androidnative";
        public const string AndroidNativeNotificationPackageName = "com.sglib.easymobile.androidnative.notification";

        // Generated class names
        public const string RootNameSpace = "EasyMobile";
        public const string AndroidGPGSConstantClassName = "EM_GPGSIds";
        public const string GameServicesConstantsClassName = "EM_GameServicesConstants";
        public const string IAPConstantsClassName = "EM_IAPConstants";
        public const string NotificationsConstantsClassName = "EM_NotificationsConstants";
        public const string NotificationAndroidResFolderName = "EMNotificationResources";

        // URLs
        public const string DocumentationURL = "https://sglibgames.gitbooks.io/easy-mobile-user-guide/content/";
        public const string SupportEmail = "support@sglibgames.com";
        public const string SupportEmailSubject = "[EM Pro][YOUR_INVOICE_NUMBER]";

        // Common symbols
        public const string NoneSymbol = "[None]";
        public const string DeleteSymbol = "-";
        public const string UpSymbol = "↑";
        public const string DownSymbol = "↓";

        // ProjectSettings keys
        public const string PSK_EMVersionString = "VERSION";
        public const string PSK_EMVersionInt = "VERSION_INT";
        public const string PSK_ImportedPlayServicesResolver = "IMPORTED_PLAY_SERVICES_RESOLVER";
    }
}

