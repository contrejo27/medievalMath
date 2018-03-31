using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameDataEditor
{
    public class GDEItemManagerWindow : GDEManagerWindowBase
    {
        string newItemName = string.Empty;
        int schemaIndex = 0;

        int filterSchemaIndex = 0;

        bool runtimeValues = false;

        Dictionary<string, string> renamedItems = new Dictionary<string, string>();

    	#region OnGUI Method
        protected override void OnGUI()
        {
            try
            {
            mainHeaderText = GDEConstants.GameDataHeader;

    		// Set the header color
    		headerColor = GDESettings.Instance.CreateDataColor.ToColor();
    		headerColor.a = 1f;

    		if (shouldRebuildEntriesList || entriesToDraw == null || GDEItemManager.ShouldReloadItems)
    		{
    			entriesToDraw = GetEntriesToDraw(GDEItemManager.AllItems);
    			shouldRebuildEntriesList = false;
    			shouldRecalculateHeights = true;
    			GDEItemManager.ShouldReloadItems = false;
    		}

    		base.OnGUI();

            DrawExpandCollapseAllFoldout(GDEItemManager.AllItems.Keys.ToArray(), GDEConstants.ItemListHeader);

            float currentGroupHeightTotal = CalculateGroupHeightsTotal();
            scrollViewHeight = drawHelper.HeightToBottomOfWindow();
            scrollViewY = drawHelper.TopOfLine();
            verticalScrollbarPosition = GUI.BeginScrollView(new Rect(drawHelper.CurrentLinePosition, scrollViewY, drawHelper.FullWindowWidth(), scrollViewHeight),
                                                            verticalScrollbarPosition,
                                                            new Rect(drawHelper.CurrentLinePosition, scrollViewY, drawHelper.ScrollViewWidth(), currentGroupHeightTotal));

            int count = 0;
            foreach (KeyValuePair<string, Dictionary<string, object>> item in entriesToDraw)
            {
                float currentGroupHeight;
                groupHeights.TryGetValue(item.Key, out currentGroupHeight);

                string itemSchema;
                if (runtimeValues)
                    item.Value.TryGetString(GDMConstants.SchemaKey, out itemSchema);
                else
                    itemSchema = GDEItemManager.GetSchemaForItem(item.Key);

                if (currentGroupHeight == 0f ||
                    (currentGroupHeight.NearlyEqual(GDEConstants.LineHeight) && entryFoldoutState.Contains(item.Key)))
                {
                    if (!groupHeightBySchema.TryGetValue(itemSchema, out currentGroupHeight))
                        currentGroupHeight = GDEConstants.LineHeight;
                }

    			int isVisible = drawHelper.IsVisible(verticalScrollbarPosition, scrollViewHeight, scrollViewY, currentGroupHeight);
    			if (isVisible == 1)
    				break;

                if (isVisible == 0 ||
                    (count == GDEItemManager.AllItems.Count-1 && verticalScrollbarPosition.y.NearlyEqual(currentGroupHeightTotal - GDEConstants.LineHeight)))
                {
                    DrawEntry(itemSchema, item.Key, item.Value);
                }
                else if (isVisible == -1)
                {
                    drawHelper.NewLine(currentGroupHeight/GDEConstants.LineHeight);
                }

                count++;
            }
            GUI.EndScrollView();

            // Remove any items that were deleted
            foreach(string deletedkey in deleteEntries)
            {

                if (!runtimeValues)
                    Remove(deletedkey);
                else
                {
                    // If we're in runtime mode, reset
                    // these items instead of deleting
                    GDEDataManager.ResetToDefault(deletedkey, string.Empty);

                    if (!GDEItemManager.AllItems.ContainsKey(deletedkey))
                        entriesToDraw.Remove(deletedkey);
                }
            }
            deleteEntries.Clear();

            // Rename any items that were renamed
            string error;
            foreach(KeyValuePair<string, string> pair in renamedItems)
            {
                if (!GDEItemManager.RenameItem(pair.Key, pair.Value, null, out error))
                    EditorUtility.DisplayDialog(GDEConstants.ErrorLbl, string.Format(GDEConstants.CouldNotRenameFormat, pair.Key, pair.Value, error), GDEConstants.OkLbl);
            }

            renamedItems.Clear();

    		// Clone any items
    		foreach(string itemKey in cloneEntries)
    			Clone(itemKey);
    		cloneEntries.Clear();
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }
        #endregion

        #region Draw Methods
        protected override Vector2 DrawExpandCollapseAllFoldout(string[] forKeys, string headerText)
        {
            Vector2 rightOfHeader = base.DrawExpandCollapseAllFoldout(forKeys, headerText);

            size.x = 18f;
            drawHelper.CurrentLinePosition = rightOfHeader.x + 20f;
            bool newRuntimeValue = EditorGUI.Toggle(new Rect(drawHelper.CurrentLinePosition, rightOfHeader.y, size.x, drawHelper.StandardHeight()), runtimeValues);
            drawHelper.CurrentLinePosition += size.x;

            if (newRuntimeValue != runtimeValues)
            {
                shouldRebuildEntriesList = true;
                runtimeValues = newRuntimeValue;
            }

            content.text = GDEConstants.RuntimeValuesLbl;
            drawHelper.TryGetCachedSize(GDEConstants.SizeRuntimeValuesLblKey, content, labelStyle, out size);
            EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, rightOfHeader.y, size.x, drawHelper.StandardHeight()), content);
            drawHelper.CurrentLinePosition = drawHelper.LeftBuffer;

            return rightOfHeader;
        }

        protected override void DrawCreateSection()
        {
            float topOfSection = drawHelper.TopOfLine() + 4f;
            float bottomOfSection = 0;
            float leftBoundary = drawHelper.CurrentLinePosition;

    		drawHelper.DrawSubHeader(GDEConstants.CreateNewItemHeader, headerColor, GDEConstants.SizeCreateSubHeaderKey);

            size.x = 60;
            GUI.Label(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), GDEConstants.SchemaLbl);
            drawHelper.CurrentLinePosition += (size.x + 2);
            if (drawHelper.CurrentLinePosition > leftBoundary)
                leftBoundary = drawHelper.CurrentLinePosition;

            size.x = 100;
            schemaIndex = EditorGUI.Popup(new Rect(drawHelper.CurrentLinePosition, drawHelper.PopupTop(), size.x, drawHelper.StandardHeight()), schemaIndex, GDEItemManager.SchemaKeyArray);
            drawHelper.CurrentLinePosition += (size.x + 6);
            if (drawHelper.CurrentLinePosition > leftBoundary)
                leftBoundary = drawHelper.CurrentLinePosition;

            size.x = 75;
            GUI.Label(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), GDEConstants.ItemNameLbl);
            drawHelper.CurrentLinePosition += (size.x + 2);
            if (drawHelper.CurrentLinePosition > leftBoundary)
                leftBoundary = drawHelper.CurrentLinePosition;

            size.x = 180;
            newItemName = EditorGUI.TextField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), newItemName);
            drawHelper.CurrentLinePosition += (size.x + 2);
            if (drawHelper.CurrentLinePosition > leftBoundary)
                leftBoundary = drawHelper.CurrentLinePosition;

            content.text = GDEConstants.CreateNewItemBtn;
    		drawHelper.TryGetCachedSize(GDEConstants.SizeCreateNewItemBtnKey, content, GUI.skin.button, out size);
    		if (GUI.Button(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content))
            {
                if (GDEItemManager.SchemaKeyArray.IsValidIndex(schemaIndex))
                {
                    List<object> args = new List<object>();
                    args.Add(GDEItemManager.SchemaKeyArray[schemaIndex]);
                    args.Add(newItemName);

                    if (Create(args))
                    {
                        newItemName = string.Empty;
                        GUI.FocusControl(string.Empty);
                    }
                }
                else
                    EditorUtility.DisplayDialog(GDEConstants.ErrorCreatingItem, GDEConstants.NoOrInvalidSchema, GDEConstants.OkLbl);
            }
            drawHelper.CurrentLinePosition += (size.x + 6);
            if (drawHelper.CurrentLinePosition > leftBoundary)
                leftBoundary = drawHelper.CurrentLinePosition;

            drawHelper.NewLine(1.5f);

            bottomOfSection = drawHelper.TopOfLine();

            drawHelper.DrawSectionSeparator();

            leftBoundary += 5f;

            // Draw rate box
    		Vector2 forumLinkSize;
    		Vector2 rateLinkSize;

    		content.text = GDEConstants.ForumLinkText;
    		drawHelper.TryGetCachedSize(GDEConstants.SizeForumLinkTextKey, content, linkStyle, out forumLinkSize);

    		content.text = GDEConstants.RateMeText;
    		drawHelper.TryGetCachedSize(GDEConstants.SizeRateMeTextKey, content, linkStyle, out rateLinkSize);

    		float boxWidth = Math.Max(forumLinkSize.x, rateLinkSize.x);

    		content.text = GDEConstants.ForumLinkText;
    		if (GUI.Button(new Rect(leftBoundary+(boxWidth-forumLinkSize.x)/2f+5.5f, bottomOfSection-size.y-15f, forumLinkSize.x, forumLinkSize.y), content, linkStyle))
    		{
    			Application.OpenURL(GDEConstants.ForumURL);
    		}

    		content.text = GDEConstants.RateMeText;
    		if(GUI.Button(new Rect(leftBoundary+(boxWidth-rateLinkSize.x)/2f+5.5f, topOfSection+15f, rateLinkSize.x, rateLinkSize.y), content, linkStyle))
    		{
    			Application.OpenURL(GDEConstants.RateMeURL);
    		}

    		DrawRateBox(leftBoundary, topOfSection, boxWidth+10f, bottomOfSection-topOfSection);
        }

        protected override bool DrawFilterSection()
        {
            bool clearSearch = base.DrawFilterSection();

    		int totalItems = GDEItemManager.AllItems.Count;
            string itemText = totalItems != 1 ? GDEConstants.ItemsLbl : GDEConstants.ItemLbl;
            if (!string.IsNullOrEmpty(filterText) ||
                (GDEItemManager.FilterSchemaKeyArray.IsValidIndex(filterSchemaIndex) && !GDEItemManager.FilterSchemaKeyArray[filterSchemaIndex].Equals(GDEConstants.AllLbl)))
            {
    			float pos = drawHelper.TopOfLine()+drawHelper.LineHeight*.1f;

    			content.text = string.Format(GDEConstants.SearchResultFormat, NumberOfItemsBeingShown(), totalItems, itemText);
    			drawHelper.TryGetCachedSize(content.text, content, EditorStyles.label, out size);
                EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, pos, size.x, size.y), content);
                drawHelper.CurrentLinePosition += (size.x + 2);
            }

            drawHelper.NewLine(1.25f);

            // Filter dropdown
            content.text = GDEConstants.FilterBySchemaLbl;
    		drawHelper.TryGetCachedSize(GDEConstants.SizeFilterBySchemaLblKey, content, EditorStyles.label, out size);
            EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content);
            drawHelper.CurrentLinePosition += (size.x + 8);

            size.x = 100;
            int newIndex = EditorGUI.Popup(new Rect(drawHelper.CurrentLinePosition, drawHelper.PopupTop(), size.x, drawHelper.StandardHeight()), filterSchemaIndex, GDEItemManager.FilterSchemaKeyArray);
    		if (GDEItemManager.FilterSchemaKeyArray.IsValidIndex(newIndex) && newIndex != filterSchemaIndex)
    		{
    			shouldRecalculateHeights = true;
    			shouldRebuildEntriesList = true;
    			filterSchemaIndex = newIndex;
    		}

            drawHelper.NewLine(1.25f);

            return clearSearch;
        }

        void DrawEntry(string schemaType, string itemKey, Dictionary<string, object> data)
        {
            float beginningHeight = drawHelper.CurrentHeight();
            bool currentFoldoutState = entryFoldoutState.Contains(itemKey);

            // Start drawing below
    		bool isOpen = DrawFoldout(schemaType+":", itemKey, itemKey, itemKey, RenameItem);
    		drawHelper.NewLine();

            if (isOpen)
            {
                bool shouldDrawSpace = false;
                bool didDrawSpaceForSection = false;
    			bool isFirstSection = true;
                
                List<string> fieldKeys;

                // Draw the basic types
                foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
                {
                    if (!runtimeValues)
                        fieldKeys = GDEItemManager.ItemFieldKeysOfType(itemKey, fieldType.EnumToString(), 0);
                    else
                        fieldKeys = GDEItemManager.ItemFieldKeysOfType(itemKey, fieldType.EnumToString(), entriesToDraw, 0);

                    foreach(string fieldKey in fieldKeys)
                    {
                        drawHelper.CurrentLinePosition += GDEConstants.Indent;
                        DrawSingleField(schemaType, itemKey, fieldKey, data);
                        shouldDrawSpace = true;
    					isFirstSection = false;
                    }
                }

                // Draw the custom types
                if (!runtimeValues)
                    fieldKeys = GDEItemManager.ItemCustomFieldKeys(itemKey, 0);
                else
                    fieldKeys = GDEItemManager.ItemCustomFieldKeys(itemKey, entriesToDraw, 0);
                foreach(string fieldKey in fieldKeys)
                {
                    if (shouldDrawSpace && !didDrawSpaceForSection && !isFirstSection)
                    {
                        drawHelper.NewLine(0.5f);
                        didDrawSpaceForSection = true;
                    }

                    drawHelper.CurrentLinePosition += GDEConstants.Indent;
                    DrawSingleField(schemaType, itemKey, fieldKey, data);
                    shouldDrawSpace = true;
    				isFirstSection = false;
                }
                didDrawSpaceForSection = false;

    			// Draw the basic lists
    			for(int dimension=1;  dimension <= 2;  dimension++)
    			{
    				foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
    				{
                        if (!runtimeValues)
                            fieldKeys = GDEItemManager.ItemFieldKeysOfType(itemKey, fieldType.EnumToString(), dimension);
                        else
                            fieldKeys = GDEItemManager.ItemFieldKeysOfType(itemKey, fieldType.EnumToString(), entriesToDraw, dimension);
                        
    					foreach(string fieldKey in fieldKeys)
    					{
    						if (shouldDrawSpace && !didDrawSpaceForSection && !isFirstSection)
    						{
    							drawHelper.NewLine(0.5f);
    							didDrawSpaceForSection = true;
    						}

    						drawHelper.CurrentLinePosition += GDEConstants.Indent;

    						if (dimension == 1)
    							DrawListField(schemaType, itemKey, fieldKey, data);
    						else
    							Draw2DListField(schemaType, itemKey, fieldKey, data);

    						shouldDrawSpace = true;
    						isFirstSection = false;
    						didDrawSpaceForSection = true;
    					}
    				}
    				didDrawSpaceForSection = false;
    			}

                // Draw the custom lists
    			for(int dimension=1;  dimension <= 2;  dimension++)
    			{
                    if (!runtimeValues)
                        fieldKeys = GDEItemManager.ItemCustomFieldKeys(itemKey, dimension);
                    else
                        fieldKeys = GDEItemManager.ItemCustomFieldKeys(itemKey, entriesToDraw, dimension);
                    
    	            foreach(string fieldKey in fieldKeys)
    	            {
    					if (shouldDrawSpace && !didDrawSpaceForSection && !isFirstSection)
    	                {
    	                    drawHelper.NewLine(0.5f);
    	                    didDrawSpaceForSection = true;
    	                }

    	                drawHelper.CurrentLinePosition += GDEConstants.Indent;
    					if (dimension == 1)
    						DrawListField(schemaType, itemKey, fieldKey, data);
    					else
    						Draw2DListField(schemaType, itemKey, fieldKey, data);

    	                shouldDrawSpace = true;
    					isFirstSection = false;
    					didDrawSpaceForSection = true;
    	            }
    				didDrawSpaceForSection = false;
    			}

                drawHelper.NewLine(0.5f);

    			DrawEntryFooter(GDEConstants.CloneItem, GDEConstants.SizeCloneItemKey, itemKey);
            }
            else if (!isOpen && currentFoldoutState)
            {
                // Collapse any list foldouts as well
                List<string> listKeys = GDEItemManager.ItemListFieldKeys(itemKey);
                string foldoutKey;
                foreach(string listKey in listKeys)
                {
                    foldoutKey = itemKey.FoldoutKey(listKey);
                    listFieldFoldoutState.Remove(foldoutKey);
                }
            }

            float newGroupHeight = drawHelper.CurrentHeight() - beginningHeight;
            float currentGroupHeight;

    		groupHeights.TryGetValue(itemKey, out currentGroupHeight);

    		if (!newGroupHeight.NearlyEqual(currentGroupHeight))
    		{
    			currentGroupHeightTotal -= currentGroupHeight;
    			currentGroupHeightTotal += newGroupHeight;

    			SetSchemaHeight(schemaType, newGroupHeight);
    		}

            SetGroupHeight(itemKey, newGroupHeight);
        }

        void DrawSingleField(string schemaKey, string itemKey, string fieldKey, Dictionary<string, object> itemData)
        {
        		string fieldPreviewKey = schemaKey.PreviewKey(itemKey, fieldKey);
        		string fieldType;
            itemData.TryGetString(fieldKey.TypeKey(), out fieldType);

            BasicFieldType fieldTypeEnum = BasicFieldType.Undefined;
            if (Enum.IsDefined(typeof(BasicFieldType), fieldType))
            {
                fieldTypeEnum = (BasicFieldType)Enum.Parse(typeof(BasicFieldType), fieldType);
          			fieldType = GDEItemManager.GetVariableTypeFor(fieldTypeEnum);
        		}

        		content.text = fieldType;
        		drawHelper.TryGetCachedSize(schemaKey+fieldKey+GDEConstants.TypeSuffix, content, labelStyle, out size);
        		size.x = Math.Max(size.x, GDEConstants.MinLabelWidth);

            GUI.Label(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content, labelStyle);
            drawHelper.CurrentLinePosition += (size.x + 2);

        		content.text = fieldKey.HighlightSubstring(filterText, highlightColor);
        		drawHelper.TryGetCachedSize(schemaKey+fieldKey+GDEConstants.LblSuffix, content, labelStyle, out size);
        		size.x = Math.Max(size.x, GDEConstants.MinLabelWidth);

            GUI.Label(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content, labelStyle);
            drawHelper.CurrentLinePosition += (size.x + 2);

            switch(fieldTypeEnum)
            {
                case BasicFieldType.Bool:
                {
                    DrawBool(itemKey, fieldKey, itemData, GDEConstants.ValueLbl);
                    drawHelper.NewLine();
                    break;
                }
                case BasicFieldType.Int:
                {
                    DrawInt(itemKey, fieldKey, itemData, GDEConstants.ValueLbl);
                    drawHelper.NewLine();
                    break;
                }
                case BasicFieldType.Float:
                {
                    DrawFloat(itemKey, fieldKey, itemData, GDEConstants.ValueLbl);
                    drawHelper.NewLine();
                    break;
                }
                case BasicFieldType.String:
                {
                    DrawString(itemKey, fieldKey, itemData, GDEConstants.ValueLbl);
                    drawHelper.NewLine();
                    break;
                }
                case BasicFieldType.Vector2:
                {
            				DrawVector2(itemKey, fieldKey, itemData, GDEConstants.ValueLbl);
                    drawHelper.NewLine(GDEConstants.VectorFieldBuffer+1);
                    break;
                }
                case BasicFieldType.Vector3:
                {
            				DrawVector3(itemKey, fieldKey, itemData, GDEConstants.ValueLbl);
                    drawHelper.NewLine(GDEConstants.VectorFieldBuffer+1);
                    break;
                }
                case BasicFieldType.Vector4:
                {
            				DrawVector4(itemKey, fieldKey, itemData, GDEConstants.ValueLbl);
                    drawHelper.NewLine(GDEConstants.VectorFieldBuffer+1);
                    break;
                }
                case BasicFieldType.Color:
                {
            				DrawColor(itemKey, fieldKey, itemData, GDEConstants.ValueLbl);
                    drawHelper.NewLine();
                    break;
                }
          			case BasicFieldType.GameObject:
          			{
            				DrawObject<GameObject>(itemKey, fieldPreviewKey, fieldKey, itemData, GDEConstants.ValueLbl);
            				drawHelper.NewLine();
            				break;
          			}
          			case BasicFieldType.Texture2D:
          			{
            				DrawObject<Texture2D>(itemKey, fieldPreviewKey, fieldKey, itemData, GDEConstants.ValueLbl);
            				drawHelper.NewLine();
            				break;
          			}
          			case BasicFieldType.Material:
          			{
            				DrawObject<Material>(itemKey, fieldPreviewKey, fieldKey, itemData, GDEConstants.ValueLbl);
            				drawHelper.NewLine();
            				break;
          			}
          			case BasicFieldType.AudioClip:
          			{
            				DrawAudio(itemKey, fieldPreviewKey, fieldKey, itemData, GDEConstants.ValueLbl);
            				drawHelper.NewLine();
            				break;
          			}
                default:
                {
                    List<string> itemKeys = GetPossibleCustomValues(schemaKey, fieldType);
                    DrawCustom(itemKey, fieldKey, itemData, itemKeys);
                    drawHelper.NewLine();
                    break;
                }
            }
        }

        void DrawListField(string schemaKey, string itemKey, string fieldKey, Dictionary<string, object> itemData)
        {
            try
            {
          			string foldoutKey = itemKey.FoldoutKey(fieldKey);
                bool newFoldoutState;
                bool currentFoldoutState = listFieldFoldoutState.Contains(foldoutKey);
                object defaultResizeValue = null;

                string fieldType;
                itemData.TryGetString(fieldKey.TypeKey(), out fieldType);

                BasicFieldType fieldTypeEnum = BasicFieldType.Undefined;
                if (Enum.IsDefined(typeof(BasicFieldType), fieldType))
                {
                    fieldTypeEnum = (BasicFieldType)Enum.Parse(typeof(BasicFieldType), fieldType);
    				         fieldType = GDEItemManager.GetVariableTypeFor(fieldTypeEnum);

                    // if (runtimeValues)
                    //     defaultResizeValue = GDEItemManager.GetDefaultRuntimeValueForType(fieldTypeEnum);
                    // else
                    //     defaultResizeValue = GDEItemManager.GetDefaultValueForType(fieldTypeEnum);
                }

          			content.text = fieldType.ListDeclaration();
          			drawHelper.TryGetCachedSize(schemaKey+fieldKey+GDEConstants.TypeSuffix, content, EditorStyles.foldout, out size);
          			size.x = Math.Max(size.x, GDEConstants.MinLabelWidth);
                newFoldoutState = EditorGUI.Foldout(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), currentFoldoutState, content);
                drawHelper.CurrentLinePosition += (size.x + 2);

          			content.text = fieldKey.HighlightSubstring(filterText, highlightColor);
          			drawHelper.TryGetCachedSize(schemaKey+fieldKey+GDEConstants.LblSuffix, content, labelStyle, out size);
          			size.x = Math.Max(size.x, GDEConstants.MinLabelWidth);
                GUI.Label(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), fieldKey.HighlightSubstring(filterText, highlightColor), labelStyle);
                drawHelper.CurrentLinePosition += (size.x + 2);

                if (newFoldoutState != currentFoldoutState)
                {
                    if (newFoldoutState)
                        listFieldFoldoutState.Add(foldoutKey);
                    else
                        listFieldFoldoutState.Remove(foldoutKey);
                }

                object temp = null;
                IList list = null;

                if (itemData.TryGetValue(fieldKey, out temp))
                    list = temp as IList;

                if (runtimeValues)
                {
                    IList rtList = null;
                    switch (fieldTypeEnum)
                    {
                        case BasicFieldType.Bool:
                        {
                            rtList = GDEDataManager.GetBoolList(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = list.ToBoolList();
                            break;
                        }
                        case BasicFieldType.Int:
                        {
                            rtList = GDEDataManager.GetIntList(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = list.ToIntList();
                                break;
                        }
                        case BasicFieldType.Float:
                        {
                            rtList = GDEDataManager.GetFloatList(itemKey, fieldKey);                           
                            if (rtList == null)
                                rtList = list.ToFloatList();
                            break;
                        }
                        case BasicFieldType.Vector2:
                        {
                            rtList = GDEDataManager.GetVector2List(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = (temp as IList).ToVector2List();
                            break;
                        }
                        case BasicFieldType.Vector3:
                        {
                            rtList = GDEDataManager.GetVector3List(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = (temp as IList).ToVector3List();
                            break;
                        }
                        case BasicFieldType.Vector4:
                        {
                            rtList = GDEDataManager.GetVector4List(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = (temp as IList).ToVector4List();
                            break;
                        }
                        case BasicFieldType.Color:
                        {
                            rtList = GDEDataManager.GetColorList(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = (temp as IList).ToColorList();
                            break;
                        }
                        case BasicFieldType.GameObject:
                        {
                            rtList = GDEDataManager.GetUnityObjectList<GameObject>(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = (temp as IList).Cast<GameObject>().ToList();
                            break;
                        }
                        case BasicFieldType.Texture2D:
                        {
                            rtList = GDEDataManager.GetUnityObjectList<Texture2D>(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = (temp as IList).Cast<Texture2D>().ToList();
                        break;
                        }
                        case BasicFieldType.Material:
                        {
                            rtList = GDEDataManager.GetUnityObjectList<Material>(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = (temp as IList).Cast<Material>().ToList();
                        break;
                        }
                        case BasicFieldType.AudioClip:
                        {
                            rtList = GDEDataManager.GetUnityObjectList<AudioClip>(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = (temp as IList).Cast<AudioClip>().ToList();
                        break;
                        }
                        case BasicFieldType.String:
                        default:
                        {
                            rtList = GDEDataManager.GetStringList(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = list.ToStringList();
                            break;
                        }
                    }

                    if (rtList != null)
                        list = rtList;
                    else
                        throw new Exception("runtime typed list was not built!");
                }

                content.text = GDEConstants.SizeLbl;
          			drawHelper.TryGetCachedSize(GDEConstants.SizeSizeLblKey, content, EditorStyles.label, out size);
          			EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content);
                drawHelper.CurrentLinePosition += (size.x + 2);

                int newListCount;
                string listCountKey = string.Format(GDMConstants.MetaDataFormat, itemKey, fieldKey);
                if (runtimeValues)
                    listCountKey = GDEConstants.RuntimeKeyPrefix + listCountKey;

                if (newListCountDict.ContainsKey(listCountKey))
                {
                    newListCount = newListCountDict[listCountKey];
                }
                else
                {
                    newListCount = list.Count;
                    newListCountDict.Add(listCountKey, newListCount);
                }

                size.x = 40;

                if (runtimeValues && GDEItemManager.IsUnityType(fieldTypeEnum))
                {
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), newListCount.ToString());
                }
                else
                {
                    newListCount = EditorGUI.IntField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), newListCount);
                    drawHelper.CurrentLinePosition += (size.x + 4);

                    content.text = GDEConstants.ResizeBtn;
              			drawHelper.TryGetCachedSize(GDEConstants.SizeResizeBtnKey, content, GUI.skin.button, out size);
              			newListCountDict[listCountKey] = newListCount;
                    if (newListCount != list.Count && GUI.Button(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content))
                    {
                        if (runtimeValues)
                            defaultResizeValue = GDEItemManager.GetDefaultRuntimeValueForType(fieldTypeEnum);
                        else
            	              defaultResizeValue = GDEItemManager.GetDefaultValueForType(fieldTypeEnum);
                        
                        if (ResizeList(list, newListCount, defaultResizeValue))
                        {
                            if (runtimeValues)
                            {
                                SetRuntimeListValue(itemKey, fieldKey, list, fieldTypeEnum, 1);
                                SetNeedToSaveModData(true);
                            }
                            else
                                SetNeedToSave(true);
                        }

                        newListCountDict[listCountKey] = newListCount;
                        drawHelper.CurrentLinePosition += (size.x + 2);
                    }
                }

                drawHelper.NewLine();

                if (newFoldoutState)
                {
                    bool needsSave = false;
                    for (int i = 0; i < list.Count; i++)
                    {
              					drawHelper.CurrentLinePosition += GDEConstants.Indent*2;
              					content.text = i.OneDLbl();

                        switch (fieldTypeEnum)
                        {
                            case BasicFieldType.Bool:
                            {
                                needsSave |= DrawListBool(itemKey, fieldKey, content, i, Convert.ToBoolean(list[i]), list);
                                drawHelper.NewLine();
                                break;
                            }
                            case BasicFieldType.Int:
                            {
                                needsSave |= DrawListInt(itemKey, fieldKey, content, i, Convert.ToInt32(list[i]), list);
                                drawHelper.NewLine();
                                break;
                            }
                            case BasicFieldType.Float:
                            {
                                needsSave |= DrawListFloat(itemKey, fieldKey, content, i, Convert.ToSingle(list[i]), list);
                                drawHelper.NewLine();
                                break;
                            }
                            case BasicFieldType.String:
                            {
                                needsSave |= DrawListString(itemKey, fieldKey, content, i, list[i] as string, list);
                                drawHelper.NewLine();
                                break;
                            }
                            case BasicFieldType.Vector2:
                            {
                                needsSave |= DrawListVector2(itemKey, fieldKey, content, i, list[i] as Dictionary<string, object>, list);
                                drawHelper.NewLine(GDEConstants.VectorFieldBuffer+1);
                                break;
                            }
                            case BasicFieldType.Vector3:
                            {
                                needsSave |= DrawListVector3(itemKey, fieldKey, content, i, list[i] as Dictionary<string, object>, list);
                                drawHelper.NewLine(GDEConstants.VectorFieldBuffer+1);
                                break;
                            }
                            case BasicFieldType.Vector4:
                            {
                                needsSave |= DrawListVector4(itemKey, fieldKey, content, i, list[i] as Dictionary<string, object>, list);
                                drawHelper.NewLine(GDEConstants.VectorFieldBuffer+1);
                                break;
                            }
                            case BasicFieldType.Color:
                            {
                                needsSave |= DrawListColor(itemKey, fieldKey, content, i, list[i] as Dictionary<string, object>, list);
                                drawHelper.NewLine();
                                break;
                            }
                						case BasicFieldType.GameObject:
                						{
                                needsSave |= DrawListObject<GameObject>(itemKey, fieldKey, foldoutKey+i, content, i, list[i] as GameObject, list);
                  							drawHelper.NewLine();
                  							break;
                						}
                						case BasicFieldType.Texture2D:
                						{
                                needsSave |= DrawListObject<Texture2D>(itemKey, fieldKey, foldoutKey+i, content, i, list[i] as Texture2D, list);
                  							drawHelper.NewLine();
                  							break;
                						}
                						case BasicFieldType.Material:
                						{
                                needsSave |= DrawListObject<Material>(itemKey, fieldKey, foldoutKey+i, content, i, list[i] as Material, list);
                  							drawHelper.NewLine();
                  							break;
                						}
                						case BasicFieldType.AudioClip:
                						{
                                needsSave |= DrawListAudio(itemKey, fieldKey, foldoutKey+i, content, i, list[i] as AudioClip, list);
                  							drawHelper.NewLine();
                  							break;
                						}
                            default:
                            {
                                List<string> itemKeys = GetPossibleCustomValues(schemaKey, fieldType);
                                needsSave |= DrawListCustom(itemKey, fieldKey, content, i, list[i] as string, list, itemKeys);
                                drawHelper.NewLine();
                                break;
                            }
                        }
                    }

                    if (needsSave && runtimeValues)
                        SetRuntimeListValue(itemKey, fieldKey, list, fieldTypeEnum, 1);
                }
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }

    	void Draw2DListField(string schemaKey, string itemKey, string fieldKey, Dictionary<string, object> itemData)
    	{
    		try
    		{
    			string foldoutKey = itemKey.FoldoutKey(fieldKey);
    			object defaultResizeValue;

    			string fieldType;
    			itemData.TryGetString(fieldKey.TypeKey(), out fieldType);

    			BasicFieldType fieldTypeEnum = BasicFieldType.Undefined;
    			if (Enum.IsDefined(typeof(BasicFieldType), fieldType))
    			{
    				fieldTypeEnum = (BasicFieldType)Enum.Parse(typeof(BasicFieldType), fieldType);
    				fieldType = GDEItemManager.GetVariableTypeFor(fieldTypeEnum);
    			}

    			content.text = fieldType.ListTwoDDeclaration();
    			bool isOpen = DrawFoldout(content.text, foldoutKey, string.Empty, string.Empty, null);

    			drawHelper.CurrentLinePosition = Math.Max(drawHelper.CurrentLinePosition, GDEConstants.MinLabelWidth+GDEConstants.Indent+4);
    			content.text = fieldKey.HighlightSubstring(filterText, highlightColor);
    			drawHelper.TryGetCachedSize(schemaKey+fieldKey+GDEConstants.LblSuffix, content, labelStyle, out size);

    			size.x = Math.Max(size.x, GDEConstants.MinLabelWidth);
    			EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), fieldKey.HighlightSubstring(filterText, highlightColor), labelStyle);
    			drawHelper.CurrentLinePosition += (size.x + 2);

    			object temp = null;
    			IList list = null;

    			if (itemData.TryGetValue(fieldKey, out temp))
    				list = temp as IList;

                if (runtimeValues)
                {
                    IList rtList = null;
                    switch (fieldTypeEnum)
                    {
                        case BasicFieldType.Bool:
                        {
                            rtList = GDEDataManager.GetBoolTwoDList(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = list.ToBoolTwoDList();
                            break;
                        }
                        case BasicFieldType.Int:
                        {
                            rtList = GDEDataManager.GetIntTwoDList(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = list.ToIntTwoDList();
                            break;
                        }
                        case BasicFieldType.Float:
                        {
                            rtList = GDEDataManager.GetFloatTwoDList(itemKey, fieldKey);                           
                            if (rtList == null)
                                rtList = list.ToFloatTwoDList();
                            break;
                        }
                        case BasicFieldType.Vector2:
                        {
                            rtList = GDEDataManager.GetVector2TwoDList(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = list.ToVector2TwoDList();
                            break;
                        }
                        case BasicFieldType.Vector3:
                        {
                            rtList = GDEDataManager.GetVector3TwoDList(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = list.ToVector3TwoDList();
                            break;
                        }
                        case BasicFieldType.Vector4:
                        {
                            rtList = GDEDataManager.GetVector4TwoDList(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = list.ToVector4TwoDList();
                            break;
                        }
                        case BasicFieldType.Color:
                        {
                            rtList = GDEDataManager.GetColorTwoDList(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = list.ToColorTwoDList();
                            break;
                        }
                        case BasicFieldType.GameObject:
                        {
                            rtList = GDEDataManager.GetUnityObjectTwoDList<GameObject>(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = list.Cast<List<GameObject>>().ToList();
                            break;
                        }
                        case BasicFieldType.Texture2D:
                        {
                            rtList = GDEDataManager.GetUnityObjectTwoDList<Texture2D>(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = list.Cast<List<Texture2D>>().ToList();
                            break;
                        }
                        case BasicFieldType.Material:
                        {
                            rtList = GDEDataManager.GetUnityObjectTwoDList<Material>(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = list.Cast<List<Material>>().ToList();
                            break;
                        }
                        case BasicFieldType.AudioClip:
                        {
                            rtList = GDEDataManager.GetUnityObjectTwoDList<AudioClip>(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = list.Cast<List<AudioClip>>().ToList();
                            break;
                        }
                        case BasicFieldType.String:
                        default:
                        {
                            rtList = GDEDataManager.GetStringTwoDList(itemKey, fieldKey);
                            if (rtList == null)
                                rtList = list.ToStringTwoDList();
                            break;
                        }
                    }
                    
                    if (rtList != null)
                        list = rtList;
                    else
                        throw new Exception("runtime typed list not build!");
                }

          			content.text = GDEConstants.SizeLbl;
          			drawHelper.TryGetCachedSize(GDEConstants.SizeSizeLblKey, content, EditorStyles.label, out size);
          			EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content);
          			drawHelper.CurrentLinePosition += (size.x + 2);

          			int newListCount;
          			string listCountKey = itemKey.ListCountKey(fieldKey);
                      if (runtimeValues)
                          listCountKey = GDEConstants.RuntimeKeyPrefix + listCountKey;

          			if (newListCountDict.ContainsKey(listCountKey))
          			{
            				newListCount = newListCountDict[listCountKey];
          			}
          			else
          			{
            				newListCount = list.Count;
            				newListCountDict.Add(listCountKey, newListCount);
          			}

                size.x = 40;
                if (runtimeValues && GDEItemManager.IsUnityType(fieldTypeEnum))
                {
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), newListCount.ToString());
                }
                else
                {
              			newListCount = EditorGUI.IntField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), newListCount);
              			drawHelper.CurrentLinePosition += (size.x + 4);

              			content.text = GDEConstants.ResizeBtn;
              			drawHelper.TryGetCachedSize(GDEConstants.SizeResizeBtnKey, content, GUI.skin.button, out size);
              			newListCountDict[listCountKey] = newListCount;
              			if (list != null && newListCount != list.Count && GUI.Button(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content))
              			{
                				if (GDEItemManager.IsUnityType(fieldTypeEnum) || runtimeValues)
                					defaultResizeValue = Activator.CreateInstance(list.GetType().GetGenericArguments()[0]);
                				else
                					defaultResizeValue = new List<object>();

                				if (ResizeList(list, newListCount, defaultResizeValue))
                        {
                            if (runtimeValues)
                            {
                                SetRuntimeListValue(itemKey, fieldKey, list, fieldTypeEnum, 2);
                                SetNeedToSaveModData(true);
                            }
                            else
                                SetNeedToSave(true);
                        }
                				newListCountDict[listCountKey] = newListCount;
                				drawHelper.CurrentLinePosition += (size.x + 2);
              			}
                }

    			drawHelper.NewLine();

    			if (isOpen)
    			{
              // if (runtimeValues)
//                   defaultResizeValue = GDEItemManager.GetDefaultRuntimeValueForType(fieldTypeEnum);
//               else
//                   defaultResizeValue = GDEItemManager.GetDefaultValueForType(fieldTypeEnum);

              bool needsSave = false;
      				for (int index = 0; index < list.Count; index++)
      				{
      					IList subList = list[index] as IList;

      					drawHelper.CurrentLinePosition += GDEConstants.Indent*2;
      					content.text = fieldType.TwoDSublistDeclaration(index);

      					isOpen = DrawFoldout(content.text, foldoutKey+"_"+index, string.Empty, string.Empty, null);
      					drawHelper.CurrentLinePosition += 4;

      					// Draw resize
      					content.text = GDEConstants.SizeLbl;
      					drawHelper.TryGetCachedSize(GDEConstants.SizeSizeLblKey, content, EditorStyles.label, out size);
      					EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content);
      					drawHelper.CurrentLinePosition += (size.x + 2);

      					listCountKey = schemaKey.ListCountKey(fieldKey)+"_"+index;
                if (runtimeValues)
                    listCountKey = GDEConstants.RuntimeKeyPrefix + listCountKey;

      					if (newListCountDict.ContainsKey(listCountKey))
      					{
      						newListCount = newListCountDict[listCountKey];
      					}
      					else
      					{
      						newListCount = subList.Count;
      						newListCountDict.Add(listCountKey, newListCount);
      					}

      					size.x = 40;

                if (runtimeValues && GDEItemManager.IsUnityType(fieldTypeEnum))
                {
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), newListCount.ToString());
                }
                else
                {
          					newListCount = EditorGUI.IntField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), newListCount);
          					drawHelper.CurrentLinePosition += (size.x + 2);

          					newListCountDict[listCountKey] = newListCount;

          					content.text = GDEConstants.ResizeBtn;
          					drawHelper.TryGetCachedSize(GDEConstants.SizeResizeBtnKey, content, GUI.skin.button, out size);
          					if (newListCount != subList.Count)
          					{
            						if (GUI.Button(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content))
                        {
                            if (runtimeValues)
                                defaultResizeValue = GDEItemManager.GetDefaultRuntimeValueForType(fieldTypeEnum);
                            else
                        				defaultResizeValue = GDEItemManager.GetDefaultValueForType(fieldTypeEnum);
              							if (ResizeList(subList, newListCount, defaultResizeValue))
                            {
                                if (runtimeValues)
                                {
                                    SetRuntimeListValue(itemKey, fieldKey, list, fieldTypeEnum, 2);
                                    SetNeedToSaveModData(true);
                                }
                                else
                                    SetNeedToSave(true);
                            }
                        }
            						drawHelper.CurrentLinePosition += (size.x + 2);
          					}
                }

    					drawHelper.NewLine();

    					if (isOpen)
    					{
    						for (int x = 0; x < subList.Count; x++)
    						{
    							drawHelper.CurrentLinePosition += GDEConstants.Indent*3;
    							content.text = index.TwoDSublistLbl(x);

    							switch (fieldTypeEnum)
    							{
    								case BasicFieldType.Bool:
    								{
                                        needsSave |= DrawListBool(itemKey, fieldKey, content, x, Convert.ToBoolean(subList[x]), subList);
    									drawHelper.NewLine();
    									break;
    								}
    								case BasicFieldType.Int:
    								{
                                        needsSave |= DrawListInt(itemKey, fieldKey, content, x, Convert.ToInt32(subList[x]), subList);
    									drawHelper.NewLine();
    									break;
    								}
    								case BasicFieldType.Float:
    								{
                                        needsSave |= DrawListFloat(itemKey, fieldKey, content, x, Convert.ToSingle(subList[x]), subList);
    									drawHelper.NewLine();
    									break;
    								}
    								case BasicFieldType.String:
    								{
                                        needsSave |= DrawListString(itemKey, fieldKey, content, x, subList[x] as string, subList);
                                        drawHelper.NewLine();
    									break;
    								}
    								case BasicFieldType.Vector2:
    								{
                                        needsSave |= DrawListVector2(itemKey, fieldKey, content, x, subList[x] as Dictionary<string, object>, subList);
    									drawHelper.NewLine(GDEConstants.VectorFieldBuffer+1);
    									break;
    								}
    								case BasicFieldType.Vector3:
    								{
                                        needsSave |= DrawListVector3(itemKey, fieldKey, content, x, subList[x] as Dictionary<string, object>, subList);
    									drawHelper.NewLine(GDEConstants.VectorFieldBuffer+1);
    									break;
    								}
    								case BasicFieldType.Vector4:
    								{
                                        needsSave |= DrawListVector4(itemKey, fieldKey, content, x, subList[x] as Dictionary<string, object>, subList);
    									drawHelper.NewLine(GDEConstants.VectorFieldBuffer+1);
    									break;
    								}
    								case BasicFieldType.Color:
    								{
                                        needsSave |= DrawListColor(itemKey, fieldKey, content, x, subList[x] as Dictionary<string, object>, subList);
    									drawHelper.NewLine();
    									break;
    								}
    								case BasicFieldType.GameObject:
    								{
                                        needsSave |= DrawListObject<GameObject>(itemKey, fieldKey, foldoutKey+index+"_"+x, content, x, subList[x] as GameObject, subList);
    									drawHelper.NewLine();
    									break;
    								}
    								case BasicFieldType.Texture2D:
    								{
                                        needsSave |= DrawListObject<Texture2D>(itemKey, fieldKey, foldoutKey+index+"_"+x, content, x, subList[x] as Texture2D, subList);
    									drawHelper.NewLine();
    									break;
    								}
    								case BasicFieldType.Material:
    								{
                                        needsSave |= DrawListObject<Material>(itemKey, fieldKey, foldoutKey+index+"_"+x, content, x, subList[x] as Material, subList);
    									drawHelper.NewLine();
    									break;
    								}
    								case BasicFieldType.AudioClip:
    								{
                                        needsSave |= DrawListAudio(itemKey, fieldKey, foldoutKey+index+"_"+x, content, x, subList[x] as AudioClip, subList);
    									drawHelper.NewLine();
    									break;
    								}
    								default:
    								{
    									List<string> itemKeys = GetPossibleCustomValues(schemaKey, fieldType);
                                        needsSave |= DrawListCustom(itemKey, fieldKey, content, x, subList[x] as String, subList, itemKeys);
    									drawHelper.NewLine();
    									break;
    								}
    							}
    						}
    					}
    				}

                    if (needsSave && runtimeValues)
                        SetRuntimeListValue(itemKey, fieldKey, list, fieldTypeEnum, 2);
    			}
    		}
    		catch(Exception ex)
    		{
    			Debug.LogError(ex);
    		}
    	}

        void DrawBool(string itemKey, string fieldName, Dictionary<string, object> data, string label)
        {
            try
            {
                if (!runtimeValues)
                    DrawBool(fieldName, data, label);
                else
                {
                    bool origValue;
                    bool modValue;
                    bool newValue;

                    data.TryGetBool(fieldName, out origValue);
                    modValue = GDEDataManager.GetBool(itemKey, fieldName, origValue);

                    content.text = GDEConstants.RuntimeVarValLbl;
                    drawHelper.TryGetCachedSize(GDEConstants.SizeRuntimeVarValLblKey, content, EditorStyles.label, out size);
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    size.x = 50;
                    newValue = EditorGUI.Toggle(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), modValue);
                    drawHelper.CurrentLinePosition += (size.x + 2);

                    if (newValue != modValue && newValue != origValue)
                    {
                        GDEDataManager.SetBool(itemKey, fieldName, newValue);
                        SetNeedToSaveModData(true);
                    }
                    else if (newValue != modValue && newValue == origValue)
                    {
                        GDEDataManager.ResetToDefault(itemKey, fieldName);
                        SetNeedToSaveModData(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        void DrawInt(string itemKey, string fieldName, Dictionary<string, object> data, string label)
        {
            try
            {
                if (!runtimeValues)
                    DrawInt(fieldName, data, label);
                else
                {
                    int origValue;
                    int modValue;
                    int newValue;

                    data.TryGetInt(fieldName, out origValue);
                    modValue = GDEDataManager.GetInt(itemKey, fieldName, origValue);
                    
                    content.text = GDEConstants.RuntimeVarValLbl;
                    drawHelper.TryGetCachedSize(GDEConstants.SizeRuntimeVarValLblKey, content, EditorStyles.label, out size);
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    size.x = 50;
                    newValue = EditorGUI.IntField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), modValue);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    if (newValue != modValue && newValue != origValue)
                    {
                        GDEDataManager.SetInt(itemKey, fieldName, newValue);
                        SetNeedToSaveModData(true);
                    }
                    else if (newValue != modValue && newValue == origValue)
                    {
                        GDEDataManager.ResetToDefault(itemKey, fieldName);
                        SetNeedToSaveModData(true);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        void DrawFloat(string itemKey, string fieldName, Dictionary<string, object> data, string label)
        {
            try
            {
                if (!runtimeValues)
                    DrawFloat(fieldName, data, label);
                else
                {
                    float origValue;
                    float modValue;
                    float newValue;
                    
                    data.TryGetFloat(fieldName, out origValue);
                    modValue = GDEDataManager.GetFloat(itemKey, fieldName, origValue);
                    
                    content.text = GDEConstants.RuntimeVarValLbl;
                    drawHelper.TryGetCachedSize(GDEConstants.SizeRuntimeVarValLblKey, content, EditorStyles.label, out size);
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    size.x = 50;
                    newValue = EditorGUI.FloatField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), modValue);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    if (newValue != modValue && newValue != origValue)
                    {
                        GDEDataManager.SetFloat(itemKey, fieldName, newValue);
                        SetNeedToSaveModData(true);
                    }
                    else if (newValue != modValue && newValue == origValue)
                    {
                        GDEDataManager.ResetToDefault(itemKey, fieldName);
                        SetNeedToSaveModData(true);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        void DrawString(string itemKey, string fieldName, Dictionary<string, object> data, string label)
        {
            try
            {
                if (!runtimeValues)
                    DrawString(fieldName, data, label);
                else
                {
                    string origValue;
                    string modValue;
                    string newValue;
                    
                    data.TryGetString(fieldName, out origValue);
                    modValue = GDEDataManager.GetString(itemKey, fieldName, origValue);
                    
                    content.text = GDEConstants.RuntimeVarValLbl;
                    drawHelper.TryGetCachedSize(GDEConstants.SizeRuntimeVarValLblKey, content, EditorStyles.label, out size);
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    size.x = 50;
                    newValue = DrawResizableTextBox(modValue);
                    drawHelper.CurrentLinePosition += (size.x + 2);

                    if (newValue != modValue && newValue != origValue)
                    {
                        GDEDataManager.SetString(itemKey, fieldName, newValue);
                        SetNeedToSaveModData(true);
                    }
                    else if (newValue != modValue && newValue == origValue)
                    {
                        GDEDataManager.ResetToDefault(itemKey, fieldName);
                        SetNeedToSaveModData(true);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        void DrawVector2(string itemKey, string fieldName, Dictionary<string, object> data, string label)
        {
            try
            {
                if (!runtimeValues)
                    DrawVector2(fieldName, data, label);
                else
                {
                    Vector2 origValue;
                    Vector2 modValue;
                    Vector2 newValue;
                    
                    data.TryGetVector2(fieldName, out origValue);
                    
                    modValue = GDEDataManager.GetVector2(itemKey, fieldName, origValue);
                    
                    content.text = GDEConstants.RuntimeVarValLbl;
                    size.x = 136f;
                    newValue = EditorGUI.Vector2Field(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), content, modValue);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    if (newValue != modValue && newValue != origValue)
                    {
                        GDEDataManager.SetVector2(itemKey, fieldName, newValue);
                        SetNeedToSaveModData(true);
                    }
                    else if (newValue != modValue && newValue == origValue)
                    {
                        GDEDataManager.ResetToDefault(itemKey, fieldName);
                        SetNeedToSaveModData(true);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        void DrawVector3(string itemKey, string fieldName, Dictionary<string, object> data, string label)
        {
            try
            {
                if (!runtimeValues)
                    DrawVector3(fieldName, data, label);
                else
                {
                    Vector3 origValue;
                    Vector3 modValue;
                    Vector3 newValue;
                    
                    data.TryGetVector3(fieldName, out origValue);
                    modValue = GDEDataManager.GetVector3(itemKey, fieldName, origValue);
                    
                    content.text = GDEConstants.RuntimeVarValLbl;
                    size.x = 200f;
                    newValue = EditorGUI.Vector3Field(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), content, modValue);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    if (newValue != modValue && newValue != origValue)
                    {
                        GDEDataManager.SetVector3(itemKey, fieldName, newValue);
                        SetNeedToSaveModData(true);
                    }
                    else if (newValue != modValue && newValue == origValue)
                    {
                        GDEDataManager.ResetToDefault(itemKey, fieldName);
                        SetNeedToSaveModData(true);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        void DrawVector4(string itemKey, string fieldName, Dictionary<string, object> data, string label)
        {
            try
            {
                if (!runtimeValues)
                    DrawVector4(fieldName, data, label);
                else
                {
                    Vector4 origValue;
                    Vector4 modValue;
                    Vector4 newValue;
                    
                    data.TryGetVector4(fieldName, out origValue);
                    modValue = GDEDataManager.GetVector4(itemKey, fieldName, origValue);
                    
                    content.text = GDEConstants.RuntimeVarValLbl;
                    size.x = 228f;
                    newValue = EditorGUI.Vector4Field(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), content.text, modValue);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    if (newValue != modValue && newValue != origValue)
                    {
                        GDEDataManager.SetVector4(itemKey, fieldName, newValue);
                        SetNeedToSaveModData(true);
                    }
                    else if (newValue != modValue && newValue == origValue)
                    {
                        GDEDataManager.ResetToDefault(itemKey, fieldName);
                        SetNeedToSaveModData(true);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        void DrawColor(string itemKey, string fieldName, Dictionary<string, object> data, string label)
        {
            try
            {
                if (!runtimeValues)
                    DrawColor(fieldName, data, label);
                else
                {
                    Color origValue;
                    Color modValue;
                    Color newValue;
                    
                    data.TryGetColor(fieldName, out origValue);
                    modValue = GDEDataManager.GetColor(itemKey, fieldName, origValue);
                    
                    content.text = GDEConstants.RuntimeVarValLbl;
                    drawHelper.TryGetCachedSize(GDEConstants.SizeRuntimeVarValLblKey, content, EditorStyles.label, out size);
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content);
                    drawHelper.CurrentLinePosition += (size.x + 2);

                    size.x = 230 - size.x;
                    newValue = EditorGUI.ColorField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), modValue);
                    drawHelper.CurrentLinePosition += (size.x + 2);

                    if (newValue != modValue && newValue != origValue)
                    {
                        GDEDataManager.SetColor(itemKey, fieldName, newValue);
                        SetNeedToSaveModData(true);
                    }
                    else if (newValue != modValue && newValue == origValue)
                    {
                        GDEDataManager.ResetToDefault(itemKey, fieldName);
                        SetNeedToSaveModData(true);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        void DrawCustom(string itemKey, string fieldName, Dictionary<string, object> data, List<string> possibleValues = null)
        {
            try
            {
                if (!runtimeValues)
                    DrawCustom(fieldName, data, true, possibleValues);
                else
                {
                    string origValue;
                    string modValue;
                    string newValue;

                    int origIndex;
                    int modIndex;
                    int newIndex;

                    if (possibleValues != null)
                    {
                        data.TryGetString(fieldName, out origValue);
                        origIndex = possibleValues.IndexOf(origValue);

                        modValue = GDEDataManager.GetString(itemKey, fieldName, origValue);
                        modIndex = possibleValues.IndexOf(modValue);
                        
                        content.text = GDEConstants.RuntimeVarValLbl;
                        drawHelper.TryGetCachedSize(GDEConstants.SizeRuntimeVarValLblKey, content, EditorStyles.label, out size);
                        EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content);
                        drawHelper.CurrentLinePosition += (size.x + 2);
                        
                        size.x = 80;
                        newIndex = EditorGUI.Popup(new Rect(drawHelper.CurrentLinePosition, drawHelper.PopupTop(), size.x, drawHelper.StandardHeight()), modIndex, possibleValues.ToArray());
                        drawHelper.CurrentLinePosition += (size.x + 2);

                        if (possibleValues.IsValidIndex(newIndex))
                        {
                            newValue = possibleValues[newIndex];
                            if (newIndex != modIndex && newIndex != origIndex)
                            {
                                GDEDataManager.SetString(itemKey, fieldName, newValue);
                                SetNeedToSaveModData(true);
                            }
                            else if (newIndex != modIndex && newIndex == origIndex)
                            {
                                GDEDataManager.ResetToDefault(itemKey, fieldName);
                                SetNeedToSaveModData(true);
                            }
                        }
                    }
                    else
                    {
                        content.text = GDEConstants.DefaultValueLbl + " null";
                        drawHelper.TryGetCachedSize(GDEConstants.SizeDefaultValueLblKey, content, EditorStyles.label, out size);
                        EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content);
                        drawHelper.CurrentLinePosition += (size.x + 4);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        void DrawObject<T>(string itemKey, string fieldKey, string fieldName, Dictionary<string, object> data, string label) where T : UnityEngine.Object
        {
            try
            {
                if (!runtimeValues)
                    DrawObject<T>(fieldKey, fieldName, data, label);
                else
                {
                    T origValue;
                    T modValue;

                    object tmp;
                    data.TryGetValue(fieldName, out tmp);
                    origValue = tmp as T;

                    modValue = GDEDataManager.GetUnityObject(itemKey, fieldName, origValue);
                    
                    content.text = GDEConstants.RuntimeVarValLbl;
                    drawHelper.TryGetCachedSize(GDEConstants.SizeRuntimeVarValLblKey, content, EditorStyles.label, out size);
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    size.x = 230f - size.x;

                    GUI.enabled = false;
                    EditorGUI.ObjectField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), modValue as T, typeof(T), false);
                    GUI.enabled = true;

                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    DrawPreview<T>(fieldKey, modValue);
                }
            }
            catch(Exception ex)
            {
                // Don't log ExitGUIException here. This is a unity bug with ObjectField and ColorField.
                if (!(ex is ExitGUIException))
                    Debug.LogError(ex);
            }
        }

        void DrawAudio(string itemKey, string fieldKey, string fieldName, Dictionary<string, object> data, string label)
        {
            try
            {
                if (!runtimeValues)
                    DrawAudio(fieldKey, fieldName, data, label);
                else
                {
                    AudioClip origClip;
                    AudioClip modClip;

                    object tmp;
                    data.TryGetValue(fieldName, out tmp);
                    origClip = tmp as AudioClip;

                    modClip = GDEDataManager.GetUnityObject(itemKey, fieldName, origClip);
                    
                    content.text = GDEConstants.RuntimeVarValLbl;
                    drawHelper.TryGetCachedSize(GDEConstants.SizeRuntimeVarValLblKey, content, EditorStyles.label, out size);
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    size.x = 230f - size.x;

                    GUI.enabled = false;
                    EditorGUI.ObjectField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), modClip as AudioClip, typeof(AudioClip), false);
                    GUI.enabled = true;

                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    DrawAudioPreview(fieldKey, modClip);
                }
            }
            catch(Exception ex)
            {
                // Don't log ExitGUIException here. This is a unity bug with ObjectField and ColorField.
                if (!(ex is ExitGUIException))
                    Debug.LogError(ex);
            }
        }

        bool DrawListBool(string itemKey, string fieldName, GUIContent label, int index, bool value, IList boolList)
        {
            try
            {
                if (!runtimeValues)
                    DrawListBool(label, index, value, boolList);
                else
                {
                    bool newValue;
                    
                    drawHelper.TryGetCachedSize(label.text, label, EditorStyles.label, out size);
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), label);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    size.x = 30;
                    newValue = EditorGUI.Toggle(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), value);
                    drawHelper.CurrentLinePosition += (size.x + 2);

                    if (value != newValue)
                    {
                        boolList[index] = newValue;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return false;
        }

        bool DrawListInt(string itemKey, string fieldName, GUIContent label, int index, int value, IList intList)
        {
            try
            {
                if (!runtimeValues)
                    DrawListInt(label, index, value, intList);
                else
                {
                    int newValue;
                    
                    drawHelper.TryGetCachedSize(label.text, label, EditorStyles.label, out size);
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), label);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    size.x = 50;
                    newValue = EditorGUI.IntField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), value);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    if (value != newValue)
                    {
                        intList[index] = newValue;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            
            return false;
        }

        bool DrawListFloat(string itemKey, string fieldName, GUIContent label, int index, float value, IList floatList)
        {
            try
            {
                if (!runtimeValues)
                    DrawListFloat(label, index, value, floatList);
                else
                {
                    float newValue;
                    
                    drawHelper.TryGetCachedSize(label.text, label, EditorStyles.label, out size);
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), label);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    size.x = 50;
                    newValue = EditorGUI.FloatField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), value);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    if (value != newValue)
                    {
                        floatList[index] = newValue;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return false;
        }

        bool DrawListString(string itemKey, string fieldName, GUIContent label, int index, string value, IList stringList)
        {
            try
            {
                if (!runtimeValues)
                    DrawListString(label, index, value, stringList);
                else
                {
                    drawHelper.TryGetCachedSize(label.text, label, EditorStyles.label, out size);
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), label);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    string newValue = DrawResizableTextBox(value);
                    
                    if (!value.Equals(newValue))
                    {
                        stringList[index] = newValue;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return false;
        }

        bool DrawListVector2(string itemKey, string fieldName, GUIContent label, int index, Dictionary<string, object> value, IList vectorList)
        {
            try
            {
                if (!runtimeValues)
                    DrawListVector2(label, index, value, vectorList);
                else
                {
                    Vector2 newValue;
                    Vector2 currentValue = (Vector2)vectorList[index];

                    size.x = 136;
                    newValue = EditorGUI.Vector2Field(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.VectorFieldHeight()), label.text, currentValue);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    if (newValue != currentValue)
                    {
                        vectorList[index] = newValue;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return false;
        }

        bool DrawListVector3(string itemKey, string fieldName, GUIContent label, int index, Dictionary<string, object> value, IList vectorList)
        {
            try
            {
                if (!runtimeValues)
                    DrawListVector3(label, index, value, vectorList);
                else
                {
                    Vector3 newValue;
                    Vector3 currentValue = (Vector3)vectorList[index];
                    
                    size.x = 200;
                    newValue = EditorGUI.Vector3Field(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.VectorFieldHeight()), label.text, currentValue);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    if (newValue != currentValue)
                    {
                        vectorList[index] = newValue;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return false;
        }

        bool DrawListVector4(string itemKey, string fieldName, GUIContent label, int index, Dictionary<string, object> value, IList vectorList)
        {
            try
            {
                if (!runtimeValues)
                    DrawListVector4(label, index, value, vectorList);
                else
                {
                    Vector4 newValue;
                    Vector4 currentValue = (Vector4)vectorList[index];
                    
                    size.x = 228;
                    newValue = EditorGUI.Vector4Field(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.VectorFieldHeight()), label.text, currentValue);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    if (newValue != currentValue)
                    {
                        vectorList[index] = newValue;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return false;
        }

        bool DrawListColor(string itemKey, string fieldName, GUIContent label, int index, Dictionary<string, object> value, IList colorList)
        {
            try
            {
                if (!runtimeValues)
                    DrawListColor(label, index, value, colorList);
                else
                {
                    Color newValue;
                    Color currentValue = (Color)colorList[index];
                    
                    drawHelper.TryGetCachedSize(label.text, label, EditorStyles.label, out size);
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), label);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    size.x = 230 - size.x;
                    newValue = EditorGUI.ColorField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, drawHelper.StandardHeight()), currentValue);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    if (newValue != currentValue)
                    {
                        colorList[index] = newValue;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Don't log ExitGUIException here. This is a unity bug with ObjectField and ColorField.
                if (!(ex is ExitGUIException))
                    Debug.LogError(ex);
            }

            return false;
        }

        bool DrawListAudio(string itemKey, string fieldName, string fieldKey, GUIContent label, int index, AudioClip value, IList goList)
        {
            try
            {
                if (!runtimeValues)
                    DrawListAudio(fieldKey, label, index, value, goList);
                else
                {
                    drawHelper.TryGetCachedSize(label.text, label, EditorStyles.label, out size);
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), label);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    size.x = 230f - size.x;

                    GUI.enabled = false;
                    EditorGUI.ObjectField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), value, typeof(AudioClip), false);
                    GUI.enabled = true;

                    drawHelper.CurrentLinePosition += (size.x + 2);
                    DrawAudioPreview(fieldKey, value);
                }
            }
            catch (Exception ex)
            {
                // Don't log ExitGUIException here. This is a unity bug with ObjectField and ColorField.
                if (!(ex is ExitGUIException))
                    Debug.LogError(ex);
            }

            return false;
        }

        bool DrawListObject<T>(string itemKey, string fieldName, string fieldKey, GUIContent label, int index, T value, IList goList) where T : UnityEngine.Object
        {
            try
            {
                if (!runtimeValues)
                    DrawListObject<T>(fieldKey, label, index, value, goList);
                else
                {
                    drawHelper.TryGetCachedSize(label.text, label, EditorStyles.label, out size);
                    EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), label);
                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    size.x = 230f - size.x;

                    GUI.enabled = false;
                    EditorGUI.ObjectField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), value, typeof(T), false);
                    GUI.enabled = true;

                    drawHelper.CurrentLinePosition += (size.x + 2);
                    
                    DrawPreview<T>(fieldKey, value);
                }
            }
            catch (Exception ex)
            {
                // Don't log ExitGUIException here. This is a unity bug with ObjectField and ColorField.
                if (!(ex is ExitGUIException))
                    Debug.LogError(ex);
            }

            return false;
        }

        bool DrawListCustom(string itemKey, string fieldName, GUIContent label, int index, string value, IList customList, List<string> possibleValues = null)
        {
            try
            {
                if (!runtimeValues)
                    DrawListCustom(label, index, value, customList, true, possibleValues);
                else
                {
                    int newIndex;
                    int currentIndex;
                    
                    if (possibleValues != null)
                    {
                        currentIndex = possibleValues.IndexOf(value);
                        
                        drawHelper.TryGetCachedSize(label.text, label, EditorStyles.label, out size);
                        EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), label);
                        drawHelper.CurrentLinePosition += (size.x + 2);
                        
                        size.x = 80;
                        newIndex = EditorGUI.Popup(new Rect(drawHelper.CurrentLinePosition, drawHelper.PopupTop(), size.x, drawHelper.StandardHeight()), currentIndex, possibleValues.ToArray());
                        drawHelper.CurrentLinePosition += (size.x + 2);
                        
                        if (newIndex != currentIndex && possibleValues.IsValidIndex(newIndex))
                        {
                            customList[index] = possibleValues[newIndex];
                            return true;
                        }
                    }
                    else
                    {
                        label.text += " null";
                        drawHelper.TryGetCachedSize(label.text, label, EditorStyles.label, out size);
                        EditorGUI.LabelField(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), label);
                        drawHelper.CurrentLinePosition += (size.x + 2);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }

            return false;
        }

        protected override void DrawEntryFooter(string cloneLbl, string cloneSizeKey, string entryKey)
        {
            if (!runtimeValues)
                base.DrawEntryFooter(cloneLbl, cloneSizeKey, entryKey);
      			#if !UNITY_WEBPLAYER
            else
            {
                content.text = GDEConstants.ResetBtn;
                drawHelper.TryGetCachedSize(GDEConstants.SizeResetBtnKey, content, GUI.skin.button, out size);
                if (GUI.Button(new Rect(drawHelper.CurrentLinePosition, drawHelper.TopOfLine(), size.x, size.y), content))
                    deleteEntries.Add(entryKey);
          
                drawHelper.NewLine();
                drawHelper.DrawSectionSeparator();
                drawHelper.NewLine(0.25f);
            }
      			#else
      			else
      			{
        				drawHelper.NewLine();
        				drawHelper.DrawSectionSeparator();
        				drawHelper.NewLine(0.25f);
            }
      			#endif
        }
        
        List<string> GetPossibleCustomValues(string fieldKey, string fieldType)
        {
            object temp;
            List<string> itemKeys = new List<string>();
            itemKeys.Add("null");

            if (!runtimeValues)
            {
                // Build a list of possible custom field values
                // All items that match the schema type of the custom field type
                // will be added to the selection list
                foreach(KeyValuePair<string, Dictionary<string, object>> item in GDEItemManager.AllItems)
                {
                    string itemType = "<unknown>";
                    Dictionary<string, object> itemData = item.Value as Dictionary<string, object>;

                    if (itemData.TryGetValue(GDMConstants.SchemaKey, out temp))
                        itemType = temp as string;

                    if (item.Key.Equals(fieldKey) || !itemType.Equals(fieldType))
                        continue;

                    itemKeys.Add(item.Key);
                }
            }
            else
            {
                // Get any runtime items if Show Runtime Values is checked
                List<string> allKeysOfType;
                if (GDEDataManager.GetAllDataKeysBySchema(fieldType, out allKeysOfType))
                {
                    allKeysOfType.Remove(fieldKey);
                    itemKeys.AddRange(allKeysOfType);
                }
            }

            return itemKeys;
        }
        #endregion

        #region Filter Methods
        protected override Dictionary<string, Dictionary<string, object>> GetEntriesToDraw(Dictionary<string, Dictionary<string, object>> source)
        {
            var entries = base.GetEntriesToDraw(GDEItemManager.AllItems);

            if (runtimeValues)
            {
                var runtimeEntries = base.GetEntriesToDraw(GDEDataManager.ModifiedData);
                foreach(var entry in runtimeEntries)
                {
                    if (entries.ContainsKey(entry.Key))
                        continue;

                    string schema;
                    entry.Value.TryGetString(GDMConstants.SchemaKey, out schema);

                    Dictionary<string, object> schemaDict;
                    if (GDEItemManager.AllSchemas.TryGetValue(schema, out schemaDict))
                    {
                        foreach(var pair in schemaDict)
                        {
                            if (!entry.Value.ContainsKey(pair.Key))
                                entry.Value.Add(pair.Key, pair.Value);
                        }

                        entries.Add(entry.Key, entry.Value);
                    }
                }
            }

            return entries;
        }

        protected override bool ShouldFilter(string itemKey, Dictionary<string, object> itemData)
        {
            if (itemData == null)
                return true;

            string schemaType = "<unknown>";
            itemData.TryGetString(GDMConstants.SchemaKey, out schemaType);

            // Return if we don't match any of the filter types
            if (GDEItemManager.FilterSchemaKeyArray.IsValidIndex(filterSchemaIndex) &&
                !GDEItemManager.FilterSchemaKeyArray[filterSchemaIndex].Equals(GDEConstants.AllLbl) &&
                !schemaType.Equals(GDEItemManager.FilterSchemaKeyArray[filterSchemaIndex]))
                return true;
        		else if (!GDEItemManager.FilterSchemaKeyArray[filterSchemaIndex].Equals(GDEConstants.AllLbl) &&
    		         schemaType.Equals(GDEItemManager.FilterSchemaKeyArray[filterSchemaIndex]) &&
    		         string.IsNullOrEmpty(filterText))
      			return false;

            bool schemaKeyMatch = schemaType.ToLower().Contains(filterText.ToLower());
            bool fieldKeyMatch = !GDEItemManager.ShouldFilterByField(schemaType, filterText);
            bool itemKeyMatch = itemKey.ToLower().Contains(filterText.ToLower());

            // Return if the schema keys don't contain the filter text or
            // if the schema fields don't contain the filter text
            if (!schemaKeyMatch && !fieldKeyMatch && !itemKeyMatch)
                return true;

            return false;
        }

        protected override void ClearSearch()
        {
            base.ClearSearch();
            filterSchemaIndex = GDEItemManager.FilterSchemaKeyArray.ToList().IndexOf(GDEConstants.AllLbl);
        }
        #endregion

        #region Load/Save/Create/Remove Item Methods
        protected override void Load()
        {
            base.Load();

            newItemName = string.Empty;
            schemaIndex = 0;
            filterSchemaIndex = 0;
            renamedItems.Clear();
        }

        protected override bool Create(object data)
        {
            bool result = true;
            List<object> args = data as List<object>;
            string schemaKey = args[0] as string;
            string itemName = args[1] as string;

            Dictionary<string, object> schemaData = null;
            if (GDEItemManager.AllSchemas.TryGetValue(schemaKey, out schemaData))
            {
                Dictionary<string, object> itemData = schemaData.DeepCopy();
                itemData.Add(GDMConstants.SchemaKey, schemaKey);

                string error;
                if (GDEItemManager.AddItem(itemName, itemData, out error))
                {
                    SetFoldout(true, itemName);
                    SetNeedToSave(true);

    				HighlightNew(itemName);
                }
                else
                {
                    result = false;
                    EditorUtility.DisplayDialog(GDEConstants.ErrorCreatingItem, error, GDEConstants.OkLbl);
                }
            }
            else
            {
                result = false;
                EditorUtility.DisplayDialog(GDEConstants.ErrorLbl, GDEConstants.SchemaNotFound + ": " + schemaKey, GDEConstants.OkLbl);
            }

            return result;
        }

    	protected override bool Clone(string key)
    	{
    		bool result = true;
    		string error;
    		string newKey;

    		result = GDEItemManager.CloneItem(key, out newKey, out error);
    		if (result)
    		{
    			SetNeedToSave(true);
    			SetFoldout(true, newKey);

    			HighlightNew(newKey);
    		}
    		else
    		{
    			EditorUtility.DisplayDialog(GDEConstants.ErrorCloningItem, error, GDEConstants.OkLbl);
    			result = false;
    		}

    		return result;
    	}

        protected override void Remove(string key)
        {
            GDEItemManager.RemoveItem(key);
            SetNeedToSave(true);
        }

        protected override bool NeedToSave()
        {
            return GDEItemManager.ItemsNeedSave || GDEItemManager.ModDataNeedSave;
        }

        protected void SetNeedToSaveModData(bool shouldSave)
        {
            GDEItemManager.ModDataNeedSave = shouldSave;
        }

        protected override void SetNeedToSave(bool shouldSave)
        {
            GDEItemManager.ItemsNeedSave = shouldSave;
        }
        #endregion

        #region Helper Methods
        void SetSchemaHeight(string schemaKey, float groupHeight)
        {
    		if (!groupHeight.NearlyEqual(drawHelper.LineHeight))
    	        groupHeightBySchema[schemaKey] = groupHeight;
        }

    	protected override float CalculateGroupHeightsTotal()
        {
    		if (!shouldRecalculateHeights)
    			return currentGroupHeightTotal;

    		currentGroupHeightTotal = 0;
    		float itemHeight = 0;
            float schemaHeight = 0;
            string schema = string.Empty;

            foreach(var item in entriesToDraw)
            {
                groupHeights.TryGetValue(item.Key, out itemHeight);
    			if (itemHeight < GDEConstants.LineHeight)
    			{
    				itemHeight = GDEConstants.LineHeight;
    				SetGroupHeight(item.Key, itemHeight);
    			}

    			//Check to see if this item's height has been updated
                //otherwise use the min height for the schema
                if (entryFoldoutState.Contains(item.Key) && itemHeight.NearlyEqual(GDEConstants.LineHeight))
                {
                    schema = GDEItemManager.GetSchemaForItem(item.Key);
                    groupHeightBySchema.TryGetValue(schema, out schemaHeight);

    				// Only use the schema height if its greater than
    				// the default item height
    				if (schemaHeight > itemHeight)
    				{
    					currentGroupHeightTotal += schemaHeight;
    					SetGroupHeight(item.Key, schemaHeight);
    				}
    				else
    					currentGroupHeightTotal += itemHeight;
                }
                else
                    currentGroupHeightTotal += itemHeight;
            }

    		shouldRecalculateHeights = false;

            return currentGroupHeightTotal;
        }

        protected override string FilePath()
        {
            return GDEItemManager.DataFilePath;
        }

        void SetRuntimeListValue(string item, string field, IList val, BasicFieldType type, int dimension = 0)
        {
            switch (type)
            {
                case BasicFieldType.Bool:
                {
                    if (dimension == 1)
                    {
                        List<bool> tmp = val as List<bool>;
                        GDEDataManager.SetBoolList(item, field, tmp);
                    }
                    else if (dimension == 2)
                    {
                        List<List<bool>> tmp = val as List<List<bool>>;
                        GDEDataManager.SetBoolTwoDList(item, field, tmp);
                    }
                    break;
                }
                case BasicFieldType.Int:
                {
                    if (dimension == 1)
                    {
                        List<int> tmp = val as List<int>;
                        GDEDataManager.SetIntList(item, field, tmp);
                    }
                    else if (dimension == 2)
                    {
                        List<List<int>> tmp = val as List<List<int>>;
                        GDEDataManager.SetIntTwoDList(item, field, tmp);
                    }
                    break;
                }
                case BasicFieldType.Float:
                {
                    if (dimension == 1)
                    {
                        List<float> tmp = val as List<float>;
                        GDEDataManager.SetFloatList(item, field, tmp);
                    }
                    else if (dimension == 2)
                    {
                        List<List<float>> tmp = val as List<List<float>>;
                        GDEDataManager.SetFloatTwoDList(item, field, tmp);
                    }
                    break;
                }
                case BasicFieldType.Vector2:
                {
                    if (dimension == 1)
                    {
                        List<Vector2> tmp = val as List<Vector2>;
                        GDEDataManager.SetVector2List(item, field, tmp);
                    }
                    else if (dimension == 2)
                    {
                        List<List<Vector2>> tmp = val as List<List<Vector2>>;
                        GDEDataManager.SetVector2TwoDList(item, field, tmp);
                    }
                    break;
                }
                case BasicFieldType.Vector3:
                {
                    if (dimension == 1)
                    {
                        List<Vector3> tmp = val as List<Vector3>;
                        GDEDataManager.SetVector3List(item, field, tmp);
                    }
                    else if (dimension == 2)
                    {
                        List<List<Vector3>> tmp = val as List<List<Vector3>>;
                        GDEDataManager.SetVector3TwoDList(item, field, tmp);
                    }
                    break;
                }
                case BasicFieldType.Vector4:
                {
                    if (dimension == 1)
                    {
                        List<Vector4> tmp = val as List<Vector4>;
                        GDEDataManager.SetVector4List(item, field, tmp);
                    }
                    else if (dimension == 2)
                    {
                        List<List<Vector4>> tmp = val as List<List<Vector4>>;
                        GDEDataManager.SetVector4TwoDList(item, field, tmp);
                    }
                    break;
                }
                case BasicFieldType.Color:
                {
                    if (dimension == 1)
                    {
                        List<Color> tmp = val as List<Color>;
                        GDEDataManager.SetColorList(item, field, tmp);
                    }
                    else if (dimension == 2)
                    {
                        List<List<Color>> tmp = val as List<List<Color>>;
                        GDEDataManager.SetColorTwoDList(item, field, tmp);
                    }
                    break;
                }
                case BasicFieldType.GameObject:
                {
                    if (dimension == 1)
                    {
                        List<GameObject> tmp = val as List<GameObject>;
                        GDEDataManager.SetUnityObjectList(item, field, tmp);
                    }
                    else if (dimension == 2)
                    {
                        List<List<GameObject>> tmp = val as List<List<GameObject>>;
                        GDEDataManager.SetUnityObjectTwoDList(item, field, tmp);
                    }
                    break;
                }
                case BasicFieldType.Texture2D:
                {
                    if (dimension == 1)
                    {
                        List<Texture2D> tmp = val as List<Texture2D>;
                        GDEDataManager.SetUnityObjectList(item, field, tmp);
                    }
                    else if (dimension == 2)
                    {
                        List<List<Texture2D>> tmp = val as List<List<Texture2D>>;
                        GDEDataManager.SetUnityObjectTwoDList(item, field, tmp);
                    }
                    break;
                }
                case BasicFieldType.Material:
                {
                    if (dimension == 1)
                    {
                        List<Material> tmp = val as List<Material>;
                        GDEDataManager.SetUnityObjectList(item, field, tmp);
                    }
                    else if (dimension == 2)
                    {
                        List<List<Material>> tmp = val as List<List<Material>>;
                        GDEDataManager.SetUnityObjectTwoDList(item, field, tmp);
                    }
                    break;
                }
                case BasicFieldType.AudioClip:
                {
                    if (dimension == 1)
                    {
                        List<AudioClip> tmp = val as List<AudioClip>;
                        GDEDataManager.SetUnityObjectList(item, field, tmp);
                    }
                    else if (dimension == 2)
                    {
                        List<List<AudioClip>> tmp = val as List<List<AudioClip>>;
                        GDEDataManager.SetUnityObjectTwoDList(item, field, tmp);
                    }
                    break;
                }
                case BasicFieldType.String:
                default:
                {
                    if (dimension == 1)
                    {
                        List<string> tmp = val as List<string>;
                        GDEDataManager.SetStringList(item, field, tmp);
                    }
                    else if (dimension == 2)
                    {
                        List<List<string>> tmp = val as List<List<string>>;
                        GDEDataManager.SetStringTwoDList(item, field, tmp);
                    }
                    break;
                }
            }

            SetNeedToSaveModData(true);
        }
        #endregion
        
        #region Rename Methods
        protected bool RenameItem(string oldItemKey, string newItemKey, Dictionary<string, object> data, out string error)
        {
            error = string.Empty;
            renamedItems.Add(oldItemKey, newItemKey);
            return true;
        }
        #endregion
    }
}
