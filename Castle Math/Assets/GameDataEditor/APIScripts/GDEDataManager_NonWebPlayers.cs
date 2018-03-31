#if !UNITY_WEBPLAYER
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameDataEditor
{
	public partial class GDEDataManager
	{
		#region Properties
		static string modifiedDataPath = Path.Combine(Application.persistentDataPath, GDMConstants.ModDataFileName);
		#endregion

		#region Init Methods
		/// <summary>
		/// Loads GDE data from a string
		/// </summary>
		/// <param name="dataString">String that contains GDE data</param>
		public static bool InitFromText(string dataString)
		{
            bool result = true;

			try
			{
				masterData = Json.Deserialize(dataString) as Dictionary<string, object>;
				LoadModData();
				
				BuildDataKeysBySchemaList();
			}
			catch(Exception ex)
			{
				Debug.LogError(ex);
                result = false;
			}
			
			return result;
		}
		#endregion

		#region Data Access Methods
		public static void ResetToDefault(string itemName, string fieldName = "")
		{
			Dictionary<string, object> dict;

            if (string.IsNullOrEmpty(fieldName))
                ModifiedData.Remove(itemName);
            else if (ModifiedData.TryGetValue(itemName, out dict))
				dict.Remove(fieldName);
		}
		#endregion

		#region Persistent Data Methods
		public static void Save()
		{
			try
			{
				using (var stream = new MemoryStream())
				{
					BinaryFormatter bin = new BinaryFormatter();
					bin.Serialize(stream, ModifiedData);
					
					File.WriteAllBytes(modifiedDataPath, stream.ToArray());
				}
			} 
			catch (Exception ex)
			{
				Debug.LogError(GDMConstants.ErrorSavingData);
				Debug.LogException(ex);
			}
		}
		
		public static void ClearSaved()
		{
			if (File.Exists(modifiedDataPath))
				File.Delete(modifiedDataPath);

			ModifiedData.Clear();
		}

        public static void RegisterItem(string schema, string key)
        {
            Dictionary<string, object> dict;
            if (!ModifiedData.TryGetValue(key, out dict))
            {
                dict = new Dictionary<string, object>();
                dict.TryAddOrUpdateValue(GDMConstants.SchemaKey, schema);
                ModifiedData.TryAddOrUpdateValue(key, dict);

                HashSet<string> keys;
                dataKeysBySchema.TryGetValue(schema, out keys);

                if (keys != null)
                    keys.Add(key);
                else
                {
                    keys = new HashSet<string>();
                    keys.Add(key);
                    dataKeysBySchema.Add(schema, keys);
                }
            }
        }

        /// <summary>
        /// This is only public in the Unity Editor.
        /// It is used by the GDE Editors, not intended
        /// for use in runtime!
        /// </summary>
		public static void LoadModData()
		{
			try
			{
				if (File.Exists(modifiedDataPath))
				{
					byte[] bytes = File.ReadAllBytes(modifiedDataPath);
					
					using (var stream = new MemoryStream(bytes))
					{
						BinaryFormatter bin = new BinaryFormatter();
						ModifiedData = bin.Deserialize(stream) as Dictionary<string, Dictionary<string, object>>;
					}
				}
				else
				{
					ModifiedData = new Dictionary<string, Dictionary<string, object>>();
				}
			}
			catch(Exception ex)
			{
				Debug.LogError(GDMConstants.ErrorLoadingSavedData);
				Debug.LogException(ex);
			}
		}
		#endregion

		#region Get Saved Data Methods (Basic Types)
		public static string GetString(string key, string field, string defaultVal)
		{
			string retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp) && temp != null)
    					retVal = temp.ToString();
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		public static List<string> GetStringList(string key, string field, List<string> defaultVal = null)
		{
			List<string> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp))
    					retVal = temp as List<string>;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
			
		}
		
		public static List<List<string>> GetStringTwoDList(string key, string field, List<List<string>> defaultVal = null)
		{
			List<List<string>> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp))
    					retVal = temp as List<List<string>>;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		public static int GetInt(string key, string field, int defaultVal)
		{
			int retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp))
    					retVal = Convert.ToInt32(temp);
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
        public static List<int> GetIntList(string key, string field, List<int> defaultVal = null)
		{
			List<int> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp))
    					retVal = temp as List<int>;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		public static List<List<int>> GetIntTwoDList(string key, string field, List<List<int>> defaultVal = null)
		{
			List<List<int>> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp))
    					retVal = temp as List<List<int>>;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		
		public static float GetFloat(string key, string field, float defaultVal)
		{
			float retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp))
    					retVal = Convert.ToSingle(temp);
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		public static List<float> GetFloatList(string key, string field, List<float> defaultVal = null)
		{
			List<float> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp))
    					retVal = temp as List<float>;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		public static List<List<float>> GetFloatTwoDList(string key, string field, List<List<float>> defaultVal = null)
		{
			List<List<float>> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp))
    					retVal = temp as List<List<float>>;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		
		public static bool GetBool(string key, string field, bool defaultVal)
		{
			bool retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp))
    					retVal = Convert.ToBoolean(temp);
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}

        public static List<bool> GetBoolList(string key, string field, List<bool> defaultVal = null)
		{
			List<bool> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp))
    					retVal = temp as List<bool>;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		public static List<List<bool>> GetBoolTwoDList(string key, string field, List<List<bool>> defaultVal = null)
		{
			List<List<bool>> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp))
    					retVal = temp as List<List<bool>>;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		
		public static Color GetColor(string key, string field, Color defaultVal)
		{
			Color retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if(dict.TryGetValue(field, out temp) && temp.GetType().IsAssignableFrom(typeof(GDEColor)))
    					retVal = (GDEColor)temp;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
			
		}
		
		public static List<Color> GetColorList(string key, string field, List<Color> defaultVal = null)
		{
			List<Color> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp) && temp != null && temp.GetType().IsAssignableFrom(typeof(List<GDEColor>)))
    					retVal = (temp as List<GDEColor>).ConvertAll(x => (Color)x);
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		public static List<List<Color>> GetColorTwoDList(string key, string field, List<List<Color>> defaultVal = null)
		{
			List<List<Color>> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp) && temp != null && temp.GetType().IsAssignableFrom(typeof(List<List<GDEColor>>)))
    					retVal = (temp as List<List<GDEColor>>).ConvertAll(x => x.ConvertAll(y => (Color)y));;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		public static Vector2 GetVector2(string key, string field, Vector2 defaultVal)
		{
			Vector2 retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
                    if (dict.TryGetValue(field, out temp) && temp.GetType().IsAssignableFrom(typeof(GDEV2)))
    					retVal = (GDEV2)temp;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
			
		}
		
		public static List<Vector2> GetVector2List(string key, string field, List<Vector2> defaultVal = null)
		{
			List<Vector2> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp) && temp != null && temp.GetType().IsAssignableFrom(typeof(List<GDEV2>)))
    					retVal = (temp as List<GDEV2>).ConvertAll(x => (Vector2)x);
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		public static List<List<Vector2>> GetVector2TwoDList(string key, string field, List<List<Vector2>> defaultVal = null)
		{
			List<List<Vector2>> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp) && temp.GetType().IsAssignableFrom(typeof(List<List<GDEV2>>)))
                    retVal = (temp as List<List<GDEV2>>).ConvertAll(x => {
                        if (x != null) 
                            return x.ConvertAll(y => (Vector2)y);
                        return null;
                        }
                    );
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		public static Vector3 GetVector3(string key, string field, Vector3 defaultVal)
		{
			Vector3 retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp) && temp.GetType().IsAssignableFrom(typeof(GDEV3)))
    					retVal = (GDEV3)temp;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
			
		}
		
		public static List<Vector3> GetVector3List(string key, string field, List<Vector3> defaultVal = null)
		{
			List<Vector3> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp) && temp.GetType().IsAssignableFrom(typeof(List<GDEV3>)))
    					retVal = (temp as List<GDEV3>).ConvertAll(x => (Vector3)x);
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		public static List<List<Vector3>> GetVector3TwoDList(string key, string field, List<List<Vector3>> defaultVal = null)
		{
			List<List<Vector3>> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp)  && temp != null && temp.GetType().IsAssignableFrom(typeof(List<List<GDEV3>>)))
    					retVal = (temp as List<List<GDEV3>>).ConvertAll(x => x.ConvertAll(y => (Vector3)y));
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		public static Vector4 GetVector4(string key, string field, Vector4 defaultVal)
		{
			Vector4 retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp) && temp.GetType().IsAssignableFrom(typeof(GDEV4)))
    					retVal = (GDEV4)temp;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
			
		}
		
		public static List<Vector4> GetVector4List(string key, string field, List<Vector4> defaultVal = null)
		{
			List<Vector4> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp) && temp != null && temp.GetType().IsAssignableFrom(typeof(List<GDEV4>)))
    					retVal = (temp as List<GDEV4>).ConvertAll(x => (Vector4)x);
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		public static List<List<Vector4>> GetVector4TwoDList(string key, string field, List<List<Vector4>> defaultVal = null)
		{
			List<List<Vector4>> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					object temp;
					if (dict.TryGetValue(field, out temp) && temp != null && temp.GetType().IsAssignableFrom(typeof(List<List<GDEV4>>)))
    					retVal = (temp as List<List<GDEV4>>).ConvertAll(x => x.ConvertAll(y => (Vector4)y));
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}

		public static T GetUnityObject<T>(string key, string field, T defaultVal) where T : UnityEngine.Object
		{
			string path = GetString(key, field, string.Empty);
			if (!string.IsNullOrEmpty(path))
				return Resources.Load<T>(path);
			
			return defaultVal;
		}
		
		public static List<T> GetUnityObjectList<T>(string key, string field, List<T> defaultVal = null) where T : UnityEngine.Object
		{
			List<string> paths = GetStringList(key, field, null);
			List<T> retVal = defaultVal;

			if (paths != null)
			{
				retVal = new List<T>();
				foreach(string path in paths)
				{
					if (!string.IsNullOrEmpty(path))
						retVal.Add(Resources.Load<T>(path));
					else
						retVal.Add(null);
				}
			}
			
			return retVal;
		}
		
		public static List<List<T>> GetUnityObjectTwoDList<T>(string key, string field, List<List<T>> defaultVal = null) where T : UnityEngine.Object
		{
			List<List<string>> paths = GetStringTwoDList(key, field, null);
			List<List<T>> retVal = defaultVal;

			if (paths != null)
			{
				retVal = new List<List<T>>();
				foreach(List<string> sublist in paths)
				{
					List<T> list = new List<T>();
					foreach(string path in sublist)
					{
						if (!string.IsNullOrEmpty(path))
							list.Add(Resources.Load<T>(path));
						else
							list.Add(null);
					}
					retVal.Add(list);
				}
			}
			
			return retVal;
		}
		#endregion

		#region Get Saved Data Methods (Custom Types)
		public static T GetCustom<T>(string key, string field, T defaultVal) where T : IGDEData
		{
			T retVal = defaultVal;
			
			try
			{
				string defaultKey = (defaultVal != null)?defaultVal.Key:string.Empty;
				string customKey = GDEDataManager.GetString(key, field, defaultKey);
				if (customKey != defaultKey)
					retVal = (T)Activator.CreateInstance(typeof(T), new object[] { customKey });
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		public static List<T> GetCustomList<T>(string key, string field, List<T> defaultVal) where T : IGDEData
		{
			List<T> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					retVal = new List<T>();
					
					List<string> customDataKeys = GDEDataManager.GetStringList(key, field, null);
					
					if (customDataKeys != null)
					{
						foreach(string customDataKey in customDataKeys)
							retVal.Add((T)Activator.CreateInstance(typeof(T), new object[] { customDataKey }));
					}
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		
		public static List<List<T>> GetCustomTwoDList<T>(string key, string field, List<List<T>> defaultVal) where T : IGDEData
		{
			List<List<T>> retVal = defaultVal;
			
			try
			{
				Dictionary<string, object> dict;
				if (ModifiedData.TryGetValue(key, out dict) && dict.ContainsKey(field))
				{
					retVal = new List<List<T>>();
					
					List<List<string>> customDataKeys = GDEDataManager.GetStringTwoDList(key, field, null);
					
					if (customDataKeys != null)
					{
						foreach(var subListKeys in customDataKeys)
						{
							List<T> subList = new List<T>();
							foreach(var customDataKey in subListKeys)
								subList.Add((T)Activator.CreateInstance(typeof(T), new object[] { customDataKey }));
							retVal.Add(subList);
						}
					}
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		#endregion

		#region Save Data Methods (Basic Types)
		static void SaveValue(string key, string field, object val)
		{
			Dictionary<string, object> dict;
			if (!ModifiedData.TryGetValue(key, out dict))
				dict = new Dictionary<string, object>();
			
			dict.TryAddOrUpdateValue(field, val);
			ModifiedData.TryAddOrUpdateValue(key, dict);
		}
		
		public static void SetString(string key, string field, string val)
		{
			try 
			{
				SaveValue (key, field, val);
			}
			catch (Exception ex)
			{
				Debug.LogException (ex);
			}
		}
		
		public static void SetStringList(string key, string field, List<string> val)
		{
			try
			{
				SaveValue(key, field, val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetStringTwoDList(string key, string field, List<List<string>> val)
		{
			try
			{
				SaveValue(key, field, val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetInt(string key, string field, int val)
		{
			try
			{
				SaveValue(key, field, val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetIntList(string key, string field, List<int> val)
		{
			try
			{
				SaveValue(key, field, val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetIntTwoDList(string key, string field, List<List<int>> val)
		{
			try
			{
				SaveValue(key, field, val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetFloat(string key, string field, float val)
		{
			try
			{
				SaveValue(key, field, val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetFloatList(string key, string field, List<float> val)
		{
			try
			{
				SaveValue(key, field, val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetFloatTwoDList(string key, string field, List<List<float>> val)
		{
			try
			{
				SaveValue(key, field, val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetBool(string key, string field, bool val)
		{
			try
			{
				SaveValue(key, field, val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetBoolList(string key, string field, List<bool> val)
		{
			try
			{
				SaveValue(key, field, val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetBoolTwoDList(string key, string field, List<List<bool>> val)
		{
			try
			{
				SaveValue(key, field, val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetColor(string key, string field, Color val)
		{
			try
			{
				SaveValue(key, field, (GDEColor)val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetColorList(string key, string field, List<Color> val)
		{
			try
			{
				SaveValue(key, field, val.ConvertAll(x => (GDEColor)x));
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetColorTwoDList(string key, string field, List<List<Color>> val)
		{
			try
			{
				SaveValue(key, field, val.ConvertAll(x => x.ConvertAll(y => (GDEColor)y)));
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetVector2(string key, string field, Vector2 val)
		{
			try
			{
				SaveValue(key, field, (GDEV2)val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetVector2List(string key, string field, List<Vector2> val)
		{
			try
			{
				SaveValue(key, field, val.ConvertAll(x => (GDEV2)x));
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetVector2TwoDList(string key, string field, List<List<Vector2>> val)
		{
			try
			{
				SaveValue(key, field, val.ConvertAll(x => x.ConvertAll(y => (GDEV2)y)));
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetVector3(string key, string field, Vector3 val)
		{
			try
			{
				SaveValue(key, field, (GDEV3)val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetVector3List(string key, string field, List<Vector3> val)
		{
			try
			{
				SaveValue(key, field, val.ConvertAll(x => (GDEV3)x));
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetVector3TwoDList(string key, string field, List<List<Vector3>> val)
		{
			try
			{
				SaveValue(key, field, val.ConvertAll(x => x.ConvertAll(y => (GDEV3)y)));
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetVector4(string key, string field, Vector4 val)
		{
			try
			{
				SaveValue(key, field, (GDEV4)val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetVector4List(string key, string field, List<Vector4> val)
		{
			try
			{
				SaveValue(key, field, val.ConvertAll(x => (GDEV4)x));
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		
		public static void SetVector4TwoDList(string key, string field, List<List<Vector4>> val)
		{
			try
			{
				SaveValue(key, field, val.ConvertAll(x => x.ConvertAll(y => (GDEV4)y)));
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		#endregion

		#region Save Data Methods (Custom Types)
		public static void SetCustom<T>(string key, string field, T val) where T : IGDEData
		{
            if (val != null)
    			GDEDataManager.SetString(key, field, val.Key);
            else
                GDEDataManager.SetString(key, field, string.Empty);
		}
		
		public static void SetCustomList<T>(string key, string field, List<T> val) where T : IGDEData
		{
			List<string> customKeys = new List<string>();
            
            if (val != null)
            {
                val.ForEach(x => {
                    if (x != null)
                        customKeys.Add(x.Key);
                    else
                        customKeys.Add(string.Empty);
                });
            }
            
			GDEDataManager.SetStringList(key, field, customKeys);
		}
		
		public static void SetCustomTwoDList<T>(string key, string field, List<List<T>> val) where T : IGDEData
		{
			List<List<string>> customKeys = new List<List<string>>();
            
            if (val != null)
            {
    			foreach(List<T> subList in val)
    			{
    				List<string> subListKeys = new List<string>();
                    
                    if (subList != null)
                    {
                        subList.ForEach(x => {
                            if (x != null)
                                subListKeys.Add(x.Key);
                            else
                                subListKeys.Add(string.Empty);
                        });
                    }
                    
    				customKeys.Add(subListKeys);
    			}
            }
			GDEDataManager.SetStringTwoDList(key, field, customKeys);
		}
		#endregion

        #region Deprecated Methods
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static GameObject GetGameObject(string key, string field, GameObject defaultVal)
        {
            return GetUnityObject<GameObject>(key, field, defaultVal);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static List<GameObject> GetGameObjectList(string key, string field, List<GameObject> defaultVal)
        {
            return GetUnityObjectList<GameObject>(key, field, defaultVal);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static List<List<GameObject>> GetGameObjectTwoDList(string key, string field, List<List<GameObject>> defaultVal)
        {
            return GetUnityObjectTwoDList<GameObject>(key, field, defaultVal);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static Texture2D GetTexture2D(string key, string field, Texture2D defaultVal)
        {
            return GetUnityObject<Texture2D>(key, field, defaultVal);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static List<Texture2D> GetTexture2DList(string key, string field, List<Texture2D> defaultVal)
        {
            return GetUnityObjectList<Texture2D>(key, field, defaultVal);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static List<List<Texture2D>> GetTexture2DTwoDList(string key, string field, List<List<Texture2D>> defaultVal)
        {
            return GetUnityObjectTwoDList<Texture2D>(key, field, defaultVal);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static Material GetMaterial(string key, string field, Material defaultVal)
        {
            return GetUnityObject<Material>(key, field, defaultVal);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static List<Material> GetMaterialList(string key, string field, List<Material> defaultVal)
        {
            return GetUnityObjectList<Material>(key, field, defaultVal);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static List<List<Material>> GetMaterialTwoDList(string key, string field, List<List<Material>> defaultVal)
        {
            return GetUnityObjectTwoDList<Material>(key, field, defaultVal);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static AudioClip GetAudioClip(string key, string field, AudioClip defaultVal)
        {
            return GetUnityObject<AudioClip>(key, field, defaultVal);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static List<AudioClip> GetAudioClipList(string key, string field, List<AudioClip> defaultVal)
        {
            return GetUnityObjectList<AudioClip>(key, field, defaultVal);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static List<List<AudioClip>> GetAudioClipTwoDList(string key, string field, List<List<AudioClip>> defaultVal)
        {
            return GetUnityObjectTwoDList<AudioClip>(key, field, defaultVal);
        }

        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static void SetGameObject(string key, string field, GameObject val)
        {
            SetUnityObject(key, field, val);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static void SetGameObjectList(string key, string field, List<GameObject> val)
        {
            SetUnityObjectList(key, field, val);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static void SetGameObjectTwoDList(string key, string field, List<List<GameObject>> val)
        {
            SetUnityObjectTwoDList(key, field, val);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static void SetTexture2D(string key, string field, Texture2D val)
        {
            SetUnityObject(key, field, val);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static void SetTexture2DList(string key, string field, List<Texture2D> val)
        {
            SetUnityObjectList(key, field, val);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static void SetTexture2DTwoDList(string key, string field, List<List<Texture2D>> val)
        {
            SetUnityObjectTwoDList(key, field, val);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static void SetMaterial(string key, string field, Material val)
        {
            SetUnityObject(key, field, val);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static void SetMaterialList(string key, string field, List<Material> val)
        {
            SetUnityObjectList(key, field, val);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static void SetMaterialTwoDList(string key, string field, List<List<Material>> val)
        {
            SetUnityObjectTwoDList(key, field, val);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static void SetAudioClip(string key, string field, AudioClip val)
        {
            SetUnityObject(key, field, val);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static void SetAudioClipList(string key, string field, List<AudioClip> val)
        {
            SetUnityObjectList(key, field, val);
        }
        
        [Obsolete("The API has changed. Please regenerate your GDE Data Classes.")]
        public static void SetAudioClipTwoDList(string key, string field, List<List<AudioClip>> val)
        {
            SetUnityObjectTwoDList(key, field, val);
        }
        #endregion
	}
}
#endif