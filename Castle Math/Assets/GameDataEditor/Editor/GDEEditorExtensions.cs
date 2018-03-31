using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameDataEditor
{
  	public static class EditorGUIStyleExtensions
  	{
  		public static bool IsNullOrEmpty(this GUIStyle variable)
  		{
  			return variable == null || string.IsNullOrEmpty(variable.name);
  		}
  	}

  	public static class EditorStringExtensions
  	{
        public static void ClearCaches()
        {
            listKeyCache.Clear();
            typeKeyCache.Clear();
            schemaKeyCache.Clear();
            listDecCache.Clear();
            listTwoDDecCache.Clear();
            twoDSublistDecCache.Clear();
            twoDSublistLblDecCache.Clear();
        }
      
        static char[] dirSeparators = {'\\', '/'};
        public static string TrimLeadingDirChars(this string variable)
        {
        	return variable.TrimStart(dirSeparators);
        }

        static Dictionary<string, string> listKeyCache = new Dictionary<string, string>();
        public static string ListKey(this string variable)
        {
            if (string.IsNullOrEmpty(variable))
                return string.Empty;

            if (listKeyCache.ContainsKey(variable))
                return listKeyCache[variable];
            else
            {
                string listKey = string.Format(GDMConstants.MetaDataFormat, GDMConstants.IsListPrefix, variable);
                listKeyCache.Add(variable, listKey);
                return listKey;
            }
        }
      
        static Dictionary<string, string> typeKeyCache = new Dictionary<string, string>();
        public static string TypeKey(this string variable)
        {
          if (string.IsNullOrEmpty(variable))
            return string.Empty;
    
          if (typeKeyCache.ContainsKey(variable))
            return typeKeyCache[variable];
          else
          {
            string typeKey = string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, variable);
            typeKeyCache.Add(variable, typeKey);
            return typeKey;
          }
        }
  
        static Dictionary<string, string> schemaKeyCache = new Dictionary<string, string>();
        public static string SchemaKey(this string variable)
        {
          if (string.IsNullOrEmpty(variable))
            return string.Empty;
    
          if (schemaKeyCache.ContainsKey(variable))
            return schemaKeyCache[variable];
          else
          {
            string schemaKey = string.Format(GDMConstants.MetaDataFormat, GDMConstants.SchemaPrefix, variable);
            schemaKeyCache.Add(variable, schemaKey);
            return schemaKey;
          }
        }
  
        static Dictionary<string, string> listDecCache = new Dictionary<string, string>();
        public static string ListDeclaration(this string variable)
        {
          if (string.IsNullOrEmpty(variable))
            return string.Empty;
    
          if (listDecCache.ContainsKey(variable))
            return listDecCache[variable];
          else
          {
            string listDec = string.Format("List<{0}>", variable);
            listDecCache.Add(variable, listDec);
            return listDec;
          }
        }
  
        static Dictionary<string, string> listTwoDDecCache = new Dictionary<string, string>();
        public static string ListTwoDDeclaration(this string variable)
        {
          if (string.IsNullOrEmpty(variable))
            return string.Empty;
    
          if (listTwoDDecCache.ContainsKey(variable))
            return listTwoDDecCache[variable];
          else
          {
            string listTwoDDec = string.Format("List<List<{0}>>", variable);
            listTwoDDecCache.Add(variable, listTwoDDec);
            return listTwoDDec;
          }
        }
  
        static Dictionary<int, string> twoDSublistDecCache = new Dictionary<int, string>();
        public static string TwoDSublistDeclaration(this string variable, int i)
        {
            if (string.IsNullOrEmpty(variable))
                return string.Empty;
    
            int hash = new {variable, i}.GetHashCode();
            if (twoDSublistDecCache.ContainsKey(hash))
                return twoDSublistDecCache[hash];
            else
            {
              string dec = string.Format("[{0}]: List<{1}>", i, variable);
              twoDSublistDecCache.Add(hash, dec);
              return dec;
            }
        }
  
        static Dictionary<int, string> twoDSublistLblDecCache = new Dictionary<int, string>();
        public static string TwoDSublistLbl(this int variable, int i)
        {
            int hash = new {variable, i}.GetHashCode();
            if (twoDSublistLblDecCache.ContainsKey(hash))
                return twoDSublistLblDecCache[hash];
            else
            {
                string dec = string.Format("[{0}][{1}]:", variable, i);
                twoDSublistLblDecCache.Add(hash, dec);
                return dec;
            }
        }
      
        static Dictionary<int, string> oneDLblCache = new Dictionary<int, string>();
        public static string OneDLbl(this int i)
        {
            if (oneDLblCache.ContainsKey(i))
                return oneDLblCache[i];
            else
            {
                string lbl = string.Format("[{0}]:", i);
                oneDLblCache.Add(i, lbl);
                return lbl;
            }
        }
      
        static Dictionary<string, string> lowerCache = new Dictionary<string, string>();
        public static string Lower(this string variable)
        {
            if (string.IsNullOrEmpty(variable))
                return string.Empty;
    
            if (lowerCache.ContainsKey(variable))
                return lowerCache[variable];
            else
            {
                string lowerVer = variable.ToLower();
                lowerCache.Add(variable, lowerVer);
                return lowerVer;
            }
        }
    
        static Dictionary<string, string> fieldNameCache = new Dictionary<string, string>();
        public static string FieldName(this string variable)
        {
            if (string.IsNullOrEmpty(variable))
                return string.Empty;
    
            if (fieldNameCache.ContainsKey(variable))
                return fieldNameCache[variable];
            else
            {
                string fieldName = variable.Replace(GDMConstants.TypePrefix, string.Empty);
                fieldNameCache.Add(variable, fieldName);
                return fieldName;
            }
        }
    
        static Dictionary<Enum, string> enumStringCache = new Dictionary<Enum, string>();
        public static string EnumToString(this Enum variable)
        {
            if (enumStringCache.ContainsKey(variable))
                return enumStringCache[variable];
            else
            {
                string val = variable.ToString();
                enumStringCache.Add(variable, val);
                return val;
            }
        }
    
        static Dictionary<int, string> foldoutCache = new Dictionary<int, string>();
        public static string FoldoutKey(this string itemKey, string fieldKey)
        {
            int hash = new {itemKey, fieldKey}.GetHashCode();
            if (foldoutCache.ContainsKey(hash))
                return foldoutCache[hash];
            else
            {
                string val = string.Format(GDMConstants.MetaDataFormat, itemKey, fieldKey);
                foldoutCache.Add(hash, val);
                return val;
            }
        }
        public static string ListCountKey(this string itemKey, string fieldKey)
        {
            return itemKey.FoldoutKey(fieldKey);
        }
        public static string EditFieldKey(this string schemaKey, string fieldKey)
        {
            return schemaKey.FoldoutKey(fieldKey);
        }
    
        static Dictionary<int, string> previewKeyCache = new Dictionary<int, string>();
        public static string PreviewKey(this string schemaKey, string itemKey, string fieldKey)
        {
          int hash = new {schemaKey, itemKey, fieldKey}.GetHashCode();
          if (previewKeyCache.ContainsKey(hash))
              return previewKeyCache[hash];
          else
          {
              string val = schemaKey+"_"+itemKey+"_"+fieldKey;
              previewKeyCache.Add(hash, val);
              return val;
          }
      }
      
        const string highLightFormat = "{0}<color={1}>{2}</color>{3}";
        const string schemaLbl = "Schema:";
        const string blankLbl = "       ";
        static Dictionary<int, string> highlightCache = new Dictionary<int, string>();
        /// <summary>
        /// Returns a new string that hightlights the first instance of substring with html color tag
        /// Ex. "The sky is <color=blue>blue</color>!"
        /// Only supported in Unity 4.0+
        /// </summary>
        /// <returns>A new string formatted with the color tag around the first instance of substring.</returns>
        /// <param name="substring">Substring to highlight</param>
        /// <param name="color">Color to specify in the color tag</param>
        public static string HighlightSubstring (this string variable, string substring, string color)
        {
          string highlightedString = variable;

          if (!string.IsNullOrEmpty(substring) && variable.IndexOf(substring) > -1)
          {
              int hash = new {variable, substring, color}.GetHashCode();
              if (highlightCache.ContainsKey(hash))
                return highlightCache[hash];
              else
              {
                  int index = variable.Replace(schemaLbl, blankLbl).IndexOf(substring, StringComparison.CurrentCultureIgnoreCase);

                  if (index != -1)
                  {
                      highlightedString = string.Format(highLightFormat,
                                                        variable.Substring(0, index), color, 
                                                        variable.Substring(index, substring.Length), 
                                                        variable.Substring(index + substring.Length));
                      highlightCache.Add(hash, highlightedString);
                  }
              }
          }

          return highlightedString;
      }
  	}

    public static class EditorDictionaryExtensions
    {
        public static List<bool> ToBoolList(this IList variable)
        {
            var result = new List<bool>();
            try
            {
                for(int x=0; x<variable.Count;  x++)
                    result.Add(Convert.ToBoolean(variable[x]));
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<List<bool>> ToBoolTwoDList(this IList variable)
        {
            var result = new List<List<bool>>();
            try
            {
                for(int x=0; x<variable.Count;  x++)
                {
                    IList sublist = variable[x] as IList;
                    List<bool> newSublist = new List<bool>();

                    for(int y=0;  y<sublist.Count;  y++)
                        newSublist.Add(Convert.ToBoolean(sublist[y]));

                    result.Add(newSublist);
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<int> ToIntList(this IList variable)
        {
            var result = new List<int>();
            try
            {
                for(int x=0; x<variable.Count;  x++)
                    result.Add(Convert.ToInt32(variable[x]));
            }
            catch
            {

            }

            return result;
        }

        public static List<List<int>> ToIntTwoDList(this IList variable)
        {
            var result = new List<List<int>>();
            try
            {
                for(int x=0; x<variable.Count;  x++)
                {
                    IList sublist = variable[x] as IList;
                    List<int> newSublist = new List<int>();
                    
                    for(int y=0;  y<sublist.Count;  y++)
                        newSublist.Add(Convert.ToInt32(sublist[y]));
                    
                    result.Add(newSublist);
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<float> ToFloatList(this IList variable)
        {
            var result = new List<float>();
            try
            {
                for(int x=0; x<variable.Count;  x++)
                    result.Add(Convert.ToSingle(variable[x]));
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<List<float>> ToFloatTwoDList(this IList variable)
        {
            var result = new List<List<float>>();
            try
            {
                for(int x=0; x<variable.Count;  x++)
                {
                    IList sublist = variable[x] as IList;
                    List<float> newSublist = new List<float>();
                    
                    for(int y=0;  y<sublist.Count;  y++)
                        newSublist.Add(Convert.ToSingle(sublist[y]));
                    
                    result.Add(newSublist);
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<string> ToStringList(this IList variable)
        {
            var result = new List<string>();
            try
            {
                for(int x=0;  x<variable.Count;  x++)
                    result.Add(variable[x].ToString());
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<List<string>> ToStringTwoDList(this IList variable)
        {
            var result = new List<List<string>>();
            try
            {
                for(int x=0; x<variable.Count;  x++)
                {
                    IList sublist = variable[x] as IList;
                    List<string> newSublist = new List<string>();
                    
                    for(int y=0;  y<sublist.Count;  y++)
                        newSublist.Add(sublist[y].ToString());
                    
                    result.Add(newSublist);
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static Vector2 ToVector2(this Dictionary<string, object> variable)
        {
            Vector2 result = Vector2.zero;
            try
            {
                if (variable != null)
                {
                    result = new Vector2(Convert.ToSingle(variable["x"]),
                                         Convert.ToSingle(variable["y"]));
                }
            }
            catch
            {

            }

            return result;
        }

        public static List<Vector2> ToVector2List(this IList variable)
        {
            List<Vector2> result = new List<Vector2>();

            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var dict = entry as Dictionary<string, object>;
                        result.Add(dict.ToVector2());
                    }
                }
            }
            catch
            {

            }

            return result;
        }

        public static List<List<Vector2>> ToVector2TwoDList(this IList variable)
        {
            List<List<Vector2>> result = new List<List<Vector2>>();

            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var sublist = entry as IList;
                        result.Add(sublist.ToVector2List());
                    }
                }
            }
            catch
            {

            }

            return result;
        }

        public static Vector3 ToVector3(this Dictionary<string, object> variable)
        {
            Vector3 result = Vector3.zero;
            try
            {
                if (variable != null)
                {
                    result = new Vector3(Convert.ToSingle(variable["x"]),
                                         Convert.ToSingle(variable["y"]),
                                         Convert.ToSingle(variable["z"]));
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<Vector3> ToVector3List(this IList variable)
        {
            List<Vector3> result = new List<Vector3>();
            
            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var dict = entry as Dictionary<string, object>;
                        result.Add(dict.ToVector3());
                    }
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<List<Vector3>> ToVector3TwoDList(this IList variable)
        {
            List<List<Vector3>> result = new List<List<Vector3>>();
            
            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var sublist = entry as IList;
                        result.Add(sublist.ToVector3List());
                    }
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static Vector4 ToVector4(this Dictionary<string, object> variable)
        {
            Vector4 result = Vector4.zero;
            try
            {
                if (variable != null)
                {
                    result = new Vector4(Convert.ToSingle(variable["x"]),
                                         Convert.ToSingle(variable["y"]),
                                         Convert.ToSingle(variable["z"]),
                                         Convert.ToSingle(variable["w"]));
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<Vector4> ToVector4List(this IList variable)
        {
            List<Vector4> result = new List<Vector4>();
            
            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var dict = entry as Dictionary<string, object>;
                        result.Add(dict.ToVector4());
                    }
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<List<Vector4>> ToVector4TwoDList(this IList variable)
        {
            List<List<Vector4>> result = new List<List<Vector4>>();
            
            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var sublist = entry as IList;
                        result.Add(sublist.ToVector4List());
                    }
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static Color ToColor(this Dictionary<string, object> variable)
        {
            Color result = Color.black;
            try
            {
                if (variable != null)
                {
                    result = new Vector4(Convert.ToSingle(variable["r"]),
                                         Convert.ToSingle(variable["g"]),
                                         Convert.ToSingle(variable["b"]),
                                         Convert.ToSingle(variable["a"]));
                }
            }
            catch
            {
                
            }
            
            return result;
        }
        
        public static List<Color> ToColorList(this IList variable)
        {
            List<Color> result = new List<Color>();
            
            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var dict = entry as Dictionary<string, object>;
                        result.Add(dict.ToColor());
                    }
                }
            }
            catch
            {
                
            }
            
            return result;
        }

        public static List<List<Color>> ToColorTwoDList(this IList variable)
        {
            List<List<Color>> result = new List<List<Color>>();
            
            try
            {
                if (variable != null)
                {
                    foreach(var entry in variable)
                    {
                        var sublist = entry as IList;
                        result.Add(sublist.ToColorList());
                    }
                }
            }
            catch
            {
                
            }
            
            return result;
        }
    }
}