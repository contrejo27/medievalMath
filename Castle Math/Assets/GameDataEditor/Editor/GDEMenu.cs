using UnityEngine;
using UnityEditor;
using GameDataEditor;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class GDEMenu : EditorWindow {

	const string contextItemLocation = "Assets/Game Data Editor";
	const string menuItemLocation = "Window/Game Data Editor";
	const int menuItemStartPriority = 300;

	[MenuItem(menuItemLocation + "/" + GDEConstants.DefineDataMenu, false, menuItemStartPriority)]
	static void GDESchemaEditor()
	{
		EditorWindow.GetWindow<GDESchemaManagerWindow>(false, GDEConstants.DefineDataMenu);
	}
	
	[MenuItem(menuItemLocation + "/" + GDEConstants.CreateDataMenu, false, menuItemStartPriority+1)]
	static void GDEItemEditor()
	{
		EditorWindow.GetWindow<GDEItemManagerWindow>(false, GDEConstants.CreateDataMenu);
	}

	
	// **** Divider Here **** //
    
    #if GDE_PLAYMAKER_SUPPORT
    [MenuItem(menuItemLocation + "/" + GDEConstants.PlaymakerSupportMenu + GDEConstants.DisableMenu, false, menuItemStartPriority+100)]
    static void DisablePM()
    {
        DoSymbolDefine(GDEConstants.PMSymbol, false);
    }
    #else
    [MenuItem(menuItemLocation + "/" + GDEConstants.PlaymakerSupportMenu + GDEConstants.EnableMenu, false, menuItemStartPriority+100)]
    static void EnablePM()
    {
        DoSymbolDefine(GDEConstants.PMSymbol, true);
    }
    #endif
        
    #if GDE_ICODE_SUPPORT
    [MenuItem(menuItemLocation + "/" + GDEConstants.iCodeSupportMenu + GDEConstants.DisableMenu, false, menuItemStartPriority+100)]
    static void DisableiCode()
    {
        DoSymbolDefine(GDEConstants.iCodeSymbol, false);
    }
    #else
    [MenuItem(menuItemLocation + "/" + GDEConstants.iCodeSupportMenu + GDEConstants.EnableMenu, false, menuItemStartPriority+100)]
    static void EnableiCode()
    {
        DoSymbolDefine(GDEConstants.iCodeSymbol, true);
    }
    #endif
    
    static void DoSymbolDefine(string define, bool add)
    {
        string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        bool symbolsChanged = false;
        
        // Replace any spaces and trim semicolons
        currentDefines = currentDefines.Replace(" ", string.Empty);
        currentDefines = currentDefines.Trim(';');
        
        List<string> symbols = currentDefines.Split(';').ToList();
        
        if (!symbols.Contains(define) && add)
        {
            symbols.Add(define);
            symbolsChanged = true;
        }
        else if (symbols.Contains(define) && !add)
        {
            symbols.Remove(define);
            symbolsChanged = true;
        }
        
        if (symbolsChanged)
        {
            currentDefines = string.Empty;
            
            symbols.ForEach(symbol => currentDefines += (symbol + ";"));
            currentDefines = currentDefines.Trim(';');
            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currentDefines);
        }
    }


    // **** Divider Here **** //


	[MenuItem(menuItemLocation + "/" + GDEConstants.EncryptMenu, false, menuItemStartPriority+200)]
	static void GDEEncrypt()
	{
		Debug.Log(GDEConstants.EncryptingMsg);
		string dataFilePath = GDEItemManager.DataFilePath;
		GDEEncryption.Encrypt(File.ReadAllText(dataFilePath), GDEItemManager.EncryptedFilePath);
		Debug.Log(GDEConstants.DoneLbl);

		var window = EditorWindow.GetWindow<GDEEncryptionWindow>(true, GDEConstants.EncryptionCompleteLbl);

		window.minSize = new Vector2(650, 200);
		window.Show();
	}

    [MenuItem(menuItemLocation + "/" + GDEConstants.GenerateExtensionsMenu, false, menuItemStartPriority+201)]
	public static void DoGenerateCustomExtensions()
	{
		GDEItemManager.Load();

		GDECodeGen.GenStaticSchemaKeysClass(GDEItemManager.AllSchemas);
		GDECodeGen.GenStaticKeysClass(GDEItemManager.AllSchemas);
		GDECodeGen.GenClasses(GDEItemManager.AllSchemas);
		
		AssetDatabase.Refresh();
	}

    [MenuItem(menuItemLocation + "/" + GDEConstants.ClearModDataMenu, false, menuItemStartPriority+202)]
    static void ClearModData()
    {
        GDEDataManager.ClearSaved();

        #if !UNITY_WEBPLAYER
        Debug.Log("Removed "+ Application.persistentDataPath + "/" + GDMConstants.ModDataFileName);
        #endif
    }
    
    
    // **** Divider Here **** //


	[MenuItem(menuItemLocation + "/" + GDEConstants.ImportSpreadsheetMenu, false, menuItemStartPriority+300)]
	public static void DoSpreadsheetImport()
	{
		GDEExcelManager.DoImport();
	}

	/// <summary>
	/// Displays the Localization Editor Export Spreadsheet Wizard
	/// </summary>
	[MenuItem(menuItemLocation + "/" + GDEConstants.ExportSpreadsheetLbl, false, menuItemStartPriority+301)]
	public static void DoExportSpreadsheet()
	{
		GDEExcelManager.DoExport();
	}

	[MenuItem(menuItemLocation + "/" + GDEConstants.ClearExcelMenu, false, menuItemStartPriority+302)]
	static void GDEClearExcelSettings()
	{
		GDEExcelManager.ClearExcelSettings();
	}


	// **** Divider Here **** //


	[MenuItem(menuItemLocation + "/" + GDEConstants.ForumMenu, false, menuItemStartPriority+400)]
	static void GDEForumPost()
	{
		Application.OpenURL(GDEConstants.ForumURL);
	}

	[MenuItem(menuItemLocation + "/" + GDEConstants.DocsMenu, false, menuItemStartPriority+401)]
	static void GDEFreeDocs()
	{
		Application.OpenURL(GDEConstants.DocURL);
	}


	// *** Divider Here *** //


	[MenuItem(menuItemLocation + "/" + GDEConstants.BundleMenu, false, menuItemStartPriority+500)]
	static void GDEUpgradeBundle()
	{
		Application.OpenURL(GDEConstants.BundleURL);
	}

	[MenuItem(menuItemLocation + "/" + GDEConstants.RateMenu, false, menuItemStartPriority+501)]
    static void GDERateMe()
    {
        Application.OpenURL(GDEConstants.RateMeURL);
    }
	
    [MenuItem(menuItemLocation + "/" + GDEConstants.ContactMenu + "/" + GDEConstants.EmailMenu, false, menuItemStartPriority+502)]
	static void GDEEmail()
	{
		Application.OpenURL(GDEConstants.MailTo);
	}

	[MenuItem(menuItemLocation + "/" + GDEConstants.ContactMenu + "/Twitter" , false, menuItemStartPriority+503)]
	static void GDETwitter()
	{
		Application.OpenURL(GDEConstants.Twitter);
	}


	// **** Context Menu Below **** //


	[MenuItem(contextItemLocation + "/" + GDEConstants.LoadDataMenu, true)]
	static bool GDELoadDataValidation()
	{
		return Selection.activeObject != null && Selection.activeObject.GetType() == typeof(TextAsset);
	}

	[MenuItem(contextItemLocation + "/" + GDEConstants.LoadDataMenu, false, menuItemStartPriority)]
	static void GDELoadData () 
	{
		string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
		string fullPath = Path.GetFullPath(assetPath);

		GDESettings.Instance.DataFilePath = fullPath;
		GDESettings.Instance.Save();

		GDEItemManager.Load(true);
	}

	[MenuItem(contextItemLocation + "/" + GDEConstants.LoadAndGenMenu, true)]
	static bool GDELoadAndGenDataValidation()
	{
		return GDELoadDataValidation();
	}

	[MenuItem(contextItemLocation + "/" + GDEConstants.LoadAndGenMenu, false, menuItemStartPriority+1)]
	static void GDELoadAndGenData () 
	{
		GDELoadData();
		DoGenerateCustomExtensions();
	}
}

