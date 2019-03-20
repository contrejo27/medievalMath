using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using MSP_Input;

[CustomEditor (typeof(VirtualTouchpad))]
public class VirtualTouchpadEditor : Editor
{
	private static bool showHelp = false;
	private VirtualTouchpad virtualTouchpadScript;

	//================================================================================

	public override void OnInspectorGUI() 
	{
		virtualTouchpadScript = target as VirtualTouchpad;

		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.PrefixLabel (" ");
			AddTouchpad ();
			ToggleHelp ();
		}
		EditorGUILayout.EndHorizontal();

		ShowHelp ();

		int i = 0;
		bool removeItem = false;
		int itemToRemove = -1;
		bool moveItem = false;
		int itemToMove = -1;
		int itemMoveDirection = 0;
		foreach (VirtualTouchpad.Touchpad touchpad in virtualTouchpadScript.virtualTouchpads) 
		{
			EditorGUILayout.BeginHorizontal();
			{
				touchpad.showSettingsInInspector = EditorGUILayout.Foldout (touchpad.showSettingsInInspector, "[" + (i+1) + "] " + touchpad.name);
				touchpad.enabled = EditorGUILayout.Toggle(" " ,touchpad.enabled);	
				if (GUILayout.Button("<"))
				{
					moveItem = true;
					itemToMove = i;
					itemMoveDirection = -1;
				}
				if (GUILayout.Button(">"))
				{
					moveItem = true;
					itemToMove = i;
					itemMoveDirection = 1;
				}
				EditorGUILayout.LabelField (" ", GUILayout.Width (8));
				if (GUILayout.Button("-"))
				{
					removeItem = true;
					itemToRemove = i;
				}
			}
			EditorGUILayout.EndHorizontal();
			
			if (touchpad.showSettingsInInspector) 
			{
				touchpad.name = EditorGUILayout.TextField ("Name", touchpad.name);
				EditorGUILayout.BeginHorizontal ();
				{
					EditorGUILayout.PrefixLabel ("Background Texture");
					touchpad.backgroundTexture = EditorGUILayout.ObjectField(touchpad.backgroundTexture,typeof(Texture),false) as Texture;
					touchpad.hideBackgroundTexture = EditorGUILayout.ToggleLeft("Hide",touchpad.hideBackgroundTexture, GUILayout.Width(40));
				}
				EditorGUILayout.EndHorizontal();

				if (touchpad.backgroundTexture) 
				{
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel("Horizontal");
						float horizontalMin = touchpad.touchpadRect.x;
						float horizontalMax = touchpad.touchpadRect.x + touchpad.touchpadRect.width;
						EditorGUILayout.MinMaxSlider(ref horizontalMin, ref horizontalMax, 0f, 1f);
						horizontalMin = Mathf.Round(horizontalMin * 100f) * 0.01f;
						horizontalMax = Mathf.Round(horizontalMax * 100f) * 0.01f;
						touchpad.touchpadRect.x = horizontalMin;
						touchpad.touchpadRect.width = horizontalMax - touchpad.touchpadRect.x;
						EditorGUILayout.LabelField(""+horizontalMin.ToString("F2")+"/"+horizontalMax.ToString("F2"), GUILayout.Width(60));
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel("Vertical");
						float verticalMin = 1f-(touchpad.touchpadRect.y + touchpad.touchpadRect.height); ;
						float verticalMax = 1f-touchpad.touchpadRect.y;
						EditorGUILayout.MinMaxSlider(ref verticalMin, ref verticalMax, 0f, 1f);
						verticalMin = Mathf.Round(verticalMin * 100f) * 0.01f;
						verticalMax = Mathf.Round(verticalMax * 100f) * 0.01f;
						touchpad.touchpadRect.y = (1f-verticalMax);
						touchpad.touchpadRect.height = (1f-verticalMin) - touchpad.touchpadRect.y;
						EditorGUILayout.LabelField(""+verticalMin.ToString("F2")+"/"+verticalMax.ToString("F2"), GUILayout.Width(60));
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					{
						touchpad.axisMultiplier = EditorGUILayout.Vector2Field("Axis Multiplier", touchpad.axisMultiplier);
					}
					EditorGUILayout.EndHorizontal();

					touchpad.compensateForDeviceRoll = EditorGUILayout.Toggle("Device Roll Compensation",touchpad.compensateForDeviceRoll);
					
					//
					// EditorSimulation
					//
					EditorGUILayout.LabelField("Editor Keyboard Simulation:");
					touchpad.editorTouchpadSimulation.simulationMode  = (VirtualTouchpad.EditorTouchpadSimulationMode) EditorGUILayout.EnumPopup("  * Touchpad", touchpad.editorTouchpadSimulation.simulationMode);
					switch (touchpad.editorTouchpadSimulation.simulationMode)
					{
					case VirtualTouchpad.EditorTouchpadSimulationMode.None:
						//touchpad.editorTouchpadSimulation.keyCode_Up = KeyCode.None;
						//touchpad.editorTouchpadSimulation.keyCode_Left = KeyCode.None;
						//touchpad.editorTouchpadSimulation.keyCode_Down = KeyCode.None;
						//touchpad.editorTouchpadSimulation.keyCode_Right = KeyCode.None;
						break;
					case VirtualTouchpad.EditorTouchpadSimulationMode.Keyboard_WASD:
						touchpad.editorTouchpadSimulation.keyCode_Up = KeyCode.W;
						touchpad.editorTouchpadSimulation.keyCode_Left = KeyCode.A;
						touchpad.editorTouchpadSimulation.keyCode_Down = KeyCode.S;
						touchpad.editorTouchpadSimulation.keyCode_Right = KeyCode.D;
						break;
					case VirtualTouchpad.EditorTouchpadSimulationMode.Keyboard_CursorKeys:
						touchpad.editorTouchpadSimulation.keyCode_Up = KeyCode.UpArrow;
						touchpad.editorTouchpadSimulation.keyCode_Left = KeyCode.LeftArrow;
						touchpad.editorTouchpadSimulation.keyCode_Down = KeyCode.DownArrow;
						touchpad.editorTouchpadSimulation.keyCode_Right = KeyCode.RightArrow;
						break;
					case VirtualTouchpad.EditorTouchpadSimulationMode.Keyboard_Custom:
						touchpad.editorTouchpadSimulation.keyCode_Left = (KeyCode) EditorGUILayout.EnumPopup("      Left", touchpad.editorTouchpadSimulation.keyCode_Left);
						touchpad.editorTouchpadSimulation.keyCode_Right = (KeyCode) EditorGUILayout.EnumPopup("      Right", touchpad.editorTouchpadSimulation.keyCode_Right);
						touchpad.editorTouchpadSimulation.keyCode_Up = (KeyCode) EditorGUILayout.EnumPopup("      Up", touchpad.editorTouchpadSimulation.keyCode_Up);
						touchpad.editorTouchpadSimulation.keyCode_Down = (KeyCode) EditorGUILayout.EnumPopup("      Down", touchpad.editorTouchpadSimulation.keyCode_Down);
						break;
					default:
						break;			
					}
					touchpad.editorTouchpadSimulation.keyCode_DoubleTap = (KeyCode) EditorGUILayout.EnumPopup("  * DoubleTap", touchpad.editorTouchpadSimulation.keyCode_DoubleTap);
					
					//
					// Auto Update
					//
					touchpad.autoUpdateEnabled = EditorGUILayout.Toggle("Auto Update",touchpad.autoUpdateEnabled);
					if (touchpad.autoUpdateEnabled)
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel(" ");
							touchpad.autoUpdateGyroAccelScript = EditorGUILayout.ToggleLeft(" GyroAccelScript",touchpad.autoUpdateGyroAccelScript);
						}
						EditorGUILayout.EndHorizontal();

						// SendMessage: Axis

						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel(" ");
							if (GUILayout.Button ("+")) 
							{
								touchpad.autoUpdateAxisMessageReceivers.Add(new VirtualTouchpad.SendMessageReceiver());
								int last = touchpad.autoUpdateAxisMessageReceivers.Count-1;
								touchpad.autoUpdateAxisMessageReceivers[last].messageReceiver = null;
								touchpad.autoUpdateAxisMessageReceivers[last].functionName = "Function to call";
							}
							EditorGUILayout.LabelField ("SendMessage: Axis");
						}
						EditorGUILayout.EndHorizontal();
						
						int j = 0;
						bool removeReceiver = false;
						int receiverToRemove = -1;
						foreach (VirtualTouchpad.SendMessageReceiver receiver in touchpad.autoUpdateAxisMessageReceivers) 
						{
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.PrefixLabel(" ");
								if (GUILayout.Button("-"))
								{
									removeReceiver = true;
									receiverToRemove = j;
								}
								receiver.messageReceiver = EditorGUILayout.ObjectField(receiver.messageReceiver,typeof(GameObject),true) as GameObject;
							}
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.PrefixLabel(" ");
								EditorGUILayout.LabelField ("",GUILayout.Width(20));
								receiver.functionName = EditorGUILayout.TextField ("", receiver.functionName);
							}
							EditorGUILayout.EndHorizontal();
							j++;
						}
						if (removeReceiver) 
						{
							touchpad.autoUpdateAxisMessageReceivers.RemoveAt(receiverToRemove);
						}

						// SendMessage: DoubleTap

						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel(" ");
							if (GUILayout.Button ("+")) 
							{
								touchpad.autoUpdateDoubleTapMessageReceivers.Add(new VirtualTouchpad.SendMessageReceiver());
								int last = touchpad.autoUpdateDoubleTapMessageReceivers.Count-1;
								touchpad.autoUpdateDoubleTapMessageReceivers[last].messageReceiver = null;
								touchpad.autoUpdateDoubleTapMessageReceivers[last].functionName = "Function to call";
							}
							EditorGUILayout.LabelField ("SendMessage: DoubleTap");
						}
						EditorGUILayout.EndHorizontal();
						
						j = 0;
						removeReceiver = false;
						receiverToRemove = -1;
						foreach (VirtualTouchpad.SendMessageReceiver receiver in touchpad.autoUpdateDoubleTapMessageReceivers) 
						{
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.PrefixLabel(" ");
								if (GUILayout.Button("-"))
								{
									removeReceiver = true;
									receiverToRemove = j;
								}
								receiver.messageReceiver = EditorGUILayout.ObjectField(receiver.messageReceiver,typeof(GameObject),true) as GameObject;
							}
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.PrefixLabel(" ");
								EditorGUILayout.LabelField ("",GUILayout.Width(20));
								receiver.functionName = EditorGUILayout.TextField ("", receiver.functionName);
							}
							EditorGUILayout.EndHorizontal();
							j++;
						}
						if (removeReceiver) 
						{
							touchpad.autoUpdateDoubleTapMessageReceivers.RemoveAt(receiverToRemove);
						}

						// SendMessage: DoubleTapHold
						
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel(" ");
							if (GUILayout.Button ("+")) 
							{
								touchpad.autoUpdateDoubleTapHoldMessageReceivers.Add(new VirtualTouchpad.SendMessageReceiver());
								int last = touchpad.autoUpdateDoubleTapHoldMessageReceivers.Count-1;
								touchpad.autoUpdateDoubleTapHoldMessageReceivers[last].messageReceiver = null;
								touchpad.autoUpdateDoubleTapHoldMessageReceivers[last].functionName = "Function to call";
							}
							EditorGUILayout.LabelField ("SendMessage: DoubleTapHold");
						}
						EditorGUILayout.EndHorizontal();
						
						j = 0;
						removeReceiver = false;
						receiverToRemove = -1;
						foreach (VirtualTouchpad.SendMessageReceiver receiver in touchpad.autoUpdateDoubleTapHoldMessageReceivers) 
						{
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.PrefixLabel(" ");
								if (GUILayout.Button("-"))
								{
									removeReceiver = true;
									receiverToRemove = j;
								}
								receiver.messageReceiver = EditorGUILayout.ObjectField(receiver.messageReceiver,typeof(GameObject),true) as GameObject;
							}
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.PrefixLabel(" ");
								EditorGUILayout.LabelField ("",GUILayout.Width(20));
								receiver.functionName = EditorGUILayout.TextField ("", receiver.functionName);
							}
							EditorGUILayout.EndHorizontal();
							j++;
						}
						if (removeReceiver) 
						{
							touchpad.autoUpdateDoubleTapHoldMessageReceivers.RemoveAt(receiverToRemove);
						}
					}
				}

				if (Application.isPlaying) 
				{
					EditorGUILayout.BeginHorizontal ();
					{
						EditorGUILayout.PrefixLabel ("Debug Info");
						EditorGUILayout.LabelField ("Axis = " + touchpad.axis);
					}
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.BeginHorizontal ();
					{
						EditorGUILayout.PrefixLabel (" ");
						EditorGUILayout.LabelField ("doubleTap = " + touchpad.doubleTap);
					}
					EditorGUILayout.EndHorizontal ();				
					EditorGUILayout.BeginHorizontal ();
					{
						EditorGUILayout.PrefixLabel (" ");
						EditorGUILayout.LabelField ("doubleTapHold = " + touchpad.doubleTapHold);
					}
					EditorGUILayout.EndHorizontal ();	
				} // if (touchpad.showDebugInfoInInspector)

			} // if (touchpad.showSettingsInInspector)
			EditorGUILayout.Space();
			i++;
		}
		
		if (removeItem) 
		{
			virtualTouchpadScript.virtualTouchpads.RemoveAt(itemToRemove);
		}

		if (moveItem) 
		{
			int itemToSwap = Mathf.Clamp (itemToMove + itemMoveDirection, 0, i - 1);
			VirtualTouchpad.Touchpad tmpItem = virtualTouchpadScript.virtualTouchpads[itemToSwap];
			virtualTouchpadScript.virtualTouchpads[itemToSwap] =  virtualTouchpadScript.virtualTouchpads[itemToMove];
			virtualTouchpadScript.virtualTouchpads[itemToMove] =  tmpItem;
		}
		
		EditorGUILayout.Space ();
		
		if (GUI.changed) 
		{
			EditorUtility.SetDirty(virtualTouchpadScript);    
		}

	} // public override void OnInspectorGUI() 

	//================================================================================
	
	void ToggleHelp()
	{
		if (virtualTouchpadScript.virtualTouchpads.Count > 0) 
		{
			showHelp = EditorGUILayout.ToggleLeft ("show help", showHelp, GUILayout.Width (80));
		} else {
			EditorGUILayout.LabelField ("", GUILayout.Width (80));
		}
	} // void ToggleHelp()

	//================================================================================

	void AddTouchpad() 
	{
		if (GUILayout.Button ("Add Touchpad")) 
		{
			virtualTouchpadScript.virtualTouchpads.Add (new VirtualTouchpad.Touchpad ());
			int last = virtualTouchpadScript.virtualTouchpads.Count - 1;
			virtualTouchpadScript.virtualTouchpads [last].name = "VirtualTouchpad";
			virtualTouchpadScript.virtualTouchpads [last].showSettingsInInspector = true;
			virtualTouchpadScript.virtualTouchpads [last].touchpadScreenRect = new Rect (0.4f, 0.4f, 0.2f, 0.2f);
			virtualTouchpadScript.virtualTouchpads [last].axisMultiplier = new Vector2 (1f, 1f);
			virtualTouchpadScript.virtualTouchpads [last].compensateForDeviceRoll = true;
			virtualTouchpadScript.virtualTouchpads [last].hideBackgroundTexture = false;
			virtualTouchpadScript.virtualTouchpads [last].autoUpdateEnabled = false;
			virtualTouchpadScript.virtualTouchpads [last].autoUpdateGyroAccelScript = false;
			virtualTouchpadScript.virtualTouchpads [last].autoUpdateAxisMessageReceivers = new List<VirtualTouchpad.SendMessageReceiver> ();
			virtualTouchpadScript.virtualTouchpads [last].autoUpdateDoubleTapMessageReceivers = new List<VirtualTouchpad.SendMessageReceiver> ();
			virtualTouchpadScript.virtualTouchpads [last].autoUpdateDoubleTapHoldMessageReceivers = new List<VirtualTouchpad.SendMessageReceiver> ();

		}
	} // void AddTouchpad() 

	//================================================================================

	void ShowHelp() 
	{
		if (showHelp && virtualTouchpadScript.virtualTouchpads.Count > 0) 
		{
			string helpText = 
				"Name - " +
					"The name of the touchpad. " +
					"Make sure you use the exact same name when refering to this touchpad " +
					"when using it elsewhere " +
					"(e.g. when using the MSP_Input.VirtualTouchpad.GetAxis() " +
					"command in other scripts).\n\n" +
				"Background Texture - " +
					"The texture to be used for the touchpad’s background. " +
					"Select 'hide' to hide the texture during gameplay.\n\n" +
				"!! NOTE: \n" +
					"All of the following settings are only visible after selecting a texture!\n\n" +
				"Horizontal / Vertical - " +
					"The horiontal and vertical size and offset on screen. " +
					"Shown values are in relative screen coordinates, " +
					"e.g. 0 is left/bottom and 1 is right/top.\n\n" +
				"Axis Multiplier - " +
					"How sensitive should the touchpad be? " +
					"Tip: if you want to invert the touchpad’s movement, use negative values.\n\n" +
				"Device Roll Compensation - " +
					"Should the output be compensated for the roll of your device. " +
					"e.g.: when turning your device like a steering wheel, " +
					"the input direction of the touchpad will follow accordingly.\n\n" +
				"Auto Update - " +
					"Integration with the GyroAccel script is a build in option, which can be (de)selected.\n" +
					"Other scripts can also be automatically informed of the touchpad's's axis values " +
					"and/or when the touchpad has been double-tapped. " +
					"This info is being send using a SendMessage:\n" +
					"- add the GameObject that should receive the message;\n" +
					"- pass the name of the function that should be called on this GameObject.\n" +
					"For sending the axis, this function must be void and excepting a Vector2 as input, " +
					"e.g.: 'void SomeFunctionName(Vector2 axis)'.\n" +
					"For sending the doubleTap or doubleTapHold status, this function must be void and excepting no input, " +
					"e.g.: 'void SomeOtherFunctionName()'.";
			EditorGUILayout.HelpBox(helpText,MessageType.None, true);
		}
	} // void ShowHelp()

	//================================================================================

} // public class VirtualTouchpadEditor : Editor 
