using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using MSP_Input;

[CustomEditor (typeof(VirtualJoystick))]
public class VirtualJoystickEditor : Editor 
{
	private static bool showHelp = false;
	private VirtualJoystick virtualJoystickScript;

	//================================================================================

	public override void OnInspectorGUI() 
	{
		//DrawDefaultInspector();
		virtualJoystickScript = target as VirtualJoystick;

		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.PrefixLabel (" ");
			AddJoystick ();
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
		foreach (VirtualJoystick.Joystick joystick in virtualJoystickScript.virtualJoysticks) 
		{
			if (!joystick.isActive) 
			{
				joystick.centerBackground = joystick.centerDefault;
				joystick.centerBackgroundNew = joystick.centerDefault;
				joystick.centerButton = joystick.centerDefault;
				joystick.centerButtonNew = joystick.centerDefault;
				joystick.sensitivityCurve = new AnimationCurve(new Keyframe(-1f, 1f), new Keyframe(0f, joystick.sensitivity), new Keyframe(1f, 1f));
				joystick.touchID = -1;
			}

			EditorGUILayout.BeginHorizontal();
			{
				joystick.showSettingsInInspector = EditorGUILayout.Foldout (joystick.showSettingsInInspector, "[" + (i+1) + "] " + joystick.name);
				joystick.enabled = EditorGUILayout.Toggle(" " ,joystick.enabled);	
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

			if (joystick.showSettingsInInspector) 
			{
				joystick.name = EditorGUILayout.TextField ("Name", joystick.name);
				EditorGUILayout.BeginHorizontal ();
				{
					EditorGUILayout.PrefixLabel ("Background Texture");
					joystick.backgroundTexture = EditorGUILayout.ObjectField(joystick.backgroundTexture,typeof(Texture),false) as Texture;
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal ();
				{
					EditorGUILayout.PrefixLabel ("Button Texture");
					joystick.buttonTexture = EditorGUILayout.ObjectField(joystick.buttonTexture,typeof(Texture),false) as Texture;
				}
				EditorGUILayout.EndHorizontal();

				if (joystick.backgroundTexture || joystick.buttonTexture) 
				{
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel("Joystick size");
						joystick.backgroundSize = EditorGUILayout.Slider(joystick.backgroundSize,0f,1f);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel("Button size");
						joystick.buttonSize = EditorGUILayout.Slider(joystick.buttonSize,0f,1f);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					{
						joystick.centerDefault = EditorGUILayout.Vector2Field("Center", joystick.centerDefault);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel("Center X");
						joystick.centerDefault.x = EditorGUILayout.Slider(joystick.centerDefault.x,0f,1f);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel("Center Y");
						joystick.centerDefault.y = EditorGUILayout.Slider(joystick.centerDefault.y,0f,1f);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					{
						joystick.axisMultiplier = EditorGUILayout.Vector2Field("Axis Multiplier", joystick.axisMultiplier);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel("Sensitivity");
						joystick.sensitivity = EditorGUILayout.Slider(joystick.sensitivity,0f,1f);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel("Smoothing Time");
						joystick.smoothingTime = EditorGUILayout.Slider(joystick.smoothingTime,0f,1f);
					}
					EditorGUILayout.EndHorizontal();

					joystick.alwaysVisible = EditorGUILayout.Toggle("Always Visible",joystick.alwaysVisible);
					
					//
					//EditorSimulation
					//
					EditorGUILayout.LabelField("Editor Keyboard Simulation:");
					joystick.editorJoystickSimulation.simulationMode  = (VirtualJoystick.EditorJoystickSimulationMode) EditorGUILayout.EnumPopup("  * Joystick", joystick.editorJoystickSimulation.simulationMode);
					switch (joystick.editorJoystickSimulation.simulationMode)
					{
					case VirtualJoystick.EditorJoystickSimulationMode.None:
						//joystick.editorJoystickSimulation.keyCode_Up = KeyCode.None;
						//joystick.editorJoystickSimulation.keyCode_Left = KeyCode.None;
						//joystick.editorJoystickSimulation.keyCode_Down = KeyCode.None;
						//joystick.editorJoystickSimulation.keyCode_Right = KeyCode.None;
						break;
					case VirtualJoystick.EditorJoystickSimulationMode.Keyboard_WASD:
						joystick.editorJoystickSimulation.keyCode_Up = KeyCode.W;
						joystick.editorJoystickSimulation.keyCode_Left = KeyCode.A;
						joystick.editorJoystickSimulation.keyCode_Down = KeyCode.S;
						joystick.editorJoystickSimulation.keyCode_Right = KeyCode.D;
						break;
					case VirtualJoystick.EditorJoystickSimulationMode.Keyboard_CursorKeys:
						joystick.editorJoystickSimulation.keyCode_Up = KeyCode.UpArrow;
						joystick.editorJoystickSimulation.keyCode_Left = KeyCode.LeftArrow;
						joystick.editorJoystickSimulation.keyCode_Down = KeyCode.DownArrow;
						joystick.editorJoystickSimulation.keyCode_Right = KeyCode.RightArrow;
						break;
					case VirtualJoystick.EditorJoystickSimulationMode.Keyboard_Custom:
						joystick.editorJoystickSimulation.keyCode_Left = (KeyCode) EditorGUILayout.EnumPopup("      Left", joystick.editorJoystickSimulation.keyCode_Left);
						joystick.editorJoystickSimulation.keyCode_Right = (KeyCode) EditorGUILayout.EnumPopup("      Right", joystick.editorJoystickSimulation.keyCode_Right);
						joystick.editorJoystickSimulation.keyCode_Up = (KeyCode) EditorGUILayout.EnumPopup("      Up", joystick.editorJoystickSimulation.keyCode_Up);
						joystick.editorJoystickSimulation.keyCode_Down = (KeyCode) EditorGUILayout.EnumPopup("      Down", joystick.editorJoystickSimulation.keyCode_Down);
						break;
					default:
						break;			
					}
					joystick.editorJoystickSimulation.keyCode_DoubleTap = (KeyCode) EditorGUILayout.EnumPopup("  * DoubleTap", joystick.editorJoystickSimulation.keyCode_DoubleTap);
					//
					//AutoUpdate
					//
					
					joystick.autoUpdateEnabled = EditorGUILayout.Toggle("Auto Update",joystick.autoUpdateEnabled);
					if (joystick.autoUpdateEnabled) 
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel(" ");
							joystick.autoUpdateGyroAccelScript = EditorGUILayout.ToggleLeft(" GyroAccelScript",joystick.autoUpdateGyroAccelScript);
						}
						EditorGUILayout.EndHorizontal();
						
						// SendMessage: Axis
						
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel(" ");
							if (GUILayout.Button ("+")) 
							{
								joystick.autoUpdateAxisMessageReceivers.Add(new VirtualJoystick.SendMessageReceiver());
								int last = joystick.autoUpdateAxisMessageReceivers.Count-1;
								joystick.autoUpdateAxisMessageReceivers[last].messageReceiver = null;
								joystick.autoUpdateAxisMessageReceivers[last].functionName = "Function to call";
							}
							EditorGUILayout.LabelField ("SendMessage: Axis");
						}
						EditorGUILayout.EndHorizontal();
						
						int j = 0;
						bool removeReceiver = false;
						int receiverToRemove = -1;
						foreach (VirtualJoystick.SendMessageReceiver receiver in joystick.autoUpdateAxisMessageReceivers) 
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
							joystick.autoUpdateAxisMessageReceivers.RemoveAt(receiverToRemove);
						}
						
						// SendMessage: DoubleTap
						
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel(" ");
							if (GUILayout.Button ("+")) 
							{
								joystick.autoUpdateDoubleTapMessageReceivers.Add(new VirtualJoystick.SendMessageReceiver());
								int last = joystick.autoUpdateDoubleTapMessageReceivers.Count-1;
								joystick.autoUpdateDoubleTapMessageReceivers[last].messageReceiver = null;
								joystick.autoUpdateDoubleTapMessageReceivers[last].functionName = "Function to call";
							}
							EditorGUILayout.LabelField ("SendMessage: DoubleTap");
						}
						EditorGUILayout.EndHorizontal();
						
						j = 0;
						removeReceiver = false;
						receiverToRemove = -1;
						foreach (VirtualJoystick.SendMessageReceiver receiver in joystick.autoUpdateDoubleTapMessageReceivers) 
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
							joystick.autoUpdateDoubleTapMessageReceivers.RemoveAt(receiverToRemove);
						}

						// SendMessage: DoubleTapHold
						
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel(" ");
							if (GUILayout.Button ("+")) 
							{
								joystick.autoUpdateDoubleTapHoldMessageReceivers.Add(new VirtualJoystick.SendMessageReceiver());
								int last = joystick.autoUpdateDoubleTapHoldMessageReceivers.Count-1;
								joystick.autoUpdateDoubleTapHoldMessageReceivers[last].messageReceiver = null;
								joystick.autoUpdateDoubleTapHoldMessageReceivers[last].functionName = "Function to call";
							}
							EditorGUILayout.LabelField ("SendMessage: DoubleTapHold");
						}
						EditorGUILayout.EndHorizontal();
						
						j = 0;
						removeReceiver = false;
						receiverToRemove = -1;
						foreach (VirtualJoystick.SendMessageReceiver receiver in joystick.autoUpdateDoubleTapHoldMessageReceivers) 
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
							joystick.autoUpdateDoubleTapHoldMessageReceivers.RemoveAt(receiverToRemove);
						}

					}
				}
				//
				if (Application.isPlaying) 
				{
					EditorGUILayout.BeginHorizontal ();
					{
						EditorGUILayout.PrefixLabel ("Debug Info");
						EditorGUILayout.LabelField ("axis = " + joystick.axis);
					}
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.BeginHorizontal ();
					{
						EditorGUILayout.PrefixLabel (" ");
						EditorGUILayout.LabelField ("angle = " + joystick.angle);
					}
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.BeginHorizontal ();
					{
						EditorGUILayout.PrefixLabel (" ");
						EditorGUILayout.LabelField ("magnitude = " + joystick.magnitude);
					}
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.BeginHorizontal ();
					{
						EditorGUILayout.PrefixLabel (" ");
						EditorGUILayout.LabelField ("doubleTap = " + joystick.doubleTap);
					}
					EditorGUILayout.EndHorizontal ();				
					EditorGUILayout.BeginHorizontal ();
					{
						EditorGUILayout.PrefixLabel (" ");
						EditorGUILayout.LabelField ("doubleTapHold = " + joystick.doubleTapHold);
					}
					EditorGUILayout.EndHorizontal ();				
				} // if (joystick.showDebugInfoInInspector)
			} // if (joystick.showSettingsInInspector)

			EditorGUILayout.Space();
			i++;
		} // foreach (VirtualJoystick.Joystick joystick in virtualJoystickScript.virtualJoysticks)

		if (removeItem) 
		{
			virtualJoystickScript.virtualJoysticks.RemoveAt(itemToRemove);
		}
		if (moveItem) 
		{
			int itemToSwap = Mathf.Clamp (itemToMove + itemMoveDirection, 0, i - 1);
			VirtualJoystick.Joystick tmpItem = virtualJoystickScript.virtualJoysticks[itemToSwap];
			virtualJoystickScript.virtualJoysticks[itemToSwap] =  virtualJoystickScript.virtualJoysticks[itemToMove];
			virtualJoystickScript.virtualJoysticks[itemToMove] =  tmpItem;
		}

		EditorGUILayout.Space ();

		if (GUI.changed) 
		{
			EditorUtility.SetDirty(virtualJoystickScript);    
		}

		if (!Application.isPlaying) {
			foreach (VirtualJoystick.Joystick joystick in virtualJoystickScript.virtualJoysticks) 
			{
				float sw = (float)Screen.width;
				float sh = (float)Screen.height;
				joystick.backgroundRect.width = (int)(joystick.backgroundSize * sw);
				joystick.backgroundRect.height = joystick.backgroundRect.width; 
				joystick.backgroundRect.center = new Vector2 ((int)(sw * joystick.centerBackground.x), (int)(sh * (1f - joystick.centerBackground.y)));

				joystick.buttonRect.width = (int)(joystick.backgroundSize * joystick.buttonSize * sw);
				joystick.buttonRect.height = joystick.buttonRect.width; 
				joystick.buttonRect.center = new Vector2 ((int)(sw * joystick.centerButton.x), (int)(sh * (1f - joystick.centerButton.y)));

				joystick.centerBackground = joystick.centerDefault;
				joystick.centerBackgroundNew = joystick.centerDefault;
				joystick.centerButton = joystick.centerDefault;
				joystick.centerButtonNew = joystick.centerDefault;
				joystick.sensitivityCurve = new AnimationCurve(new Keyframe(-1f, 1f), new Keyframe(0f, joystick.sensitivity), new Keyframe(1f, 1f));
				joystick.touchID = -1;
			}
		}

	} // public override void OnInspectorGUI() 

	//================================================================================
	
	void ToggleHelp()
	{
		if (virtualJoystickScript.virtualJoysticks.Count > 0) 
		{
			showHelp = EditorGUILayout.ToggleLeft ("show help", showHelp, GUILayout.Width (80));
		} else {
			EditorGUILayout.LabelField ("", GUILayout.Width (80));
		}
	} // void ToggleHelp()

	//================================================================================

	void AddJoystick()
	{
		if (GUILayout.Button ("Add Joystick")) 
		{
			virtualJoystickScript.virtualJoysticks.Add (new VirtualJoystick.Joystick ());
			int last = virtualJoystickScript.virtualJoysticks.Count-1;
			virtualJoystickScript.virtualJoysticks[last].name = "VirtualJoystick";
			virtualJoystickScript.virtualJoysticks[last].showSettingsInInspector = true;
			virtualJoystickScript.virtualJoysticks[last].backgroundSize = 0.2f;
			virtualJoystickScript.virtualJoysticks[last].buttonSize = 0.25f;
			virtualJoystickScript.virtualJoysticks[last].centerDefault = new Vector2(0.5f,0.5f);
			virtualJoystickScript.virtualJoysticks[last].axisMultiplier = new Vector2(1.0f,1.0f);
			virtualJoystickScript.virtualJoysticks[last].sensitivity = 0.5f;
			virtualJoystickScript.virtualJoysticks[last].smoothingTime = 0.1f;
			virtualJoystickScript.virtualJoysticks[last].alwaysVisible = true;
			virtualJoystickScript.virtualJoysticks[last].autoUpdateEnabled = false;
			virtualJoystickScript.virtualJoysticks[last].autoUpdateGyroAccelScript = false;
			virtualJoystickScript.virtualJoysticks[last].autoUpdateAxisMessageReceivers = new List<VirtualJoystick.SendMessageReceiver> ();
			virtualJoystickScript.virtualJoysticks[last].autoUpdateDoubleTapMessageReceivers = new List<VirtualJoystick.SendMessageReceiver> ();
			virtualJoystickScript.virtualJoysticks[last].autoUpdateDoubleTapHoldMessageReceivers = new List<VirtualJoystick.SendMessageReceiver> ();
		}
	}

	//================================================================================
	
	void ShowHelp() 
	{
		if (showHelp && virtualJoystickScript.virtualJoysticks.Count > 0) 
		{
			string helpText = 
				"Name - " +
					"The name of the joystick. " +
					"Make sure you use the exact same name when refering to this joystick " +
					"when using it elsewhere " +
					"(e.g. when using the MSP_Input.VirtualJoystick.GetAxis() " +
					"command in other scripts).\n\n" +
				"Background Texture / Button Texture - " +
					"The texture to be used for the joystick’s background and button. " +
				"!! NOTE: \n" +
					"All of the following settings are only visible after selecting either a texture" +
					"for the background or button!\n\n" +
				"Joystick Size - " +
					"The size of the joystick’s background, in relative screen coordinates. " +
					"The joystick button cannot be pulled outside it’s background.\n\n" +
				"Button Size - " +
					"The size of the joystick’s button, relative to the joystick's background.\n\n" +
				"Center / Center X / Center Y - " +
					"The center of the button, in relative screen coordinates. " +
					"The values can be entered directly, or by using the sliders.\n\n" +
				"Axis Multiplier - " +
					"By default, the joystick returns a Vector2 with values between -1 and 1. " +
					"These values can be multiplied with an axisMultiplier. " +
					"Tip: if you want to invert the joystick movement, " +
					"use negative values for the axisMultiplier." +
				"Sensitivity - " +
					"Smaller values will make the joystick less sensitive, " +
					"while the button is near the middle.\n\n" +
				"Smoothing Time - " +
					"The (maximum) time in which the joystick moves towards it's target position\n\n" +
				"Always Visible - " +
					"Should the joysick be visible all the time, or only when it is being pressed?\n\n" +
				"Auto Update - " +
					"Integration with the GyroAccel script is a build in option, which can be (de)selected.\n" +
					"Other scripts can also be automatically informed of the joystick's axis values " +
					"and/or when the joystick has been double-tapped. " +
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
	} // public class VirtualJoystickEditor : Editor 