using UnityEditor;
using MSP_Input;

[CustomEditor (typeof(ErrorHandling))]
public class ErrorHandlingEditor : Editor 
{
	public override void OnInspectorGUI() 
	{
		//base.OnInspectorGUI();
		string helpText = "Do not remove: This component is part of MSP_Input";
		EditorGUILayout.HelpBox(helpText,MessageType.None, true);
	}
}
