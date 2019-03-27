using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using MSP_Input;

[CustomEditor (typeof(VirtualButton))]
public class VirtualButtonEditor : Editor
{
	private static bool showHelp = false;
	private VirtualButton virtualButtonScript;

	//================================================================================

	public override void OnInspectorGUI() 
	{
		//DrawDefaultInspector();
		virtualButtonScript = target as VirtualButton;

		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.PrefixLabel (" ");
			AddButton ();
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
		foreach (VirtualButton.Button button in virtualButtonScript.virtualButtons) 
		{
			if (!button.isActive && !Application.isPlaying) 
			{
				button.center = button.centerDefault;
				button.centerNew = button.centerDefault;
				button.touchID = -1;
			}

			EditorGUILayout.BeginHorizontal();
			{
				button.showSettingsInInspector = EditorGUILayout.Foldout (button.showSettingsInInspector, "[" + (i+1) + "] " + button.name);
				button.enabled = EditorGUILayout.Toggle(" " ,button.enabled);	
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
			
			if (button.showSettingsInInspector) 
			{
				button.name = EditorGUILayout.TextField ("Name", button.name);
				EditorGUILayout.BeginHorizontal ();
				{
					EditorGUILayout.PrefixLabel ("Texture Unpressed");
					button.textureUnpressed = EditorGUILayout.ObjectField(button.textureUnpressed,typeof(Texture),false) as Texture;
					if (!Application.isPlaying) button.buttonTexture = button.textureUnpressed;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal ();
				{
					EditorGUILayout.PrefixLabel ("Texture Pressed");
					button.texturePressed = EditorGUILayout.ObjectField(button.texturePressed,typeof(Texture),false) as Texture;
				}
				EditorGUILayout.EndHorizontal();

				if (button.textureUnpressed && button.texturePressed) 
				{
					if (button.forceSquareButton) 
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel("Size");
							button.buttonSize.x = EditorGUILayout.Slider(button.buttonSize.x,0f,1f);
							button.buttonSize.y = button.buttonSize.x;
							button.forceSquareButton = EditorGUILayout.ToggleLeft("Square",button.forceSquareButton,GUILayout.Width(80));

						}
						EditorGUILayout.EndHorizontal();
					} else {
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel("Size Horizontal");
							button.buttonSize.x = EditorGUILayout.Slider(button.buttonSize.x,0f,1f);
							button.forceSquareButton = EditorGUILayout.ToggleLeft("Square",button.forceSquareButton,GUILayout.Width(80));
						}
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel("Size Vertical");
							button.buttonSize.y = EditorGUILayout.Slider(button.buttonSize.y,0f,1f);
							EditorGUILayout.LabelField (" ",GUILayout.Width(80));
						}
						EditorGUILayout.EndHorizontal();
					}

					EditorGUILayout.BeginHorizontal();
					{
						button.centerDefault = EditorGUILayout.Vector2Field("Center", button.centerDefault);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel("Center X");
						button.centerDefault.x = EditorGUILayout.Slider(button.centerDefault.x,0f,1f);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel("Center Y");
						button.centerDefault.y = EditorGUILayout.Slider(button.centerDefault.y,0f,1f);
					}
					EditorGUILayout.EndHorizontal();

					if (button.moveWithTouch) 
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel("Move With Touch: Smoothness");
							button.moveWithTouch = EditorGUILayout.ToggleLeft("",button.moveWithTouch,GUILayout.Width(12));
							button.smoothingTime = EditorGUILayout.Slider(button.smoothingTime,0f,1f);
						}
						EditorGUILayout.EndHorizontal();
					} else {
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel("Move With Touch");
							button.moveWithTouch = EditorGUILayout.ToggleLeft("",button.moveWithTouch,GUILayout.Width(12));
						}
						EditorGUILayout.EndHorizontal();
					}

					button.useAsSwitch = EditorGUILayout.Toggle("Use As Switch",button.useAsSwitch);

					button.editorSimulationKey = (KeyCode) EditorGUILayout.EnumPopup("Editor Simulation Key", button.editorSimulationKey);
					
					
					
					button.autoUpdateEnabled = EditorGUILayout.Toggle("Auto Update",button.autoUpdateEnabled);
					if (button.autoUpdateEnabled)
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel(" ");
							if (GUILayout.Button ("+")) 
							{
								button.autoUpdateButtonDownMessageReceivers.Add(new VirtualButton.SendMessageReceiver());
								int last = button.autoUpdateButtonDownMessageReceivers.Count-1;
								button.autoUpdateButtonDownMessageReceivers[last].messageReceiver = null;
								button.autoUpdateButtonDownMessageReceivers[last].functionName = "Function to call";
							}
							EditorGUILayout.LabelField ("SendMessage: Down");
						}
						EditorGUILayout.EndHorizontal();
						//
						int j = 0;
						bool removeReceiver = false;
						int receiverToRemove = -1;
						foreach (VirtualButton.SendMessageReceiver receiver in button.autoUpdateButtonDownMessageReceivers) 
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
							button.autoUpdateButtonDownMessageReceivers.RemoveAt(receiverToRemove);
						}
							
						EditorGUILayout.Space();
						
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel(" ");
							if (GUILayout.Button ("+")) 
							{
								button.autoUpdateButtonUpMessageReceivers.Add(new VirtualButton.SendMessageReceiver());
								int last = button.autoUpdateButtonUpMessageReceivers.Count-1;
								button.autoUpdateButtonUpMessageReceivers[last].messageReceiver = null;
								button.autoUpdateButtonUpMessageReceivers[last].functionName = "Function to call";
							}
							EditorGUILayout.LabelField ("SendMessage: Up");
						}
						EditorGUILayout.EndHorizontal();
						//
						j = 0;
						removeReceiver = false;
						receiverToRemove = -1;
						foreach (VirtualButton.SendMessageReceiver receiver in button.autoUpdateButtonUpMessageReceivers) 
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
							button.autoUpdateButtonUpMessageReceivers.RemoveAt(receiverToRemove);
						}	

						EditorGUILayout.Space();

						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel(" ");
							if (GUILayout.Button ("+")) 
							{
								button.autoUpdateButtonGoingDownMessageReceivers.Add(new VirtualButton.SendMessageReceiver());
								int last = button.autoUpdateButtonGoingDownMessageReceivers.Count-1;
								button.autoUpdateButtonGoingDownMessageReceivers[last].messageReceiver = null;
								button.autoUpdateButtonGoingDownMessageReceivers[last].functionName = "Function to call";
							}
							EditorGUILayout.LabelField ("SendMessage: Going Down");
						}
						EditorGUILayout.EndHorizontal();
						//
						j = 0;
						removeReceiver = false;
						receiverToRemove = -1;
						foreach (VirtualButton.SendMessageReceiver receiver in button.autoUpdateButtonGoingDownMessageReceivers) 
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
							button.autoUpdateButtonGoingDownMessageReceivers.RemoveAt(receiverToRemove);
						}
						
						EditorGUILayout.Space();
						
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel(" ");
							if (GUILayout.Button ("+")) 
							{
								button.autoUpdateButtonGoingUpMessageReceivers.Add(new VirtualButton.SendMessageReceiver());
								int last = button.autoUpdateButtonGoingUpMessageReceivers.Count-1;
								button.autoUpdateButtonGoingUpMessageReceivers[last].messageReceiver = null;
								button.autoUpdateButtonGoingUpMessageReceivers[last].functionName = "Function to call";
							}
							EditorGUILayout.LabelField ("SendMessage: Going Up");
						}
						EditorGUILayout.EndHorizontal();
						//
						j = 0;
						removeReceiver = false;
						receiverToRemove = -1;
						foreach (VirtualButton.SendMessageReceiver receiver in button.autoUpdateButtonGoingUpMessageReceivers) 
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
							button.autoUpdateButtonGoingUpMessageReceivers.RemoveAt(receiverToRemove);
						}	
					}
				}

				if (Application.isPlaying) 
				{
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel ("Debug Info");
						EditorGUILayout.LabelField ("Button Status = " + button.status);
					}
					EditorGUILayout.EndHorizontal();
				} // if (button.showDebugInfoInInspector)
			} // if (button.showSettingsInInspector)
			EditorGUILayout.Space();
			i++;
		}

		if (removeItem) 
		{
			virtualButtonScript.virtualButtons.RemoveAt(itemToRemove);
		}
		if (moveItem) 
		{
			int itemToSwap = Mathf.Clamp (itemToMove + itemMoveDirection, 0, i - 1);
			VirtualButton.Button tmpItem = virtualButtonScript.virtualButtons[itemToSwap];
			virtualButtonScript.virtualButtons[itemToSwap] =  virtualButtonScript.virtualButtons[itemToMove];
			virtualButtonScript.virtualButtons[itemToMove] =  tmpItem;
		}
		
		EditorGUILayout.Space ();

		if (GUI.changed) 
		{
			EditorUtility.SetDirty(virtualButtonScript);    
		}

		if (!Application.isPlaying) {
			foreach (VirtualButton.Button button in virtualButtonScript.virtualButtons) 
			{
				button.center = button.centerDefault;
				button.centerNew = button.centerDefault;
				button.touchID = -1;
			}
			virtualButtonScript.CalculateRects ();
		}
	
	} // public override void OnInspectorGUI() 

	//================================================================================

	void AddButton() 
	{
		if (GUILayout.Button ("Add button")) {
			virtualButtonScript.virtualButtons.Add (new VirtualButton.Button ());
			int last = virtualButtonScript.virtualButtons.Count - 1;
			virtualButtonScript.virtualButtons[last].name = "VirtualButton";
			virtualButtonScript.virtualButtons[last].showSettingsInInspector = true;
			virtualButtonScript.virtualButtons[last].buttonSize = new Vector2 (0.1f, 0.1f);
			virtualButtonScript.virtualButtons[last].centerDefault = new Vector2 (0.5f, 0.5f);
			virtualButtonScript.virtualButtons[last].smoothingTime = 0.1f;
			virtualButtonScript.virtualButtons[last].autoUpdateEnabled = false;
			virtualButtonScript.virtualButtons[last].autoUpdateButtonDownMessageReceivers = new List<VirtualButton.SendMessageReceiver> ();
			virtualButtonScript.virtualButtons[last].autoUpdateButtonGoingDownMessageReceivers = new List<VirtualButton.SendMessageReceiver> ();
			virtualButtonScript.virtualButtons[last].autoUpdateButtonGoingUpMessageReceivers = new List<VirtualButton.SendMessageReceiver> ();
			virtualButtonScript.virtualButtons[last].autoUpdateButtonUpMessageReceivers = new List<VirtualButton.SendMessageReceiver> ();
		}
	}

	//================================================================================

	void ToggleHelp()
	{
		if (virtualButtonScript.virtualButtons.Count > 0) 
		{
			showHelp = EditorGUILayout.ToggleLeft ("show help", showHelp, GUILayout.Width (80));
		} else {
			EditorGUILayout.LabelField ("", GUILayout.Width (80));
		}
	} // void ToggleHelp()

	//================================================================================

	void ShowHelp() 
	{
		if (showHelp && virtualButtonScript.virtualButtons.Count > 0) 
		{
			string helpText = 
				"Name - " +
					"The name of the button. " +
					"Make sure you use the exact same name when refering to this button " +
					"when using it elsewhere " +
					"(e.g. when using the MSP_Input.VirtualButton.GetButton(), etc. " +
					"commands in other scripts).\n\n" +
				"Texture Unpressed / Pressed - " +
					"The texture to use when the button is (un)pressed.\n\n" +
					"Horizontal / Vertical - " +
				"!! NOTE: \n" +
					"All of the following settings are only visible after selecting both textures!\n\n" +
				"Size - " +
					"The size of the button, in relative screen coordinates. " +
					"Select 'square' if you want to force a button with square dimensions.\n\n" +
				"Center / Center X / Center Y - " +
					"The center of the button, in relative screen coordinates. " +
					"The values can be entered directly, or by using the sliders.\n\n" +
				"Move With Touch - " +
					"Once pressed, should the button move along with the finger? " +
					"Use the slider to set the transition speed.\n\n" +
				"Use As Switch - " +
					"The button stays down/up after pressing it, like a switch.\n\n" +
				"Auto Update - " +
					"Button states (Down, U, Going Down, Going Up) can be automatically pushed " +
					"to scripts on other GameObjects, by sending them a message using a SendMessage:\n" +
					"- add the GameObject that should receive the message;\n" +
					"- pass the name of the function that should be called on this GameObject.\n" +
					"For sending the doubleTap status, this function must be void and excepting no input, " +
					"e.g.: 'void SomeOtherFunctionName()'.";
			EditorGUILayout.HelpBox(helpText,MessageType.None, true);
		}
	} // void ShowHelp()

	//================================================================================

	} // public class VirtualButtonEditor : Editor