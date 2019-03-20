using UnityEngine;
using UnityEditor;
using MSP_Input;

[CustomEditor (typeof(GyroAccel))]
public class GyroAccelEditor : Editor 
{
	private static bool showHelpSettingsMenu = false;
	private static bool showHelpAutoUpdate = false;
	private static bool settingsMenuEnabled = false;
	private static bool autoUpdateMenuEnabled = false;
	private static bool editorSimulationMenuEnabled = false;
	
	private GyroAccel gyroAccelScript;
	
	//================================================================================
	
	public override void OnInspectorGUI() 
	{
		gyroAccelScript = target as GyroAccel;
		
		//DrawDefaultInspector();
		SettingsMenu();
		AutoUpdateMenu();
		EditorSimulation();
		RuntimeDebugInfo();
		
		if (GUI.changed) 
		{
			gyroAccelScript.headingOffset = Mathf.RoundToInt(gyroAccelScript.headingOffset);
			gyroAccelScript.pitchOffsetMinimum = Mathf.RoundToInt(gyroAccelScript.pitchOffsetMinimum);
			gyroAccelScript.pitchOffsetMinimum = Mathf.Clamp(gyroAccelScript.pitchOffsetMinimum,-89f,0f);
			gyroAccelScript.pitchOffsetMaximum = Mathf.RoundToInt(gyroAccelScript.pitchOffsetMaximum);
			gyroAccelScript.pitchOffsetMaximum = Mathf.Clamp(gyroAccelScript.pitchOffsetMaximum,0f,89f);
			gyroAccelScript.pitchOffset = Mathf.RoundToInt(gyroAccelScript.pitchOffset);
			gyroAccelScript.pitchOffset = Mathf.Clamp(gyroAccelScript.pitchOffset,gyroAccelScript.pitchOffsetMinimum,gyroAccelScript.pitchOffsetMaximum);	
			EditorUtility.SetDirty(gyroAccelScript);    
		}
		
	} // public override void OnInspectorGUI() 
	
	//================================================================================
	
	void SettingsMenu() 
	{
		EditorGUILayout.BeginHorizontal();
		{
			settingsMenuEnabled = EditorGUILayout.Foldout (settingsMenuEnabled, "Settings");
			if (settingsMenuEnabled)
			{
				showHelpSettingsMenu = EditorGUILayout.ToggleLeft ("show help", showHelpSettingsMenu);
			}
		}
		EditorGUILayout.EndHorizontal();

		if (settingsMenuEnabled) 
		{
			if (showHelpSettingsMenu) 
			{
				string helpText = 
					"Force Accelerometer - " +
						"By default the gyroscope is used. " +
						"If no gyroscope is available, the accelerometer is used. " +
						"Set this variable if you want to force the use of the accelerometer, " +
						"even if there is a gyroscope available.\n\n" +
						"headingOffset - " +
						"The heading offset to be used " +
						"(North = 0, 90 = east, 180 = south, 270 = west, etc.) " +
						"Please note that this value can be changed during runtime, " +
						"either by yourself or internally by the script, " +
						"e.g. when the headingAmplifier is being used.\n\n" +
						"pitchOffset - " +
						"The pitch offset to be used " +
						"(straight up = 90, level = 0, straight down = -90). " +
						"Please note that this value can be changed during runtime, " +
						"either by yourself or internally by the script, " +
						"e.g. when the pitchAmplifier is being used.\n\n" +
						"pitchOffset Min/Max - " +
						"The minimum/maximum value of the pitchOffset. " +
						"For playability issues, don’t use values near 90 degrees.\n\n" +
						"gyroHeadingAmplifier / gyroPitchAmplifier - " +
						"The heading and pitch multipliers for the gyro. " +
						"Setting these values >1 will effectively speed up the rotation of your character in the gameworld. " +
						"Using values < 1 will slow it down.\n\n" +
						"Smoothing Time - " +
						"the smoothing time to be used. " +
						"Increase this value when your hands are shaking too much :-)";
				EditorGUILayout.HelpBox(helpText,MessageType.None, true);
			}
			//
			// Force accelerometer
			//
			gyroAccelScript.forceAccelerometer = EditorGUILayout.Toggle ("Force Accelerometer", gyroAccelScript.forceAccelerometer);
			
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel ("headingOffset");
				gyroAccelScript.headingOffset = EditorGUILayout.Slider (gyroAccelScript.headingOffset, -180, 180f);
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel ("pitchOffset");
				gyroAccelScript.pitchOffset = EditorGUILayout.Slider (gyroAccelScript.pitchOffset, -90f, 90f);
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel ("pitchOffset Min/Max");
				EditorGUILayout.MinMaxSlider (ref gyroAccelScript.pitchOffsetMinimum, ref gyroAccelScript.pitchOffsetMaximum, -90f, 90f);
				EditorGUILayout.LabelField ("" + gyroAccelScript.pitchOffsetMinimum + "/" + gyroAccelScript.pitchOffsetMaximum, GUILayout.Width (50));
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
			
			//
			// Set amplifiers for heading and pitch
			//
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel ("Gyro Heading Amplifier");
				gyroAccelScript.gyroHeadingAmplifier = EditorGUILayout.Slider (gyroAccelScript.gyroHeadingAmplifier, 0.1f, 4f);
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel ("Gyro Pitch Amplifier");
				gyroAccelScript.gyroPitchAmplifier = EditorGUILayout.Slider (gyroAccelScript.gyroPitchAmplifier, 0.1f, 4f);
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
			
			//
			// Set smoothing time
			//
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel ("Smoothing Time");
				gyroAccelScript.smoothingTime = EditorGUILayout.Slider (gyroAccelScript.smoothingTime, 0f, 1f);
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel (" ");
				if (GUILayout.Button ("Set default values")) 
				{
					gyroAccelScript.forceAccelerometer = false;
					gyroAccelScript.headingOffset = 0f;
					gyroAccelScript.pitchOffsetMinimum = -70f;
					gyroAccelScript.pitchOffsetMaximum = 70f;
					gyroAccelScript.pitchOffset = 30f;
					gyroAccelScript.gyroHeadingAmplifier = 1f;
					gyroAccelScript.gyroPitchAmplifier = 1f;
					gyroAccelScript.smoothingTime = 0.1f;
				}
			}
			EditorGUILayout.EndHorizontal();
		} 
	} // void SettingsMenu()
	
	//================================================================================
	
	void AutoUpdateMenu()
	{
		EditorGUILayout.BeginHorizontal();
		{
			autoUpdateMenuEnabled = EditorGUILayout.Foldout(autoUpdateMenuEnabled,"Auto Update");
			if (autoUpdateMenuEnabled)
			{
				showHelpAutoUpdate = EditorGUILayout.ToggleLeft ("show help", showHelpAutoUpdate);
			}
		}
		EditorGUILayout.EndHorizontal();

		if (autoUpdateMenuEnabled) 
		{
			if (showHelpAutoUpdate) 
			{
				string helpText = 
					"The rotation of GameObjects can be automatically updated." +
						"This update can be full or partially (e.g. only the heading, pitch and/or roll will be updated).\n" +
						"Also an (additional) smoothing time can be set for each GameObject separately.\n" +
						"'Self' - Update the GameObject which has this script attached to it\n" +
						"'Other' - Update other GameObjects in the scene. You can add as many as you want.\n" +
						"Note: Alle updates will be performed in the order of this list. E.g.: first 'self' will be updated, " +
						"then 'other1', 'other2', 'other3', etc. Use the sorting buttons to customize the order in which the rotations are applied. " +
						"(Usually, the main camera will come last).";
				EditorGUILayout.HelpBox(helpText,MessageType.None, true);
			}
			//
			// * Update Self
			//
			EditorGUILayout.BeginHorizontal();
			{	
				EditorGUILayout.PrefixLabel(" - Self:");
				gyroAccelScript.selfUpdate.enabled = EditorGUILayout.ToggleLeft("Enabled",gyroAccelScript.selfUpdate.enabled);
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			{
				gyroAccelScript.selfUpdate.copyHeading = EditorGUILayout.Toggle("     Heading",gyroAccelScript.selfUpdate.copyHeading);
				if (gyroAccelScript.selfUpdate.copyHeading) 
				{
					EditorGUILayout.MinMaxSlider (ref gyroAccelScript.selfUpdate.headingMin, ref gyroAccelScript.selfUpdate.headingMax, -180f, 180f);
					gyroAccelScript.selfUpdate.headingMin = Mathf.RoundToInt(gyroAccelScript.selfUpdate.headingMin);
					gyroAccelScript.selfUpdate.headingMin = Mathf.Clamp(gyroAccelScript.selfUpdate.headingMin,-180f,gyroAccelScript.selfUpdate.headingMax);
					gyroAccelScript.selfUpdate.headingMax = Mathf.RoundToInt(gyroAccelScript.selfUpdate.headingMax);
					gyroAccelScript.selfUpdate.headingMax = Mathf.Clamp(gyroAccelScript.selfUpdate.headingMax,gyroAccelScript.selfUpdate.headingMin,180f);
					EditorGUILayout.LabelField ("" + gyroAccelScript.selfUpdate.headingMin + "/" + gyroAccelScript.selfUpdate.headingMax, GUILayout.Width (70));
				} else {
					gyroAccelScript.selfUpdate.headingDefault = EditorGUILayout.Slider (gyroAccelScript.selfUpdate.headingDefault, -180f, 180f);
					gyroAccelScript.selfUpdate.headingDefault = Mathf.RoundToInt(gyroAccelScript.selfUpdate.headingDefault);
					gyroAccelScript.selfUpdate.headingDefault = Mathf.Clamp(gyroAccelScript.selfUpdate.headingDefault,-180f,180f);	
				}
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			{
				gyroAccelScript.selfUpdate.copyPitch = EditorGUILayout.Toggle("     Pitch",gyroAccelScript.selfUpdate.copyPitch);
				if (gyroAccelScript.selfUpdate.copyPitch) 
				{				
					EditorGUILayout.MinMaxSlider (ref gyroAccelScript.selfUpdate.pitchMin, ref gyroAccelScript.selfUpdate.pitchMax, -90f, 90f);
					gyroAccelScript.selfUpdate.pitchMin = Mathf.RoundToInt(gyroAccelScript.selfUpdate.pitchMin);
					gyroAccelScript.selfUpdate.pitchMin = Mathf.Clamp(gyroAccelScript.selfUpdate.pitchMin,-90f,gyroAccelScript.selfUpdate.pitchMax);
					gyroAccelScript.selfUpdate.pitchMax = Mathf.RoundToInt(gyroAccelScript.selfUpdate.pitchMax);
					gyroAccelScript.selfUpdate.pitchMax = Mathf.Clamp(gyroAccelScript.selfUpdate.pitchMax,gyroAccelScript.selfUpdate.pitchMin,90f);
					EditorGUILayout.LabelField ("" + gyroAccelScript.selfUpdate.pitchMin + "/" + gyroAccelScript.selfUpdate.pitchMax, GUILayout.Width (70));
				} else {
					gyroAccelScript.selfUpdate.pitchDefault = EditorGUILayout.Slider (gyroAccelScript.selfUpdate.pitchDefault, -90f, 90f);
					gyroAccelScript.selfUpdate.pitchDefault = Mathf.RoundToInt(gyroAccelScript.selfUpdate.pitchDefault);
					gyroAccelScript.selfUpdate.pitchDefault = Mathf.Clamp(gyroAccelScript.selfUpdate.pitchDefault,-90f,90f);	
				}
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			{
				gyroAccelScript.selfUpdate.copyRoll = EditorGUILayout.Toggle("     Roll",gyroAccelScript.selfUpdate.copyRoll);
				if (gyroAccelScript.selfUpdate.copyRoll) 
				{			
					EditorGUILayout.MinMaxSlider (ref gyroAccelScript.selfUpdate.rollMin, ref gyroAccelScript.selfUpdate.rollMax, -180f, 180f);
					gyroAccelScript.selfUpdate.rollMin = Mathf.RoundToInt(gyroAccelScript.selfUpdate.rollMin);
					gyroAccelScript.selfUpdate.rollMin = Mathf.Clamp(gyroAccelScript.selfUpdate.rollMin,-180f,gyroAccelScript.selfUpdate.rollMax);
					gyroAccelScript.selfUpdate.rollMax = Mathf.RoundToInt(gyroAccelScript.selfUpdate.rollMax);
					gyroAccelScript.selfUpdate.rollMax = Mathf.Clamp(gyroAccelScript.selfUpdate.rollMax,gyroAccelScript.selfUpdate.rollMin,180f);
					EditorGUILayout.LabelField ("" + gyroAccelScript.selfUpdate.rollMin + "/" + gyroAccelScript.selfUpdate.rollMax, GUILayout.Width (70));
				} else {
					gyroAccelScript.selfUpdate.rollDefault = EditorGUILayout.Slider (gyroAccelScript.selfUpdate.rollDefault, -180f, 180f);
					gyroAccelScript.selfUpdate.rollDefault = Mathf.RoundToInt(gyroAccelScript.selfUpdate.rollDefault);
					gyroAccelScript.selfUpdate.rollDefault = Mathf.Clamp(gyroAccelScript.selfUpdate.rollDefault,-180f,180f);
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				if (gyroAccelScript.selfUpdate.copyHeading && !(gyroAccelScript.selfUpdate.headingMin == -180f && gyroAccelScript.selfUpdate.headingMax == 180f && gyroAccelScript.selfUpdate.pitchMin == -90f && gyroAccelScript.selfUpdate.pitchMax == 90f))
				{
					EditorGUILayout.PrefixLabel(" ");
					gyroAccelScript.selfUpdate.pushEdge = EditorGUILayout.ToggleLeft("Push edges ", gyroAccelScript.selfUpdate.pushEdge, GUILayout.Width (70));			
					if (gyroAccelScript.selfUpdate.pushEdge)
					{
						string pushEdgeWarningText = "Warning: when a rotation reaches an edge," +
							" some offset variables will be automatically recalculated." +
							" This might also influence the rotation of other gameobjects controlled by this script!";
						EditorGUILayout.HelpBox(pushEdgeWarningText,MessageType.None, true);
					}
				}
			}			
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel ("     Smoothing Time");
				gyroAccelScript.selfUpdate.smoothingTime = EditorGUILayout.Slider (gyroAccelScript.selfUpdate.smoothingTime, 0f, 1f);
			}			
			EditorGUILayout.EndHorizontal();
			//
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel (" ");
				if (GUILayout.Button ("Set default values", GUILayout.Width (140))) 
				{
					gyroAccelScript.selfUpdate.copyHeading = true;
					gyroAccelScript.selfUpdate.copyPitch = true;
					gyroAccelScript.selfUpdate.copyRoll = true;
					gyroAccelScript.selfUpdate.enabled = true;
					gyroAccelScript.selfUpdate.headingMin = -180f;
					gyroAccelScript.selfUpdate.headingMax =  180f;
					gyroAccelScript.selfUpdate.headingDefault =  0f;
					gyroAccelScript.selfUpdate.pitchMin = -90f;
					gyroAccelScript.selfUpdate.pitchMax =  90f;
					gyroAccelScript.selfUpdate.pitchDefault =  0f;
					gyroAccelScript.selfUpdate.rollMin = -180f;
					gyroAccelScript.selfUpdate.rollMax =  180f;
					gyroAccelScript.selfUpdate.rollDefault =  0f;
					gyroAccelScript.selfUpdate.smoothingTime = 0.0f;
					gyroAccelScript.selfUpdate.pushEdge = false;
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			//
			// * Update Other 
			//
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(" - Other:");
			{
				if (GUILayout.Button("Add Transform", GUILayout.Width (140)))
				{
					gyroAccelScript.autoUpdateList.Add(new GyroAccel.AutoUpdateList());
				}
			}
			EditorGUILayout.EndHorizontal();
			//
			//
			// moving and sorting of list items
			int i = 0;
			bool removeItem = false;
			int itemToRemove = -1;
			bool moveItem = false;
			int itemToMove = -1;
			int itemMoveDirection = 0;
			foreach (GyroAccel.AutoUpdateList autoUpdateList in gyroAccelScript.autoUpdateList) 
			{
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				//
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PrefixLabel(" ");
					autoUpdateList.targetTransform = EditorGUILayout.ObjectField(autoUpdateList.targetTransform,typeof(Transform),true) as Transform;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PrefixLabel(" ");
					autoUpdateList.enabled = EditorGUILayout.ToggleLeft("Enabled",autoUpdateList.enabled, GUILayout.Width (70));
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					autoUpdateList.copyHeading = EditorGUILayout.Toggle("     Heading",autoUpdateList.copyHeading);
					if (autoUpdateList.copyHeading)
					{
						EditorGUILayout.MinMaxSlider (ref autoUpdateList.headingMin, ref autoUpdateList.headingMax, -180f, 180f);
						autoUpdateList.headingMin = Mathf.RoundToInt(autoUpdateList.headingMin);
						autoUpdateList.headingMin = Mathf.Clamp(autoUpdateList.headingMin,-180f,autoUpdateList.headingMax);
						autoUpdateList.headingMax = Mathf.RoundToInt(autoUpdateList.headingMax);
						autoUpdateList.headingMax = Mathf.Clamp(autoUpdateList.headingMax,autoUpdateList.headingMin,180f);
						EditorGUILayout.LabelField ("" + autoUpdateList.headingMin + "/" + autoUpdateList.headingMax, GUILayout.Width (70));
					} else {
						autoUpdateList.headingDefault = EditorGUILayout.Slider (autoUpdateList.headingDefault, -180f, 180f);
						autoUpdateList.headingDefault = Mathf.RoundToInt(autoUpdateList.headingDefault);
						autoUpdateList.headingDefault = Mathf.Clamp(autoUpdateList.headingDefault,-180f,180f);
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					autoUpdateList.copyPitch = EditorGUILayout.Toggle("     Pitch",autoUpdateList.copyPitch);
					if (autoUpdateList.copyPitch)
					{
						EditorGUILayout.MinMaxSlider (ref autoUpdateList.pitchMin, ref autoUpdateList.pitchMax, -90f, 90f);
						autoUpdateList.pitchMin = Mathf.RoundToInt(autoUpdateList.pitchMin);
						autoUpdateList.pitchMin = Mathf.Clamp(autoUpdateList.pitchMin,-90f,autoUpdateList.pitchMax);
						autoUpdateList.pitchMax = Mathf.RoundToInt(autoUpdateList.pitchMax);
						autoUpdateList.pitchMax = Mathf.Clamp(autoUpdateList.pitchMax,autoUpdateList.pitchMin,90f);
						EditorGUILayout.LabelField ("" + autoUpdateList.pitchMin + "/" + autoUpdateList.pitchMax, GUILayout.Width (70));
					} else {
						autoUpdateList.pitchDefault = EditorGUILayout.Slider (autoUpdateList.pitchDefault, -90f, 90f);
						autoUpdateList.pitchDefault = Mathf.RoundToInt(autoUpdateList.pitchDefault);
						autoUpdateList.pitchDefault = Mathf.Clamp(autoUpdateList.pitchDefault,-90f,90f);
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					autoUpdateList.copyRoll = EditorGUILayout.Toggle("     Roll",autoUpdateList.copyRoll);
					if (autoUpdateList.copyRoll)
					{
						EditorGUILayout.MinMaxSlider (ref autoUpdateList.rollMin, ref autoUpdateList.rollMax, -180f, 180f);
						EditorGUILayout.LabelField ("" + autoUpdateList.rollMin + "/" + autoUpdateList.rollMax, GUILayout.Width (70));
						autoUpdateList.rollMin = Mathf.RoundToInt(autoUpdateList.rollMin);
						autoUpdateList.rollMin = Mathf.Clamp(autoUpdateList.rollMin,-180f,autoUpdateList.rollMax);
						autoUpdateList.rollMax = Mathf.RoundToInt(autoUpdateList.rollMax);
						autoUpdateList.rollMax = Mathf.Clamp(autoUpdateList.rollMax,autoUpdateList.rollMin,180f);
					} else {
						autoUpdateList.rollDefault = EditorGUILayout.Slider (autoUpdateList.rollDefault, -180f, 180f);
						autoUpdateList.rollDefault = Mathf.RoundToInt(autoUpdateList.rollDefault);
						autoUpdateList.rollDefault = Mathf.Clamp(autoUpdateList.rollDefault,-180f,180f);
					}
				}
				EditorGUILayout.EndHorizontal();



				EditorGUILayout.BeginHorizontal();
				{
					if (autoUpdateList.copyHeading && !(autoUpdateList.headingMin == -180f && autoUpdateList.headingMax == 180f && autoUpdateList.pitchMin == -90f && autoUpdateList.pitchMax == 90f))
					{
						EditorGUILayout.PrefixLabel(" ");
						autoUpdateList.pushEdge = EditorGUILayout.ToggleLeft("Push edges ", autoUpdateList.pushEdge, GUILayout.Width (70));			
						if (autoUpdateList.pushEdge)
						{
							string pushEdgeWarningText = "Warning: when a rotation reaches an edge," +
								" some offset variables will be automatically recalculated." +
									" This might also influence the rotation of other gameobjects controlled by this script!";
							EditorGUILayout.HelpBox(pushEdgeWarningText,MessageType.None, true);
						}
					}
				}			
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PrefixLabel ("     Smoothing Time");
					autoUpdateList.smoothingTime = EditorGUILayout.Slider (autoUpdateList.smoothingTime, 0f, 1f);
				}			
				EditorGUILayout.EndHorizontal();
				//
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PrefixLabel (" ");
					if (GUILayout.Button ("Set default values", GUILayout.Width (140))) 
					{
						autoUpdateList.copyHeading = true;
						autoUpdateList.copyPitch = true;
						autoUpdateList.copyRoll = true;
						autoUpdateList.enabled = true;
						autoUpdateList.headingMin = -180f;
						autoUpdateList.headingMax =  180f;
						autoUpdateList.headingDefault =  0f;
						autoUpdateList.pitchMin = -90f;
						autoUpdateList.pitchMax =  90f;
						autoUpdateList.pitchDefault =  0f;
						autoUpdateList.rollMin = -180f;
						autoUpdateList.rollMax =  180f;
						autoUpdateList.rollDefault =  0f;
						autoUpdateList.smoothingTime = 0.0f;
						autoUpdateList.pushEdge = false;
					}
				}
				EditorGUILayout.EndHorizontal();
				//
				EditorGUILayout.BeginHorizontal();
				{

					EditorGUILayout.PrefixLabel(" ");
					if (GUILayout.Button("<", GUILayout.Width (32)))
					{
						moveItem = true;
						itemToMove = i;
						itemMoveDirection = -1;
					}
					if (GUILayout.Button(">", GUILayout.Width (32)))
					{
						moveItem = true;
						itemToMove = i;
						itemMoveDirection = 1;
					}
					EditorGUILayout.LabelField (" ", GUILayout.Width (32));
					if (GUILayout.Button("-", GUILayout.Width (32)))
					{
						removeItem = true;
						itemToRemove = i;
					}
				}
				EditorGUILayout.EndHorizontal();
				i++;
			}
			
			if (removeItem) 
			{
				gyroAccelScript.autoUpdateList.RemoveAt(itemToRemove);
			}
			
			if (moveItem) 
			{
				int itemToSwap = Mathf.Clamp(itemToMove+itemMoveDirection,0,i-1);
				GyroAccel.AutoUpdateList tmpItem = gyroAccelScript.autoUpdateList[itemToSwap];
				gyroAccelScript.autoUpdateList[itemToSwap] =  gyroAccelScript.autoUpdateList[itemToMove];
				gyroAccelScript.autoUpdateList[itemToMove] =  tmpItem;
			}
		} 
	} // void AutoUpdateMenu()
	
	//================================================================================
	
	void EditorSimulation()
	{
		EditorGUILayout.BeginHorizontal();
		{
			editorSimulationMenuEnabled = EditorGUILayout.Foldout(editorSimulationMenuEnabled,"Editor Simulation");
		}
		EditorGUILayout.EndHorizontal();
		
		if (editorSimulationMenuEnabled)
		{
			gyroAccelScript.editorSimulation.simulationMode  = (GyroAccel.SimulationMode) EditorGUILayout.EnumPopup("Simulation Mode", gyroAccelScript.editorSimulation.simulationMode);
			
			switch (gyroAccelScript.editorSimulation.simulationMode )
			{
			case GyroAccel.SimulationMode.Mouse:
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PrefixLabel ("  MouseX Sensitivity");
					gyroAccelScript.editorSimulation.mouseSenisitivity.x = EditorGUILayout.Slider (gyroAccelScript.editorSimulation.mouseSenisitivity.x, 0f, 400f);
					EditorGUILayout.LabelField("", GUILayout.Width(40));
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal ();
				{
					EditorGUILayout.PrefixLabel ("  MouseY Sensitivity");
					gyroAccelScript.editorSimulation.mouseSenisitivity.y = EditorGUILayout.Slider (gyroAccelScript.editorSimulation.mouseSenisitivity.y, 0f, 400f);
					gyroAccelScript.editorSimulation.invertMouseY = EditorGUILayout.ToggleLeft("inv",gyroAccelScript.editorSimulation.invertMouseY, GUILayout.Width(40));
				}
				EditorGUILayout.EndHorizontal();
				
				break;
			default:
				break;			
			}
		}
	} // EditorSimulation
	
	//================================================================================
		
	void RuntimeDebugInfo()
	{
		if (Application.isPlaying) 
		{
			EditorGUILayout.HelpBox("Current values (clamped and cumulative) of heading, pitch and roll",MessageType.None, false);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel ("Values at runtime:");
			EditorGUILayout.LabelField ("heading = " + MSP_Input.GyroAccel.GetHeading() + "   (Unclamped: " + MSP_Input.GyroAccel.GetHeadingUnclamped()+")");
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel (" ");
			EditorGUILayout.LabelField ("pitch = " + MSP_Input.GyroAccel.GetPitch() + "   (" + gyroAccelScript.pitch+")");
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel (" ");
			EditorGUILayout.LabelField ("roll = " + MSP_Input.GyroAccel.GetRoll() + "   (Unclamped: " + MSP_Input.GyroAccel.GetRollUnclamped()+")");
			EditorGUILayout.EndHorizontal();
		}
	} // void RuntimeDebugInfo()
	
	//================================================================================
	
	} // public class GyroAccelEditor : Editor 