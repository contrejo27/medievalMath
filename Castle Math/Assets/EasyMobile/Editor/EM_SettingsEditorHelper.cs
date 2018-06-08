using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace EasyMobile.Editor
{
    public partial class EM_SettingsEditor
    {
        private struct ArrayItemToolboxButtonResults
        {
            public bool isDeleteButton;
            public bool isMoveUpButton;
            public bool isMoveDownButton;
        }

        /// <summary>
        /// Draws the array element control toolbox with standard buttons (Move Up, Move Down, Delete).
        /// </summary>
        /// <param name="buttonResults">Button results.</param>
        /// <param name="allowMoveUp">If set to <c>true</c> allow move up.</param>
        /// <param name="allowMoveDown">If set to <c>true</c> allow move down.</param>
        static void DrawArrayElementControlToolbox(ref ArrayItemToolboxButtonResults buttonResults, bool allowMoveUp = true, bool allowMoveDown = true)
        {
            EditorGUILayout.BeginVertical(EM_GUIStyleManager.GetCustomStyle("Tool Box"), GUILayout.Width(EM_GUIStyleManager.toolboxWidth), GUILayout.Height(EM_GUIStyleManager.toolboxHeight));

            // Move up button.
            EditorGUI.BeginDisabledGroup(!allowMoveUp);
            if (GUILayout.Button(EM_Constants.UpSymbol, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                buttonResults.isMoveUpButton = true;
            }
            EditorGUI.EndDisabledGroup();

            // Delete button.
            if (GUILayout.Button(EM_Constants.DeleteSymbol, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                // DeleteArrayElementAtIndex seems working fine even while iterating
                // through the array, but it's still a better idea to move it outside the loop.
                buttonResults.isDeleteButton = true;
            }

            // Move down button.
            EditorGUI.BeginDisabledGroup(!allowMoveDown);
            if (GUILayout.Button(EM_Constants.DownSymbol, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                buttonResults.isMoveDownButton = true;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws an array element with toolbox (Delete, Move Up & Move Down buttons).
        /// </summary>
        /// <param name="element">Element.</param>
        /// <param name="elementDrawer">Element drawer.</param>
        /// <param name="buttonResults">Button results.</param>
        /// <param name="allowMoveUp">If set to <c>true</c> allow move up.</param>
        /// <param name="allowMoveDown">If set to <c>true</c> allow move down.</param>
        static void DrawArrayElementWithToolbox(SerializedProperty element, Action<SerializedProperty> elementDrawer, ref ArrayItemToolboxButtonResults buttonResults, bool allowMoveUp, bool allowMoveDown)
        {
            EditorGUILayout.BeginHorizontal();

            // Draw array element
            elementDrawer(element);

            // Draw control toolbox
            DrawArrayElementControlToolbox(ref buttonResults, allowMoveUp, allowMoveDown);

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws an array property, each element is associated with a standard toolbox with Delete, Move Up & Move Down buttons.
        /// </summary>
        /// <param name="property">Property.</param>
        /// <param name="elementDrawer">Element drawer.</param>
        static void DrawArrayProperty(SerializedProperty property, Action<SerializedProperty> elementDrawer)
        {
            if (!property.isArray)
            {
                Debug.Log("Invalid argument. Require array property.");
                return;
            }

            // Index of the element on which buttons are clicked.
            int deleteIndex = -1;
            int moveUpIndex = -1;
            int moveDownIndex = -1;

            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty element = property.GetArrayElementAtIndex(i);

                var buttonResults = new ArrayItemToolboxButtonResults();
                buttonResults.isDeleteButton = false;
                buttonResults.isMoveUpButton = false;   
                buttonResults.isMoveDownButton = false;

                DrawArrayElementWithToolbox(
                    element,
                    elementDrawer,
                    ref buttonResults,
                    i > 0,
                    i < property.arraySize - 1
                );

                if (buttonResults.isDeleteButton)
                    deleteIndex = i;
                if (buttonResults.isMoveUpButton)
                    moveUpIndex = i;
                if (buttonResults.isMoveDownButton)
                    moveDownIndex = i;
            }

            // Delete.
            if (deleteIndex >= 0)
            {
                property.DeleteArrayElementAtIndex(deleteIndex);
            }

            // Move up.
            if (moveUpIndex > 0)
            {
                property.MoveArrayElement(moveUpIndex, moveUpIndex - 1);
            }

            // Move down.
            if (moveDownIndex >= 0 && moveDownIndex < property.arraySize - 1)
            {
                property.MoveArrayElement(moveDownIndex, moveDownIndex + 1);
            }
        }

        static string DrawListAsPopup(GUIContent label, string[] values, string currentVal, params GUILayoutOption[] options)
        {
            var contents = new GUIContent[values.Length];

            for (int i = 0; i < values.Length; i++)
                contents[i] = new GUIContent(values[i]);

            // If the current value doesn't belong to the list, select the first value, which normally should be "None".
            int currentIndex = Mathf.Max(System.Array.IndexOf(values, currentVal), 0);                           
            int newIndex = EditorGUILayout.Popup(
                               label, 
                               currentIndex, 
                               contents,
                               options
                           );
                
            return values[newIndex];
        }
    }
}