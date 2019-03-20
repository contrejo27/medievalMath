using UnityEngine;
using UnityEditor;
using MSP_Input;

[CustomEditor (typeof(GUIDraw))]
public class GUIDrawEditor : Editor 
{
	private GUIDraw guiDrawScript;

	public override void OnInspectorGUI() 
	{
//		base.OnInspectorGUI();

		string helpText = "Do not remove: " +
			"This component is part of MSP_Input and handles the drawing of " +
			"VirtualTouchpads, VirtualJoysticks and VirtualButtons.";
		EditorGUILayout.HelpBox(helpText,MessageType.None, true);

		guiDrawScript = target as GUIDraw;

		guiDrawScript.showInEditMode = EditorGUILayout.Toggle ("show in edit mode", guiDrawScript.showInEditMode);


		if (GUI.changed) 
		{
			EditorUtility.SetDirty(guiDrawScript);    
		}
	}
}