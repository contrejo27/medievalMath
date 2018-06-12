using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;

/* TO DO:
 * Save how many stars have been earned for each level.
 */ 

public static class SaveData {

    public static Dictionary<EnumManager.Upgrades, bool> unlockedUpgrades = new Dictionary<EnumManager.Upgrades, bool>();
    public static Dictionary<EnumManager.ActiveQuestionCategories, bool> activeQuestionCategories = new Dictionary<EnumManager.ActiveQuestionCategories, bool>();
    public static int levelsCompleted;
    public static int numStars;

    public static bool loaded = false;

    static string saveFileName = "SaveData.json";

    [Serializable]
    struct PlayerData
    {
        public List<bool> unlockedAbilities;
        public List<bool> questionTypesActive;
        public int levelsCompleted;
        public int numStars;
        
    }

    public static void LoadDataFromJSon()
    {
        string jSonFileText;
        if (!File.Exists(GetFilePath()))
        {
            TextAsset text = (TextAsset)Resources.Load<TextAsset>("cleansave");
            jSonFileText = text.text;
        }
        else
        {
            jSonFileText = File.ReadAllText(GetFilePath());
        }

        unlockedUpgrades.Clear();
        activeQuestionCategories.Clear();
        // This will need to be adjusted to get the data from wherever we're storing it
        
        PlayerData pd = JsonUtility.FromJson<PlayerData>(jSonFileText);

        

        foreach(EnumManager.Upgrades upgrade in Enum.GetValues(typeof(EnumManager.Upgrades)))
        {
            Debug.Log("Upgrade: " + upgrade.ToString() + " index " + Convert.ToInt32(upgrade));
            unlockedUpgrades.Add(upgrade, pd.unlockedAbilities[Convert.ToInt32(upgrade)]);
        }
        foreach(EnumManager.ActiveQuestionCategories cat in Enum.GetValues(typeof(EnumManager.ActiveQuestionCategories)))
        {
            activeQuestionCategories.Add(cat, pd.questionTypesActive[Convert.ToInt32(cat)]);
        }
        levelsCompleted = pd.levelsCompleted;
        numStars = pd.numStars;
        loaded = true;
        Debug.Log("Save data loaded");
    }

    public static void SaveDataToJSon()
    {
        PlayerData pd = new PlayerData();
        pd.unlockedAbilities = new List<bool>();
        pd.questionTypesActive = new List<bool>();

        foreach(bool b in unlockedUpgrades.Values)
        {
            pd.unlockedAbilities.Add(b);
        }
        foreach(bool b in activeQuestionCategories.Values)
        {
            pd.questionTypesActive.Add(b);
        }
        pd.levelsCompleted = levelsCompleted;

        string jSonFile = JsonUtility.ToJson(pd);

        File.WriteAllText(GetFilePath(), jSonFile);
    }

    private static string GetFilePath()
    {
        #if UNITY_EDITOR
                return Application.dataPath + "/JSON/" + saveFileName;
        #elif UNITY_ANDROID
                    return Application.persistentDataPath+saveFileName;
        #elif UNITY_IPHONE
                    return Application.persistentDataPath+"/"+saveFileName;
        #else
                    return Application.dataPath +"/"+saveFileName;
        #endif
    }

}
