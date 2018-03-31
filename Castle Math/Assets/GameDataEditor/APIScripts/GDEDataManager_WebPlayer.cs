#if UNITY_WEBPLAYER
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
		public static void ResetToDefault(string itemName, string fieldName)
		{
			PlayerPrefs.DeleteKey(itemName + "_" + fieldName);
		}
		#endregion

		#region Persistent Data Methods
		public static void Save() {}
		public static void ClearSaved() {}
        public static void RegisterItem(string schema, string key) {}
		#endregion

		#region Get Saved Data Methods (Basic Types)
		public static string GetString(string key, string field, string defaultVal)
		{
			string retVal = defaultVal;
			
			try
			{
				retVal = PlayerPrefs.GetString(key+"_"+field, retVal);
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
				retVal = GDEPPX.GetStringList(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.Get2DStringList(key+"_"+field, defaultVal);
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
				retVal = PlayerPrefs.GetInt(key+"_"+field, retVal);
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
				retVal = GDEPPX.GetIntList(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.Get2DIntList(key+"_"+field, defaultVal);
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
				retVal = PlayerPrefs.GetFloat(key+"_"+field, retVal);
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
				retVal = GDEPPX.GetFloatList(key+"_"+field, retVal);
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
				retVal = GDEPPX.Get2DFloatList(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.GetBool(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.GetBoolList(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.Get2DBoolList(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.GetColor(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.GetColorList(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.Get2DColorList(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.GetVector2(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.GetVector2List(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.Get2DVector2List(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.GetVector3(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.GetVector3List(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.Get2DVector3List(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.GetVector4(key+"_"+field, defaultVal);  
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
				retVal = GDEPPX.GetVector4List(key+"_"+field, defaultVal);
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
				retVal = GDEPPX.Get2DVector4List(key+"_"+field, defaultVal);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			
			return retVal;
		}
		#endregion

		#region Save Data Methods (Basic Types)
		public static void SetString(string key, string field, string val)
		{
			try 
			{
				PlayerPrefs.SetString(key+"_"+field, val);
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
				GDEPPX.SetStringList(key+"_"+field, val);
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
				GDEPPX.Set2DStringList(key+"_"+field, val);
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
				PlayerPrefs.SetInt(key+"_"+field, val);
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
				GDEPPX.SetIntList(key+"_"+field, val);
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
				GDEPPX.Set2DIntList(key+"_"+field, val);
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
				PlayerPrefs.SetFloat(key+"_"+field, val);
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
				GDEPPX.SetFloatList(key+"_"+field, val);
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
				GDEPPX.Set2DFloatList(key+"_"+field, val);
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
				GDEPPX.SetBool(key+"_"+field, val);
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
				GDEPPX.SetBoolList(key+"_"+field, val);
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
				GDEPPX.Set2DBoolList(key+"_"+field, val);
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
				GDEPPX.SetColor(key+"_"+field, val);
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
				GDEPPX.SetColorList(key+"_"+field, val);
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
				GDEPPX.Set2DColorList(key+"_"+field, val);
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
				GDEPPX.SetVector2(key+"_"+field, val);
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
				GDEPPX.SetVector2List(key+"_"+field, val);
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
				GDEPPX.Set2DVector2List(key+"_"+field, val);
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
				GDEPPX.SetVector3(key+"_"+field, val);
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
				GDEPPX.SetVector3List(key+"_"+field, val);
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
				GDEPPX.Set2DVector3List(key+"_"+field, val);
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
				GDEPPX.SetVector4(key+"_"+field, val);
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
				GDEPPX.SetVector4List(key+"_"+field, val);
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
				GDEPPX.Set2DVector4List(key+"_"+field, val);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
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
				if (PlayerPrefs.HasKey(key+"_"+field))
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
				if (PlayerPrefs.HasKey(key+"_"+field))
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
		
		#region Save Data Methods (Custom Types)
		public static void SetCustom<T>(string key, string field, T val) where T : IGDEData
		{
			GDEDataManager.SetString(key, field, val.Key);
		}
		
		public static void SetCustomList<T>(string key, string field, List<T> val) where T : IGDEData
		{
			List<string> customKeys = new List<string>();
			val.ForEach(x => customKeys.Add(x.Key));
			GDEDataManager.SetStringList(key, field, customKeys);
		}
		
		public static void SetCustomTwoDList<T>(string key, string field, List<List<T>> val) where T : IGDEData
		{
			List<List<string>> customKeys = new List<List<string>>();
			foreach(List<T> subList in val)
			{
				List<string> subListKeys = new List<string>();
				subList.ForEach(x => subListKeys.Add(x.Key));
				customKeys.Add(subListKeys);
			}
			GDEDataManager.SetStringTwoDList(key, field, customKeys);
		}
		#endregion

        #region Get Saved Data Methods (Unity Types)
        public static T GetUnityObject<T>(string key, string field, T defaultVal) where T : UnityEngine.Object
        {
            T retVal = defaultVal;
            
            try
            {
                retVal = GDEPPX.GetUnityObject(key+"_"+field, defaultVal);  
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
            return retVal;
        }
        
        public static List<T> GetUnityObjectList<T>(string key, string field, List<T> defaultVal = null) where T : UnityEngine.Object
        {
            List<T> retVal = defaultVal;
            
            try
            {
                retVal = GDEPPX.GetUnityObjectList(key+"_"+field, defaultVal);  
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
            return retVal;
        }
        
        public static List<List<T>> GetUnityObjectTwoDList<T>(string key, string field, List<List<T>> defaultVal = null) where T : UnityEngine.Object
        {
            List<List<T>> retVal = defaultVal;
            
            try
            {
                retVal = GDEPPX.Get2DUnityObjectList(key+"_"+field, defaultVal);  
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
            return retVal;
        }
        #endregion

        #region Deprecated Methods
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static GameObject GetGameObject(string key, string field, GameObject defaultVal)
        {
            GameObject retVal = defaultVal;
            
            try
            {
                retVal = GDEPPX.GetUnityObject(key+"_"+field, defaultVal);  
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
            return retVal;
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static List<GameObject> GetGameObjectList(string key, string field, List<GameObject> defaultVal = null)
        {
            List<GameObject> retVal = defaultVal;
            
            try
            {
                retVal = GDEPPX.GetUnityObjectList(key+"_"+field, defaultVal);  
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
            return retVal;
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static List<List<GameObject>> GetGameObjectTwoDList(string key, string field, List<List<GameObject>> defaultVal = null)
        {
            List<List<GameObject>> retVal = defaultVal;
            
            try
            {
                retVal = GDEPPX.Get2DUnityObjectList(key+"_"+field, defaultVal);  
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
            return retVal;
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static Texture2D GetTexture2D(string key, string field, Texture2D defaultVal)
        {
            Texture2D retVal = defaultVal;
            
            try
            {
                retVal = GDEPPX.GetUnityObject(key+"_"+field, defaultVal);  
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
            return retVal;
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static List<Texture2D> GetTexture2DList(string key, string field, List<Texture2D> defaultVal = null)
        {
            List<Texture2D> retVal = defaultVal;
            
            try
            {
                retVal = GDEPPX.GetUnityObjectList(key+"_"+field, defaultVal);  
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
            return retVal;
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static List<List<Texture2D>> GetTexture2DTwoDList(string key, string field, List<List<Texture2D>> defaultVal = null)
        {
            List<List<Texture2D>> retVal = defaultVal;
            
            try
            {
                retVal = GDEPPX.Get2DUnityObjectList(key+"_"+field, defaultVal);  
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
            return retVal;
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static Material GetMaterial(string key, string field, Material defaultVal)
        {
            Material retVal = defaultVal;
            
            try
            {
                retVal = GDEPPX.GetUnityObject(key+"_"+field, defaultVal);  
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
            return retVal;
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static List<Material> GetMaterialList(string key, string field, List<Material> defaultVal = null)
        {
            List<Material> retVal = defaultVal;
            
            try
            {
                retVal = GDEPPX.GetUnityObjectList(key+"_"+field, defaultVal);  
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
            return retVal;
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static List<List<Material>> GetMaterialTwoDList(string key, string field, List<List<Material>> defaultVal = null)
        {
            List<List<Material>> retVal = defaultVal;
            
            try
            {
                retVal = GDEPPX.Get2DUnityObjectList(key+"_"+field, defaultVal);  
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
            return retVal;
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static AudioClip GetAudioClip(string key, string field, AudioClip defaultVal)
        {
            AudioClip retVal = defaultVal;
            
            try
            {
                retVal = GDEPPX.GetUnityObject(key+"_"+field, defaultVal);  
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
            return retVal;
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static List<AudioClip> GetAudioClipList(string key, string field, List<AudioClip> defaultVal = null)
        {
            List<AudioClip> retVal = defaultVal;
            
            try
            {
                retVal = GDEPPX.GetUnityObjectList(key+"_"+field, defaultVal);  
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
            return retVal;
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static List<List<AudioClip>> GetAudioClipTwoDList(string key, string field, List<List<AudioClip>> defaultVal = null)
        {
            List<List<AudioClip>> retVal = defaultVal;
            
            try
            {
                retVal = GDEPPX.Get2DUnityObjectList(key+"_"+field, defaultVal);  
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
            return retVal;
        }

        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static void SetGameObject(string key, string field, GameObject val)
        {
            try
            {
                GDEPPX.SetUnityObject(key+"_"+field, val);
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static void SetGameObjectList(string key, string field, List<GameObject> val)
        {
            try
            {
                GDEPPX.SetUnityObjectList(key+"_"+field, val);
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static void SetGameObjectTwoDList(string key, string field, List<List<GameObject>> val)
        {
            try
            {
                GDEPPX.SetUnityObject2DList(key+"_"+field, val);
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static void SetTexture2D(string key, string field, Texture2D val)
        {
            try
            {
                GDEPPX.SetUnityObject(key+"_"+field, val);
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static void SetTexture2DList(string key, string field, List<Texture2D> val)
        {
            try
            {
                GDEPPX.SetUnityObjectList(key+"_"+field, val);
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static void SetTexture2DTwoDList(string key, string field, List<List<Texture2D>> val)
        {
            try
            {
                GDEPPX.SetUnityObject2DList(key+"_"+field, val);
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static void SetMaterial(string key, string field, Material val)
        {
            try
            {
                GDEPPX.SetUnityObject(key+"_"+field, val);
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static void SetMaterialList(string key, string field, List<Material> val)
        {
            try
            {
                GDEPPX.SetUnityObjectList(key+"_"+field, val);
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static void SetMaterialTwoDList(string key, string field, List<List<Material>> val)
        {
            try
            {
                GDEPPX.SetUnityObject2DList(key+"_"+field, val);
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static void SetAudioClip(string key, string field, AudioClip val)
        {
            try
            {
                GDEPPX.SetUnityObject(key+"_"+field, val);
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static void SetAudioClipList(string key, string field, List<AudioClip> val)
        {
            try
            {
                GDEPPX.SetUnityObjectList(key+"_"+field, val);
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        [Obsolete("This method is obsolete. Please regenerate your GDE Data classes.")]
        public static void SetAudioClipTwoDList(string key, string field, List<List<AudioClip>> val)
        {
            try
            {
                GDEPPX.SetUnityObject2DList(key+"_"+field, val);
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        #endregion
	}
}
#endif