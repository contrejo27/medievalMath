using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace EasyMobile.Editor
{
    [CustomPropertyDrawer(typeof(AdSettings.AdMobTargetingSettings.Extra))]
    public class AdMobTargetingExtraDrawer : KeyValuePairDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            keyFieldName = "key";
            valueFieldName = "value";
            base.OnGUI(position, property, label);  
        }
    }
}