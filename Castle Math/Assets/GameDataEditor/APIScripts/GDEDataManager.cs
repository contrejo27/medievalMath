using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameDataEditor
{
    [Flags]
    public enum BasicFieldType
    {
        Undefined = 0,
        Bool = 1,
        Int = 2,
        Float = 4,
        String = 8,
        Vector2 = 16,
        Vector3 = 32,
        Vector4 = 64,
        Color = 128,
	GameObject = 256,
	Texture2D = 512,
	Material = 1024,
	AudioClip = 2048
    }

    public partial class GDEDataManager
    {
        #region Data Collections
        static Dictionary<string, object> _masterData = null;
        static Dictionary<string, object> masterData
        {
            get
            {
                if (_masterData == null)
                    _masterData = new Dictionary<string, object>();
                
                return _masterData;
            }
            set
            {
                if (value == null)
                    _masterData.Clear();
                else
                    _masterData = value;
            }
        }
	static Dictionary<string, HashSet<string>> dataKeysBySchema = null;

        static Dictionary<string, Dictionary<string, object>> _modifiedData = null;
        public static Dictionary<string, Dictionary<string, object>> ModifiedData 
        {
            get 
            {
                if (_modifiedData == null)
                    _modifiedData = new Dictionary<string, Dictionary<string, object>>();
                return _modifiedData;
            }
            set
            {
                _modifiedData = value;
            }
        }
        #endregion

        #region Properties
        static string masterDataPath;

        // Keeping this here for backwards compatability
	public static Dictionary<string, object> DataDictionary
	{
	    get { return masterData; }
	}
	#endregion

        #region Init Methods
        /// <summary>
        /// Loads the specified data file
        /// </summary>
        /// <param name="filePath">Data file path.</param>
	/// <param name="encrypted">Indicates whether data file is encrypted</param>
	/// <returns>True if initialized, false otherwise</returns>
        public static bool Init(string filePath, bool encrypted = false)
        {
            bool result = true;

            try
            {
                masterDataPath = filePath;

                TextAsset dataAsset = Resources.Load(masterDataPath) as TextAsset;
		result = Init(dataAsset, encrypted);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                result = false;
            }
            return result;
        }

	/// <summary>
	/// Loads GDE data from the specified TextAsset
	/// </summary>
	/// <param name="dataAsset">TextAsset that contains GDE data</param>
	public static bool Init(TextAsset dataAsset, bool encrypted = false)
	{
            bool result = true;

	    if (dataAsset == null)
	    {
		result = false;
		Debug.LogError(GDMConstants.ErrorTextAssetNull);
		return result;
	    }

            masterDataPath = dataAsset.name;

	    try
	    {
		string dataContent = string.Empty;
		if (encrypted)
		    dataContent = DecryptGDEData(dataAsset.bytes);
		else
		    dataContent = dataAsset.text;
				
		result = InitFromText(dataContent);
	    }
	    catch(Exception ex)
	    {
		Debug.LogError(ex);
		result = false;
	    }

	    return result;
	}

        public static string DecryptGDEData(byte[] encryptedContent)
	{
	    GDECrypto gdeCrypto = null;
	    TextAsset gdeCryptoResource = (TextAsset)Resources.Load(GDMConstants.MetaDataFileName, typeof(TextAsset));
	    byte[] bytes = Convert.FromBase64String(gdeCryptoResource.text);
	    Resources.UnloadAsset(gdeCryptoResource);

	    using (var stream = new MemoryStream(bytes))
	    {
		BinaryFormatter bin = new BinaryFormatter();
		gdeCrypto = (GDECrypto)bin.Deserialize(stream);
	    }
			
	    string content = string.Empty;
	    if (gdeCrypto != null)
		content = gdeCrypto.Decrypt(encryptedContent);

	    return content;
	}

        #if UNITY_EDITOR
        /// <summary>
        /// This is only public in the Unity Editor.
        /// It is used by the GDE Editors, not intended
        /// for use in runtime!
        /// </summary>
        public static void BuildDataKeysBySchemaList()
	    #else
	    /// <summary>
	    /// Builds the data keys by schema list for lookups by schema.
	    /// </summary>
	    static void BuildDataKeysBySchemaList()
	    #endif
        {
            dataKeysBySchema = new Dictionary<string, HashSet<string>>();
            foreach(KeyValuePair<string, object> pair in masterData)
            {
                if (pair.Key.StartsWith(GDMConstants.SchemaPrefix))
                    continue;

                // Get the schema for the current data set
                string schema;
                Dictionary<string, object> currentDataSet = pair.Value as Dictionary<string, object>;
                currentDataSet.TryGetString(GDMConstants.SchemaKey, out schema);

                // Add it to the list of data keys by type
                HashSet<string> dataKeyList;
                if (dataKeysBySchema.TryGetValue(schema, out dataKeyList))                
                {
                    dataKeyList.Add(pair.Key);
                }
                else
                {
                    dataKeyList = new HashSet<string>();
                    dataKeyList.Add(pair.Key);
                    dataKeysBySchema.Add(schema, dataKeyList);
                }
            }

            // Pull any keys from mod data
            #if !UNITY_WEBPLAYER
            foreach(var item in ModifiedData)
            {
                HashSet<string> keys;
                string schema;

                if (item.Value.TryGetString(GDMConstants.SchemaKey, out schema) && dataKeysBySchema.TryGetValue(schema, out keys))
                    keys.Add(item.Key);
            }
            #endif
        }
        #endregion

        #region Data Access Methods
        /// <summary>
        /// Get the data associated with the specified key in a Dictionary<string, object>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="data">Data</param>
        public static bool Get(string key, out Dictionary<string, object> data)
        {
	    bool result = true;
	    object temp;

            if (masterData == null || key == null)
            {
                data = null;
                return false;
            }

            result = masterData.TryGetValue(key, out temp);
            data = temp as Dictionary<string, object>;

            return result;
        }

        /// <summary>
        /// Returns a subset of the data containing only data sets by the given schema
        /// </summary>
        /// <returns><c>true</c>, if the given schema exists <c>false</c> otherwise.</returns>
        /// <param name="type">Schema.</param>
        /// <param name="data">Subset of the Data Set list containing entries with the specified schema.</param>
        public static bool GetAllDataBySchema(string schema, out Dictionary<string, object> data)
        {
            if (masterData == null)
            {
                data = null;
                return false;
            }

            HashSet<string> dataKeys;
            bool result = true;
            data = new Dictionary<string, object>();

            if (dataKeysBySchema.TryGetValue(schema, out dataKeys))
            {
                foreach(string dataKey in dataKeys)
                {
                    Dictionary<string, object> currentDataSet;
                    if (Get(dataKey, out currentDataSet))
                        data.Add(dataKey.Clone().ToString(), currentDataSet.DeepCopy());
                }
            }
            else
		result = false;

            return result;
        }

	/// <summary>
	/// Gets all GDE Items of a given schema.
	/// </summary>
	/// <returns>GDE Item list</returns>
	/// <typeparam name="T">The schema type.</typeparam>
	public static List<T> GetAllItems<T>() where T : IGDEData
	{
	    List<T> retVal = new List<T>();
	    string schemaName = retVal.SchemaName();
	    List<string> schemaKeys;
			
	    if (GetAllDataKeysBySchema(schemaName, out schemaKeys))
	    {
		for (int x=0;  x<schemaKeys.Count;  x++)
		    retVal.Add((T)Activator.CreateInstance(typeof(T), new object[] { schemaKeys[x] }));
	    }
			
	    return retVal;
	}

        /// <summary>
        /// Gets all data keys by schema.
        /// </summary>
        /// <returns><c>true</c>, if the given schema exists <c>false</c> otherwise.</returns>
        /// <param name="schema">Schema.</param>
        /// <param name="dataKeys">Data Key List.</param>
	public static bool GetAllDataKeysBySchema(string schema, out List<string> dataKeys)
        {
            if (masterData == null)
            {
                dataKeys = null;
                return false;
            }

            HashSet<string> keyList;
            if (dataKeysBySchema != null && dataKeysBySchema.TryGetValue(schema, out keyList))
            {
                dataKeys = keyList.ToList();
                return true;
            }

            dataKeys = null;
            return false;
        }

	/// <summary>
	/// Gets a random GDE Item of the specified schema.
	/// </summary>
	/// <returns>The random item.</returns>
	/// <typeparam name="T">The schema type.</typeparam>
        public static T GetRandom<T>() where T : IGDEData
        {
	    T retVal = null;
	    string schemaName = retVal.SchemaName();
	    List<string> schemaKeys;

	    if (GetAllDataKeysBySchema(schemaName, out schemaKeys))
	    {
		string key = schemaKeys.Random();
		retVal = (T)Activator.CreateInstance(typeof(T), new object[] { key });
	    }

            return retVal;
        }
        #endregion

	#region Editor Only Methods
        /// <summary>
        /// Updates the GDE Item in the loaded GDE data set in memory.
        /// </summary>
        /// <param name="item">GDE Item</param>
        /// <param name="rebuildKeys">If set to <c>true</c> rebuild item keys. Only set to true if the item you are
        /// updating does not already exist.</param>
	public static void UpdateItem(IGDEData item, bool rebuildKeys = false)
	{
            #if UNITY_EDITOR
            if (item == null)
                return;

	    bool rebuildKeyList = !masterData.ContainsKey(item.Key);
			
            masterData.TryAddOrUpdateValue(item.Key, item.SaveToDict());
            item.UpdateCustomItems(!rebuildKeyList);

	    if (rebuildKeyList && rebuildKeys)
		BuildDataKeysBySchemaList();
	    #endif
	}

        static string GetDataFilePath()
        {
            if (string.IsNullOrEmpty(masterDataPath))
                throw new Exception("Can't find path because masterDataPath is null or empty. Has a data set been loaded?");

            string path = string.Empty;

            #if UNITY_EDITOR && !UNITY_WEBPLAYER
            string masterDataAssetName = Path.GetFileNameWithoutExtension(masterDataPath);
            var results = AssetDatabase.FindAssets(masterDataAssetName);
            string currentDir = Environment.CurrentDirectory;
            
            foreach(string guid in results)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string assetName = Path.GetFileNameWithoutExtension(assetPath);
                if (assetName.Equals(masterDataAssetName))
                {
                    path = Path.Combine(currentDir, assetPath);
                    break;
                }
            }
            
            if (string.IsNullOrEmpty(path))
            {
                path = Path.Combine(Application.dataPath, "GameDataEditor");
                path = Path.Combine(path, "Resources");
                path = Path.Combine(path, masterDataPath);
            }
            
            path = Path.ChangeExtension(path, "txt");
            #endif

            return path;
        }

        static string GetRelativePathFromFullPath(string fullPath)
        {
            return fullPath.Replace(Environment.CurrentDirectory, string.Empty).TrimStart(Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Saves the loaded GDE data set to the data file on disk. Only used
        /// while in play mode in the Unity Editor.
        /// </summary>
        public static void SaveToDisk()
	{
	    #if UNITY_EDITOR && !UNITY_WEBPLAYER
            string path = GetDataFilePath();
	    File.WriteAllText(path, Json.Serialize(masterData));
	    string relativePath = GetRelativePathFromFullPath(path);
            AssetDatabase.ImportAsset(relativePath);
	    #endif
	}
	#endregion

        #region Get/Set Methods
        public static void SetUnityObject<T>(string key, string field, T val) where T : UnityEngine.Object
        {
        }
        
        public static void SetUnityObjectList<T>(string key, string field, List<T> val) where T : UnityEngine.Object
        {
        }
        
        public static void SetUnityObjectTwoDList<T>(string key, string field, List<List<T>> val) where T : UnityEngine.Object
        {
        }
        #endregion

	#region Deprecated Get/Set Methods
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetBool(string key, bool val) {
	    SetBool(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetInt(string key, int val) {
	    SetInt(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetFloat(string key, float val) {
	    SetFloat(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetString(string key, string val) {
	    SetString(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetVector2(string key, Vector2 val) {
	    SetVector2(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetVector3(string key, Vector3 val) {
	    SetVector3(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetVector4(string key, Vector4 val) {
	    SetVector4(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetColor(string key, Color val) {
	    SetColor(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetGameObject(string key, GameObject val) {
	    SetGameObject(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetTexture2D(string key, Texture2D val) {
	    SetTexture2D(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetMaterial(string key, Material val) {
	    SetMaterial(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetAudioClip(string key, AudioClip val) {
	    SetAudioClip(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetCustom<T>(string key, T val) where T : IGDEData {
	    SetCustom(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetBoolList(string key, List<bool> val) {
	    SetBoolList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetIntList(string key, List<int> val) {
	    SetIntList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetFloatList(string key, List<float> val) {
	    SetFloatList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetStringList(string key, List<string> val) {
	    SetStringList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetVector2List(string key, List<Vector2> val) {
	    SetVector2List(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetVector3List(string key, List<Vector3> val) {
	    SetVector3List(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetVector4List(string key, List<Vector4> val) {
	    SetVector4List(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetColorList(string key, List<Color> val) {
	    SetColorList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetGameObjectList(string key, List<GameObject> val) {
	    SetGameObjectList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetTexture2DList(string key, List<Texture2D> val) {
	    SetTexture2DList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetMaterialList(string key, List<Material> val) {
	    SetMaterialList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetAudioClipList(string key, List<AudioClip> val) {
	    SetAudioClipList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetCustomList<T>(string key, List<T> val) where T : IGDEData {
	    SetCustomList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetIntTwoDList(string key, List<List<int>> val) {
	    SetIntTwoDList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetBoolTwoDList(string key, List<List<bool>> val) {
	    SetBoolTwoDList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetFloatTwoDList(string key, List<List<float>> val) {
	    SetFloatTwoDList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetStringTwoDList(string key, List<List<string>> val) {
	    SetStringTwoDList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetVector2TwoDList(string key, List<List<Vector2>> val) {
	    SetVector2TwoDList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetVector3TwoDList(string key, List<List<Vector3>> val) {
	    SetVector3TwoDList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetVector4TwoDList(string key, List<List<Vector4>> val) {
	    SetVector4TwoDList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetColorTwoDList(string key, List<List<Color>> val) {
	    SetColorTwoDList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetGameObjectTwoDList(string key, List<List<GameObject>> val) {
	    SetGameObjectTwoDList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetTexture2DTwoDList(string key, List<List<Texture2D>> val) {
	    SetTexture2DTwoDList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetMaterialTwoDList(string key, List<List<Material>> val) {
	    SetMaterialTwoDList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetAudioClipTwoDList(string key, List<List<AudioClip>> val) {
	    SetAudioClipTwoDList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static void SetCustomTwoDList<T>(string key, List<List<T>> val) where T : IGDEData {
	    SetCustomTwoDList(key, string.Empty, val);
	}

	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static bool GetBool(string key, bool val) {
	    return GetBool(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static int GetInt(string key, int val) {
	    return GetInt(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static float GetFloat(string key, float val) {
	    return GetFloat(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static string GetString(string key, string val) {
	    return GetString(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static Vector2 GetVector2(string key, Vector2 val) {
	    return GetVector2(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static Vector3 GetVector3(string key, Vector3 val) {
	    return GetVector3(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static Vector4 GetVector4(string key, Vector4 val) {
	    return GetVector4(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static Color GetColor(string key, Color val) {
	    return GetColor(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static GameObject GetGameObject(string key, GameObject val) {
	    return GetGameObject(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static Texture2D GetTexture2D(string key, Texture2D val) {
	    return GetTexture2D(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static Material GetMaterial(string key, Material val) {
	    return GetMaterial(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static AudioClip GetAudioClip(string key, AudioClip val) {
	    return GetAudioClip(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static T GetCustom<T>(string key, T val) where T : IGDEData {
	    return GetCustom(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<bool> GetBoolList(string key, List<bool> val) {
	    return GetBoolList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<int> GetIntList(string key, List<int> val) {
	    return GetIntList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<float> GetFloatList(string key, List<float> val) {
	    return GetFloatList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<string> GetStringList(string key, List<string> val) {
	    return GetStringList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<Vector2> GetVector2List(string key, List<Vector2> val) {
	    return GetVector2List(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<Vector3> GetVector3List(string key, List<Vector3> val) {
	    return GetVector3List(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<Vector4> GetVector4List(string key, List<Vector4> val) {
	    return GetVector4List(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<Color> GetColorList(string key, List<Color> val) {
	    return GetColorList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<GameObject> GetGameObjectList(string key, List<GameObject> val) {
	    return GetGameObjectList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<Texture2D> GetTexture2DList(string key, List<Texture2D> val) {
	    return GetTexture2DList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<Material> GetMaterialList(string key, List<Material> val) {
	    return GetMaterialList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<AudioClip> GetAudioClipList(string key, List<AudioClip> val) {
	    return GetAudioClipList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<T> GetCustomList<T>(string key, List<T> val) where T : IGDEData {
	    return GetCustomList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<List<int>> GetIntTwoDList(string key, List<List<int>> val) {
	    return GetIntTwoDList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<List<bool>> GetBoolTwoDList(string key, List<List<bool>> val) {
	    return GetBoolTwoDList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<List<float>> GetFloatTwoDList(string key, List<List<float>> val) {
	    return GetFloatTwoDList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<List<string>> GetStringTwoDList(string key, List<List<string>> val) {
	    return GetStringTwoDList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<List<Vector2>> GetVector2TwoDList(string key, List<List<Vector2>> val) {
	    return GetVector2TwoDList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<List<Vector3>> GetVector3TwoDList(string key, List<List<Vector3>> val) {
	    return GetVector3TwoDList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<List<Vector4>> GetVector4TwoDList(string key, List<List<Vector4>> val) {
	    return GetVector4TwoDList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<List<Color>> GetColorTwoDList(string key, List<List<Color>> val) {
	    return GetColorTwoDList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<List<GameObject>> GetGameObjectTwoDList(string key, List<List<GameObject>> val) {
	    return GetGameObjectTwoDList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<List<Texture2D>> GetTexture2DTwoDList(string key, List<List<Texture2D>> val) {
	    return GetTexture2DTwoDList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<List<Material>> GetMaterialTwoDList(string key, List<List<Material>> val) {
	    return GetMaterialTwoDList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<List<AudioClip>> GetAudioClipTwoDList(string key, List<List<AudioClip>> val) {
	    return GetAudioClipTwoDList(key, string.Empty, val);
	}
		
	[Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
	public static List<List<T>> GetCustomTwoDList<T>(string key, List<List<T>> val) where T : IGDEData {
	    return GetCustomTwoDList(key, string.Empty, val);
	}
	#endregion
    }
}
