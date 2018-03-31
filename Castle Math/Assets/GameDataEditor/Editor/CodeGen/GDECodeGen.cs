using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace GameDataEditor
{
    public class GDECodeGen
    {
        public static void GenClasses(Dictionary<string, Dictionary<string, object>> allSchemas)
        {
            foreach (KeyValuePair<string, Dictionary<string, object>> pair in allSchemas)
            {
                GenClass(pair.Key, pair.Value);
            }
        }

		public static void GenStaticKeysClass(Dictionary<string, Dictionary<string, object>> allSchemas)
		{
			string fileName = string.Format(GDECodeGenConstants.StaticItemKeysFileName, Path.GetFileNameWithoutExtension(GDESettings.Instance.DataFilePath));
			Debug.Log(GDEConstants.GeneratingLbl + " " + fileName);

			StringBuilder sb = new StringBuilder();

			sb.AppendFormat(GDECodeGenConstants.AutoGenMsg, GDESettings.Instance.DataFilePath);
            sb.Append("\n");
			sb.Append(GDECodeGenConstants.StaticItemKeyClassHeader);

			foreach (KeyValuePair<string, Dictionary<string, object>> pair in allSchemas)
			{
				string schema = pair.Key;

				List<string> items = GDEItemManager.GetItemsOfSchemaType(schema);
				foreach(string item in items)
				{
					sb.Append("\n");
					sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));
					sb.AppendFormat(GDECodeGenConstants.StaticItemKeyFormat, schema, item);
				}
			}

			sb.Append("\n");
			sb.Append("}".PadLeft(GDECodeGenConstants.IndentLevel1+1));
			sb.Append("\n");
			sb.Append("}");
			sb.Append("\n");

			WriteFile(sb, fileName);
		}

		public static void GenStaticSchemaKeysClass(Dictionary<string, Dictionary<string, object>> allSchemas)
		{
			string fileName = string.Format(GDECodeGenConstants.StaticSchemaKeysFileName, Path.GetFileNameWithoutExtension(GDESettings.Instance.DataFilePath));
			Debug.Log(GDEConstants.GeneratingLbl + " " + fileName);

			StringBuilder sb = new StringBuilder();
			
			sb.AppendFormat(GDECodeGenConstants.AutoGenMsg, GDESettings.Instance.DataFilePath);
			sb.Append("\n");
			sb.Append(GDECodeGenConstants.StaticSchemaKeyClassHeader);
			
			foreach (var schemaName in allSchemas.Keys.ToList())
			{
				sb.Append("\n");
				sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));
				sb.AppendFormat(GDECodeGenConstants.StaticSchemaKeyFormat, schemaName);
			}
			
			sb.Append("\n");
			sb.Append("}".PadLeft(GDECodeGenConstants.IndentLevel1+1));
			sb.Append("\n");
			sb.Append("}");
			sb.Append("\n");
			
			WriteFile(sb, fileName);
		}

        static void GenClass(string schemaKey, Dictionary<string, object> schemaData)
        {
            StringBuilder sb = new StringBuilder();
            string className = string.Format(GDECodeGenConstants.DataClassNameFormat, schemaKey);
            string fileName = string.Format(GDECodeGenConstants.ClassFileNameFormat, className);
            Debug.Log(GDEConstants.GeneratingLbl + " " + fileName);

            // Add the auto generated comment at the top of the file
            sb.AppendFormat(GDECodeGenConstants.AutoGenMsg, GDESettings.Instance.DataFilePath);
            sb.Append("\n");

            // Append all the using statements
            sb.Append(GDECodeGenConstants.DataClassHeader);
            sb.Append("\n");

            // Append the class declaration
            sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel1));
            sb.AppendFormat(GDECodeGenConstants.ClassDeclarationFormat, className);
            sb.Append("\n");
            sb.Append("{".PadLeft(GDECodeGenConstants.IndentLevel1+1));
            sb.Append("\n");

            // Append all the data variables
			AppendVariableDeclarations(sb, schemaKey, schemaData);
            sb.Append("\n");

			// Append constructors
			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));
			sb.AppendFormat(GDECodeGenConstants.ClassConstructorsFormat, className);
			sb.Append("\n");

			// Append the save to dict method
			AppendSaveToDictMethod(sb, schemaKey, schemaData);
			sb.Append("\n");

            // Append the save to dict method
            AppendUpdateCustomItemsMethod(sb, schemaKey, schemaData);
            sb.Append("\n");

            // Append the load from dict method
            AppendLoadDictMethod(sb, schemaKey, schemaData);
            sb.Append("\n");

			// Append the load from saved data method
			AppendLoadFromSavedMethod(sb, schemaKey, schemaData);
			sb.Append("\n");

			// Append the shallow clone method
			AppendShallowCloneMethod(sb, schemaKey, schemaData);
			sb.Append("\n");

			// Append the deep clone method
			AppendDeepCloneMethod(sb, schemaKey, schemaData);
			sb.Append("\n");

            // Append the reset variable methods
            AppendResetVariableMethods(sb, schemaKey, schemaData);
            sb.Append("\n");

            // Append the reset all method
            AppendResetAllMethod(sb, schemaKey, schemaData);
            sb.Append("\n");

            // Append the close class brace
            sb.Append("}".PadLeft(GDECodeGenConstants.IndentLevel1+1));
            sb.Append("\n");

            // Append the close namespace brace
            sb.Append("}");
            sb.Append("\n");

			WriteFile(sb, fileName);
        }

		static void WriteFile(StringBuilder sb, string fileName)
		{
			string fullPath = string.Empty;
			var results = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(fileName) + " t:Script");
			if (results != null && results.Length > 0)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(results[0]);
				fullPath = Path.Combine(Environment.CurrentDirectory, assetPath);
			}
			else
				fullPath = Path.Combine(GDESettings.FullRootDir, GDECodeGenConstants.ClassFileDefaultPath + fileName);

			File.WriteAllText(fullPath, sb.ToString());
			Debug.Log(GDEConstants.DoneGeneratingLbl + " " + fileName);
		}

        static void AppendVariableDeclarations(StringBuilder sb, string schemaKey, Dictionary<string, object> schemaData)
        {
            bool didAppendSpaceForSection = false;
            bool shouldAppendSpace = false;
			bool isFirstSection = true;

			string variableType;

            // Append all the single variables first
            foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
            {
				variableType = GDEItemManager.GetVariableTypeFor(fieldType);

                List<string> fieldKeys = GDEItemManager.SchemaFieldKeysOfType(schemaKey, fieldType.ToString(), 0);
                foreach(string fieldKey in fieldKeys)
                {
                    if (shouldAppendSpace)
                        sb.Append("\n");

                    AppendVariable(sb, variableType, fieldKey);
                    shouldAppendSpace = true;
					isFirstSection = false;
                }
            }

            // Append the custom types
            foreach(string fieldKey in GDEItemManager.SchemaCustomFieldKeys(schemaKey, 0))
            {
                if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
                {
                    sb.Append("\n");
                }

                schemaData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out variableType);
                variableType = string.Format(GDECodeGenConstants.DataClassNameFormat, variableType);
                AppendCustomVariable(sb, variableType, fieldKey);

                shouldAppendSpace = true;
				isFirstSection = false;
				didAppendSpaceForSection = true;
            }
            didAppendSpaceForSection = false;

            // Append the basic lists
			for(int dimension=1;  dimension <=2;  dimension++)
			{
	            foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
	            {
	                List<string> fieldKeys = GDEItemManager.SchemaFieldKeysOfType(schemaKey, fieldType.ToString(), dimension);
					variableType = GDEItemManager.GetVariableTypeFor(fieldType);

	                foreach(string fieldKey in fieldKeys)
	                {
	                    if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
	                    {
	                        sb.Append("\n");
	                    }

	                    AppendListVariable(sb, variableType, fieldKey, dimension);

	                    shouldAppendSpace = true;
						didAppendSpaceForSection = true;
						isFirstSection = false;
	                }
	            }
	            didAppendSpaceForSection = false;
			}

            // Append the custom lists
			for(int dimension = 1;  dimension <= 2;  dimension++)
			{
	            foreach(string fieldKey in GDEItemManager.SchemaCustomFieldKeys(schemaKey, dimension))
	            {
	                if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
	                {
	                    sb.Append("\n");
	                }

	                schemaData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out variableType);
	                variableType = string.Format(GDECodeGenConstants.DataClassNameFormat, variableType);
	                AppendCustomListVariable(sb, variableType, fieldKey, dimension);

	                shouldAppendSpace = true;
					isFirstSection = false;
					didAppendSpaceForSection = true;
	            }
			}
        }

		static void AppendSaveToDictMethod(StringBuilder sb, string schemaKey, Dictionary<string, object> schemaData)
		{
			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));
			sb.AppendFormat(GDECodeGenConstants.SaveToDictMethod, schemaKey);
			sb.Append("\n");

			bool shouldAppendSpace = false;
			bool didAppendSpaceForSection = false;
			bool isFirstSection = true;
			
			// Append the basic types
			for(int dimension = 0;  dimension <= 2;  dimension++)
			{
				foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
				{
					List<string> fieldKeys = GDEItemManager.SchemaFieldKeysOfType(schemaKey, fieldType.ToString(), dimension);

					foreach(string fieldKey in fieldKeys)
					{
						if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
						{
							sb.Append("\n");
						}
						
						AppendSaveToDictVariable(sb, fieldKey);
						shouldAppendSpace = true;
						didAppendSpaceForSection = true;
						isFirstSection = false;
					}
				}
				didAppendSpaceForSection = false;
			}
			
			// Append the custom lists
			for(int dimension = 0;  dimension <= 2;  dimension++)
			{
				foreach(string fieldKey in GDEItemManager.SchemaCustomFieldKeys(schemaKey, dimension))
				{
					if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
					{
						sb.Append("\n");
					}

					AppendSaveToDictVariable(sb, fieldKey);
					
					shouldAppendSpace = true;
					isFirstSection = false;
					didAppendSpaceForSection = true;
				}
			}
			
			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
			sb.Append(GDECodeGenConstants.SaveToDictEnd);
			sb.Append("\n");
		}

        static void AppendUpdateCustomItemsMethod(StringBuilder sb, string schemaKey, Dictionary<string, object> schemaData)
        {
            sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));
            sb.AppendFormat(GDECodeGenConstants.UpdateCustomItemsMethod, schemaKey);
            sb.Append("\n");
            
            bool shouldAppendSpace = false;
            bool didAppendSpaceForSection = false;
            bool isFirstSection = true;
            
            // Append the custom lists
            for(int dimension = 0;  dimension <= 2;  dimension++)
            {
                foreach(string fieldKey in GDEItemManager.SchemaCustomFieldKeys(schemaKey, dimension))
                {
                    if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
                    {
                        sb.Append("\n");
                    }
                    
                    AppendUpdateCustomItemVariable(sb, fieldKey, dimension);
                    
                    shouldAppendSpace = true;
                    isFirstSection = false;
                    didAppendSpaceForSection = true;
                }
            }
            
            sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));
            sb.Append(GDECodeGenConstants.UpdateCustomItemsEnd);
            sb.Append("\n");
        }

        static void AppendLoadDictMethod(StringBuilder sb, string schemaKey, Dictionary<string, object> schemaData)
        {
            sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));
            sb.Append(GDECodeGenConstants.LoadDictMethod);
            sb.Append("\n");

            bool shouldAppendSpace = false;
            bool didAppendSpaceForSection = false;
			bool isFirstSection = true;

            string variableType;

            // Append all the single variables first
            foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
            {
                variableType = fieldType.ToString();
                List<string> fieldKeys = GDEItemManager.SchemaFieldKeysOfType(schemaKey, fieldType.ToString(), 0);
                foreach(string fieldKey in fieldKeys)
                {
                    AppendLoadVariable(sb, variableType, fieldKey);
                    shouldAppendSpace = true;
					isFirstSection = false;
                }
            }

            // Append the custom types
            bool appendTempKeyDeclaration = true;
            foreach(string fieldKey in GDEItemManager.SchemaCustomFieldKeys(schemaKey, 0))
            {
                if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
                {
                    sb.Append("\n");
                }

                schemaData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out variableType);
                variableType = string.Format(GDECodeGenConstants.DataClassNameFormat, variableType);

                if (appendTempKeyDeclaration)
                {
                    sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel4));
                    sb.Append(GDECodeGenConstants.TempStringKeyDeclaration);
                    sb.Append("\n");
                    appendTempKeyDeclaration = false;
                }

                AppendLoadCustomVariable(sb, variableType, fieldKey);
                shouldAppendSpace = true;
				didAppendSpaceForSection = true;
				isFirstSection = false;
            }
            didAppendSpaceForSection = false;

            // Append the basic lists
			for(int dimension = 1;  dimension <= 2;  dimension++)
			{
	            foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
	            {
	                List<string> fieldKeys = GDEItemManager.SchemaFieldKeysOfType(schemaKey, fieldType.ToString(), dimension);
	                variableType = fieldType.ToString();

	                foreach(string fieldKey in fieldKeys)
	                {
	                    if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
	                    {
	                        sb.Append("\n");
	                    }

	                    AppendLoadListVariable(sb, variableType, fieldKey, dimension);
	                    shouldAppendSpace = true;
						didAppendSpaceForSection = true;
						isFirstSection = false;
	                }
	            }
	            didAppendSpaceForSection = false;
			}

            // Append the custom lists
			for(int dimension = 1;  dimension <= 2;  dimension++)
			{
	            foreach(string fieldKey in GDEItemManager.SchemaCustomFieldKeys(schemaKey, dimension))
	            {
	                if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
	                {
	                    sb.Append("\n");
	                }

	                schemaData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out variableType);
					variableType = "Custom";
	                AppendLoadListVariable(sb, variableType, fieldKey, dimension);

					shouldAppendSpace = true;
					isFirstSection = false;
					didAppendSpaceForSection = true;
	            }
			}

			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel4));
			sb.Append(GDECodeGenConstants.LoadDictMethodEnd);
            sb.Append("\n");
        }

		static void AppendLoadFromSavedMethod(StringBuilder sb, string schemaKey, Dictionary<string, object> schemaData)
		{
			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));
			sb.Append(GDECodeGenConstants.LoadFromSavedMethod);
			sb.Append("\n");

			bool shouldAppendSpace = false;
			bool didAppendSpaceForSection = false;
			bool isFirstSection = true;

			string variableType;

			// Append all the single variables first
			foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
			{
				variableType = fieldType.ToString();
				List<string> fieldKeys = GDEItemManager.SchemaFieldKeysOfType(schemaKey, fieldType.ToString(), 0);
				foreach(string fieldKey in fieldKeys)
				{
					AppendLoadFromSavedVariable(sb, variableType, fieldKey);
					shouldAppendSpace = true;
					isFirstSection = false;
				}
			}

			// Append the custom types
			foreach(string fieldKey in GDEItemManager.SchemaCustomFieldKeys(schemaKey, 0))
			{
				if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
				{
					sb.Append("\n");
				}

				schemaData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out variableType);
				variableType = "Custom";

				AppendLoadFromSavedCustomVariable(sb, variableType, fieldKey);
				shouldAppendSpace = true;
				didAppendSpaceForSection = true;
				isFirstSection = false;
			}
			didAppendSpaceForSection = false;

			// Append the basic lists
			for(int dimension = 1;  dimension <= 2;  dimension++)
			{
				foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
				{
					List<string> fieldKeys = GDEItemManager.SchemaFieldKeysOfType(schemaKey, fieldType.ToString(), dimension);
					variableType = fieldType.ToString();

					foreach(string fieldKey in fieldKeys)
					{
						if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
						{
							sb.Append("\n");
						}

						AppendLoadSavedListVariable(sb, variableType, fieldKey, dimension);
						shouldAppendSpace = true;
						didAppendSpaceForSection = true;
						isFirstSection = false;
					}
				}
				didAppendSpaceForSection = false;
			}

			// Append the custom lists
			for(int dimension = 1;  dimension <= 2;  dimension++)
			{
				foreach(string fieldKey in GDEItemManager.SchemaCustomFieldKeys(schemaKey, dimension))
				{
					if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
					{
						sb.Append("\n");
					}

					schemaData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out variableType);
					variableType = "Custom";
					AppendLoadSavedListVariable(sb, variableType, fieldKey, dimension);

					shouldAppendSpace = true;
					isFirstSection = false;
					didAppendSpaceForSection = true;
				}
			}

			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));
			sb.Append(GDECodeGenConstants.LoadFromSavedMethodEnd);
			sb.Append("\n");
		}

		static void AppendShallowCloneMethod(StringBuilder sb, string schemaKey, Dictionary<string, object> schemaData)
		{
			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));
			sb.AppendFormat(GDECodeGenConstants.ShallowCloneMethodStart, schemaKey);
			sb.Append("\n\n");
			
			bool shouldAppendSpace = false;
			bool didAppendSpaceForSection = false;
			bool isFirstSection = true;
			bool didGetDict = false;
			bool isUnityType = false;
			bool didBasePathVar = false;
			bool didListPathVar = false;
			bool did2dListPathVar = false;

			// Base Types
			foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
			{
				List<string> fieldKeys = GDEItemManager.SchemaFieldKeysOfType(schemaKey, fieldType.ToString(), 0);
				foreach(string fieldKey in fieldKeys)
				{
					isUnityType = GDEItemManager.IsUnityType(fieldType);
					if (isUnityType)
					{
						if (!didGetDict)
						{
							sb.Append("\n");
							sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
							sb.Append(GDECodeGenConstants.GetDict);
							sb.Append("\n\n");
							didGetDict = true;
						}
						else
							sb.Append("\n");

						if (!didBasePathVar)
						{
							sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
							sb.Append(GDECodeGenConstants.BasePathVar);
							sb.Append("\n");
							didBasePathVar = true;
						}

						sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
						sb.AppendFormat(GDECodeGenConstants.SaveGOPathFormat, fieldKey);
						sb.Append("\n");
					}

					sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
					sb.AppendFormat(GDECodeGenConstants.BaseTypeCloneFormat, fieldKey);
					sb.Append("\n");

					shouldAppendSpace = true;
					isFirstSection = false;
				}
			}
			didAppendSpaceForSection = false;
			
			// Custom Types
			foreach(string fieldKey in GDEItemManager.SchemaCustomFieldKeys(schemaKey, 0))
			{
				if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
				{
					sb.Append("\n");
				}
				
				sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
				sb.AppendFormat(GDECodeGenConstants.CustomTypeShallowFormat, fieldKey);
				sb.Append("\n");
				
				shouldAppendSpace = true;
				didAppendSpaceForSection = true;
				isFirstSection = false;
			}
			didAppendSpaceForSection = false;

			// Basic List Types
			string variableType;
			foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
			{
				List<string> fieldKeys = GDEItemManager.SchemaFieldKeysOfType(schemaKey, fieldType.ToString(), 1);
				foreach(string fieldKey in fieldKeys)
				{
					if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
					{
						sb.Append("\n");
					}

					isUnityType = GDEItemManager.IsUnityType(fieldType);
					if (isUnityType)
					{
						if (!didGetDict)
						{
							sb.Append("\n");
							sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
							sb.Append(GDECodeGenConstants.GetDict);
							sb.Append("\n\n");
							didGetDict = true;
						}
						else
							sb.Append("\n");

						if (!didListPathVar)
						{
							sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
							sb.Append(GDECodeGenConstants.ListPathVar);
							sb.Append("\n");
							didListPathVar = true;
						}

						sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
						sb.AppendFormat(GDECodeGenConstants.SaveGOListPathFormat, fieldKey);
						sb.Append("\n");
					}

					variableType = GDEItemManager.GetVariableTypeFor(fieldType);

					sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
					sb.AppendFormat(GDECodeGenConstants.ListTypeCloneFormat, fieldKey, variableType);
					sb.Append("\n");
					
					shouldAppendSpace = true;
					didAppendSpaceForSection = true;
					isFirstSection = false;
				}
			}
			didAppendSpaceForSection = false;

			// Custom List Types
			foreach(string fieldKey in GDEItemManager.SchemaCustomFieldKeys(schemaKey, 1))
			{
				if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
				{
					sb.Append("\n");
				}

				schemaData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out variableType);
				
				sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
				sb.AppendFormat(GDECodeGenConstants.ListTypeCloneFormat, fieldKey, "GDE" + variableType + "Data");
				sb.Append("\n");
				
				shouldAppendSpace = true;
				didAppendSpaceForSection = true;
				isFirstSection = false;
			}
			didAppendSpaceForSection = false;

			// Basic 2D List Types
			foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
			{
				List<string> fieldKeys = GDEItemManager.SchemaFieldKeysOfType(schemaKey, fieldType.ToString(), 2);
				foreach(string fieldKey in fieldKeys)
				{
					if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
					{
						sb.Append("\n");
					}

					isUnityType = GDEItemManager.IsUnityType(fieldType);
					if (isUnityType)
					{
						if (!didGetDict)
						{
							sb.Append("\n");
							sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
							sb.Append(GDECodeGenConstants.GetDict);
							sb.Append("\n\n");
							didGetDict = true;
						}
						else
							sb.Append("\n");
						
						if (!did2dListPathVar)
						{
							sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
							sb.Append(GDECodeGenConstants.TwoDListPathVar);
							sb.Append("\n");
							did2dListPathVar = true;
						}
						
						sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
						sb.AppendFormat(GDECodeGenConstants.SaveGO2DListPathFormat, fieldKey);
						sb.Append("\n");
					}
					
					variableType = GDEItemManager.GetVariableTypeFor(fieldType);
					
					sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
					sb.AppendFormat(GDECodeGenConstants.TwoDListTypeCloneFormat, fieldKey, variableType);
					sb.Append("\n");
					
					shouldAppendSpace = true;
					didAppendSpaceForSection = true;
					isFirstSection = false;
				}
			}
			didAppendSpaceForSection = false;
			
			// Custom 2D List Types
			foreach(string fieldKey in GDEItemManager.SchemaCustomFieldKeys(schemaKey, 2))
			{
				if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
				{
					sb.Append("\n");
				}
				
				schemaData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out variableType);
				
				sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
				sb.AppendFormat(GDECodeGenConstants.TwoDListTypeCloneFormat, fieldKey, "GDE" + variableType + "Data");
				sb.Append("\n");
				
				shouldAppendSpace = true;
				didAppendSpaceForSection = true;
				isFirstSection = false;
			}

			sb.Append("\n");
			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
			sb.Append(GDECodeGenConstants.CloneMethodEnd);
			sb.Append("\n");
		}

		static void AppendDeepCloneMethod(StringBuilder sb, string schemaKey, Dictionary<string, object> schemaData)
		{
			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));
			sb.AppendFormat(GDECodeGenConstants.DeepCloneMethodStart, schemaKey);
			sb.Append("\n");

			bool shouldAppendSpace = false;
			bool didAppendSpaceForSection = false;
			bool isFirstSection = true;

			string variableType;

			// Custom Types
			foreach(string fieldKey in GDEItemManager.SchemaCustomFieldKeys(schemaKey, 0))
			{
				if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
				{
					sb.Append("\n");
				}
				
				sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
				sb.AppendFormat(GDECodeGenConstants.CustomTypeDeepFormat, fieldKey);
				sb.Append("\n");
				
				shouldAppendSpace = true;
				didAppendSpaceForSection = true;
				isFirstSection = false;
			}
			didAppendSpaceForSection = false;

			// Custom List Types
			for (int dimension = 1;  dimension <= 2;  dimension++)
			{
				foreach(string fieldKey in GDEItemManager.SchemaCustomFieldKeys(schemaKey, dimension))
				{
					if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
					{
						sb.Append("\n");
					}
					
					schemaData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out variableType);
					
					sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));

					if (dimension == 1)
						sb.AppendFormat(GDECodeGenConstants.CustomListTypeDeepCloneFormat, fieldKey, "GDE" + variableType + "Data");
					else 
						sb.AppendFormat(GDECodeGenConstants.CustomTwoDListTypeDeepCloneFormat, fieldKey, "GDE" + variableType + "Data");
					sb.Append("\n");
					
					shouldAppendSpace = true;
					didAppendSpaceForSection = true;
					isFirstSection = false;
				}
				didAppendSpaceForSection = false;
			}

			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
			sb.Append(GDECodeGenConstants.CloneMethodEnd);
			sb.Append("\n");
		}

        static void AppendResetVariableMethods(StringBuilder sb, string schemaKey, Dictionary<string, object> schemaData)
        {
			bool shouldAppendSpace = false;
			bool didAppendSpaceForSection = false;
			bool isFirstSection = true;

            string variableType;

            // Append all the single variables first
            foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
            {
				variableType = fieldType.ToString();
                List<string> fieldKeys = GDEItemManager.SchemaFieldKeysOfType(schemaKey, fieldType.ToString(), 0);
                foreach(string fieldKey in fieldKeys)
                {
					if (shouldAppendSpace)
						sb.Append("\n");

                    AppendResetVariableMethod(sb, fieldKey, variableType);
					shouldAppendSpace = true;
					isFirstSection = false;
                }
            }

			// Append all list variables
			for(int dimension = 1;  dimension <= 2;  dimension++)
			{
				foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
				{
					variableType = fieldType.ToString();
					List<string> fieldKeys = GDEItemManager.SchemaFieldKeysOfType(schemaKey, variableType, dimension);
					foreach(string fieldKey in fieldKeys)
					{
						if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
						{
							sb.Append("\n");
						}

						AppendResetListVariableMethod(sb, fieldKey, variableType, dimension);
						shouldAppendSpace = true;
						didAppendSpaceForSection = true;
						isFirstSection = false;
					}
				}
				didAppendSpaceForSection = false;
			}

			// Append all custom variables
			for(int dimension = 0;  dimension <= 2;  dimension++)
			{
				List<string> fieldKeys = GDEItemManager.SchemaCustomFieldKeys(schemaKey, dimension);
				foreach(string fieldKey in fieldKeys)
				{
					if (shouldAppendSpace && !didAppendSpaceForSection && !isFirstSection)
					{
						sb.Append("\n");
					}

					schemaData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out variableType);
					variableType = string.Format(GDECodeGenConstants.DataClassNameFormat, variableType);

					AppendResetCustomVariableMethod(sb, fieldKey, variableType, dimension);
					shouldAppendSpace = true;
					didAppendSpaceForSection = true;
					isFirstSection = false;
				}

				didAppendSpaceForSection = false;
			}
        }

        #region Gen Variable Declaration Methods
        static void AppendCustomVariable(StringBuilder sb, string type, string name)
        {
			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));
			sb.AppendFormat(GDECodeGenConstants.VariableFormat, type, name, "Custom");
			sb.Append("\n");
		}

		static void AppendVariable(StringBuilder sb, string type, string name)
        {
            string formattedType = type;
            if (GDEItemManager.IsUnityType(type))
                formattedType = "UnityObject";
            else 
                formattedType = type.UppercaseFirst();

            sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));
            sb.AppendFormat(GDECodeGenConstants.VariableFormat, type, name, formattedType);
            sb.Append("\n");
        }

        static void AppendListVariable(StringBuilder sb, string type, string name, int dimension)
        {
            string formattedType = type;
            if (GDEItemManager.IsUnityType(type))
                formattedType = "UnityObject";
            else 
                formattedType = type.UppercaseFirst();

            sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));

			if (dimension == 1)
				sb.AppendFormat(GDECodeGenConstants.OneDListVariableFormat, type, name, formattedType);
			else
				sb.AppendFormat(GDECodeGenConstants.TwoDListVariableFormat, type, name, formattedType+GDECodeGenConstants.TwoDListSuffix);

            sb.Append("\n");
        }

		static void AppendCustomListVariable(StringBuilder sb, string type, string name, int dimension)
		{
			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));

			if (dimension == 1)
				sb.AppendFormat(GDECodeGenConstants.OneDListVariableFormat, type, name, "Custom");
			else
				sb.AppendFormat(GDECodeGenConstants.TwoDListVariableFormat, type, name, "Custom"+GDECodeGenConstants.TwoDListSuffix);

			sb.Append("\n");
		}
		#endregion

        #region Gen Load Variable Methods
		static void AppendSaveToDictVariable(StringBuilder sb, string name)
		{
			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
			sb.AppendFormat(GDECodeGenConstants.SaveToDictVarFormat, name);
			sb.Append("\n");
		}

        static void AppendUpdateCustomItemVariable(StringBuilder sb, string name, int dimension)
        {
            sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));

            if (dimension == 0)
                sb.AppendFormat(GDECodeGenConstants.UpdateCustomItemsFormat, name);
            else if (dimension == 1)
                sb.AppendFormat(GDECodeGenConstants.UpdateCustomItemsListFormat, name);
            else if (dimension == 2)
                sb.AppendFormat(GDECodeGenConstants.UpdateCustomItemsTwoDListFormat, name);

            sb.Append("\n");
        }

        static void AppendLoadVariable(StringBuilder sb, string type, string name)
        {
            sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel4));
            sb.AppendFormat(GDECodeGenConstants.LoadVariableFormat, type, name, type.UppercaseFirst());
            sb.Append("\n");
        }

		static void AppendLoadFromSavedVariable(StringBuilder sb, string type, string name)
		{
            string formattedType;
            if (GDEItemManager.IsUnityType(type))
                formattedType = "UnityObject";
            else 
                formattedType = type.UppercaseFirst();

			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
			sb.AppendFormat(GDECodeGenConstants.LoadSavedVarFormat, formattedType, name);
			sb.Append("\n");
		}

        static void AppendLoadCustomVariable(StringBuilder sb, string type, string name)
        {
            sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel4));
            sb.AppendFormat(GDECodeGenConstants.LoadCustomVariableFormat, type, name);
            sb.Append("\n");
        }

		static void AppendLoadFromSavedCustomVariable(StringBuilder sb, string type, string name)
		{
			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
			sb.AppendFormat(GDECodeGenConstants.LoadSavedCustomVariableFormat, type, name);
			sb.Append("\n");
		}

        static void AppendLoadListVariable(StringBuilder sb, string type, string name, int dimension)
        {
            sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel4));

			if (dimension == 1)
	      		sb.AppendFormat(GDECodeGenConstants.LoadVariableListFormat, type, name);
			else
				sb.AppendFormat(GDECodeGenConstants.LoadVariableListFormat, type+GDECodeGenConstants.TwoDListSuffix, name);

            sb.Append("\n");
        }

		static void AppendLoadSavedListVariable(StringBuilder sb, string type, string name, int dimension)
		{
            string formattedType = type;
            if (GDEItemManager.IsUnityType(type))
                formattedType = "UnityObject";

			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));

			if (dimension == 1)
				sb.AppendFormat(GDECodeGenConstants.LoadSavedVariableListFormat, formattedType, name);
			else
				sb.AppendFormat(GDECodeGenConstants.LoadSavedVariableListFormat, formattedType+GDECodeGenConstants.TwoDListSuffix, name);

			sb.Append("\n");
		}
        #endregion

        #region Gen Reset Methods
        static void AppendResetAllMethod(StringBuilder sb, string schemaKey, Dictionary<string, object> schemaData)
        {
            bool shouldAppendSpace = false;

            sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));
            sb.Append(GDECodeGenConstants.ResetAllDeclaration);
            sb.Append("\n");

			// Reset all fields
            List<string> fields = GDEItemManager.SchemaFieldKeys(schemaKey, schemaData);
            foreach(string fieldName in fields)
            {
                if (shouldAppendSpace)
                    sb.Append("\n");

                sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
                sb.AppendFormat(GDECodeGenConstants.ResetToDefaultFormat, fieldName);
                shouldAppendSpace = true;
            }

			if (shouldAppendSpace)
				sb.Append("\n");

			// Call reset on any custom types
			for(int dimension=0;  dimension<= 2;  dimension++)
			{
				fields = GDEItemManager.SchemaCustomFieldKeys(schemaKey, dimension);
				foreach(string fieldName in fields)
				{
					if (shouldAppendSpace)
					    sb.Append("\n");

					sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));

					if (dimension == 0)
						sb.AppendFormat(GDECodeGenConstants.CustomResetAllFormat, fieldName);
					else
						sb.AppendFormat(GDECodeGenConstants.CustomResetAllFormat, fieldName);

					shouldAppendSpace = true;
				}
			}

            sb.Append("\n");
            sb.Append("\n");

            sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel3));
            sb.Append(GDECodeGenConstants.ResetAllEndMethod);
        }

        static void AppendResetVariableMethod(StringBuilder sb, string name, string type)
        {
            sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));
            sb.AppendFormat(GDECodeGenConstants.ResetVariableDeclarationFormat, name, type);
            sb.Append("\n");
        }

		static void AppendResetListVariableMethod(StringBuilder sb, string name, string type, int dimension)
		{
			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));

			if (dimension == 1)
				sb.AppendFormat(GDECodeGenConstants.ResetListVariableDeclarationFormat, name, type);
			else
				sb.AppendFormat(GDECodeGenConstants.ResetListVariableDeclarationFormat, name, type+GDECodeGenConstants.TwoDListSuffix);

			sb.Append("\n");
		}

		static void AppendResetCustomVariableMethod(StringBuilder sb, string name, string type, int dimension)
		{
			sb.Append("".PadLeft(GDECodeGenConstants.IndentLevel2));

			if (dimension == 0)
				sb.AppendFormat(GDECodeGenConstants.ResetCustomFormat, type, name);
			else if (dimension == 1)
				sb.AppendFormat(GDECodeGenConstants.ResetCustom1DListFormat, type, name);
			else
				sb.AppendFormat(GDECodeGenConstants.ResetCustom2DListFormat, type+GDECodeGenConstants.TwoDListSuffix, name);

			sb.Append("\n");
		}
        #endregion
    }
}
