using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace MSP_Input 
{
	[ExecuteInEditMode]  
	public class VirtualTouchpad : MonoBehaviour 
	{
		[System.Serializable]
		public class SendMessageReceiver
		{
			public GameObject messageReceiver;
			public string functionName;
		}
		
		public enum EditorTouchpadSimulationMode {None, Keyboard_WASD, Keyboard_CursorKeys, Keyboard_Custom};
		[System.Serializable]
		public class EditorTouchpadSimulation 
		{
			public EditorTouchpadSimulationMode simulationMode = EditorTouchpadSimulationMode.None;
			public KeyCode keyCode_Left = KeyCode.None;
			public KeyCode keyCode_Right = KeyCode.None;
			public KeyCode keyCode_Up = KeyCode.None;
			public KeyCode keyCode_Down = KeyCode.None;
			public KeyCode keyCode_DoubleTap = KeyCode.None;
		}
		
		[System.Serializable]
 		public class Touchpad 
		{
			public string name = "VirtualTouchpad";
			public bool enabled = true;
			public Texture backgroundTexture; 
			public Rect touchpadRect = new Rect(0.4f,0.4f,0.2f,0.2f);
			[HideInInspector]
			public Rect touchpadScreenRect = new Rect(0.4f,0.4f,0.2f,0.2f);
			public Vector2 axisMultiplier = new Vector2(1f,1f);
			public bool compensateForDeviceRoll = true;
			public bool hideBackgroundTexture = false;
			public bool autoUpdateEnabled = false;
			public bool autoUpdateGyroAccelScript = false;
			public List<SendMessageReceiver> autoUpdateAxisMessageReceivers = new List<SendMessageReceiver>();
			public List<SendMessageReceiver> autoUpdateDoubleTapMessageReceivers = new List<SendMessageReceiver>();
			public List<SendMessageReceiver> autoUpdateDoubleTapHoldMessageReceivers = new List<SendMessageReceiver>();
			[HideInInspector]
			public bool isActive = false;
			[HideInInspector]
			public int touchID = -1;
			[HideInInspector]
			public Vector2 axis = Vector2.zero;
			[HideInInspector]
			public float previousTouchTime = 0f;
			[HideInInspector]
			public bool doubleTap = false;
			[HideInInspector]
			public bool doubleTapHold = false;
			[HideInInspector]
			public bool showSettingsInInspector = true;
			public EditorTouchpadSimulation editorTouchpadSimulation;
		}

		public List<Touchpad> virtualTouchpads = new List<Touchpad>();	
		static public List<Touchpad> _virtualTouchpads = new List<Touchpad>();	

		private float sw;
		private float sh;
		private Vector2 mouseScreenPointOld;

		//================================================================================
		
		void Start() 
		{
			mouseScreenPointOld = new Vector2(Input.mousePosition.x, (float)Screen.height-Input.mousePosition.y);
		}

		//================================================================================
		
		void Update() 
		{
			if (Application.isPlaying) 
			{
				UpdateRuntime ();
			}
		}
		
		//================================================================================

		void UpdateRuntime() 
		{
			CalculateRects();
			//
			sw = (float)Screen.width;
			sh = (float)Screen.height;
			//
			foreach (Touchpad t in virtualTouchpads) 
			{
				if (!t.enabled) 
				{
					t.touchID = -1;
					t.isActive = false;
					t.axis = Vector2.zero;
					t.doubleTap = false;
					t.doubleTapHold = false;
				} else {
					t.isActive = false;
					t.doubleTap = false;
					bool tempDoubleTapHold = false;
					Vector2 delta = Vector2.zero;
					float deltaTimeMax = 0.5f;
					foreach (Touch touch in Input.touches) 
					{
						Vector2 touchScreenPoint = new Vector2 (touch.position.x, sh - touch.position.y);
						if (t.touchpadScreenRect.Contains (touchScreenPoint)) 
						{
							t.isActive = true;
							if (t.doubleTapHold) 
							{
								tempDoubleTapHold = true;
							}
							//
							if (touch.phase == TouchPhase.Began && !t.doubleTap) 
							{
								if ((Time.time - t.previousTouchTime) < deltaTimeMax && Time.time > 0f) 
								{
									t.doubleTap = true;
									tempDoubleTapHold = true;
									t.touchID = touch.fingerId;
								} 
								t.previousTouchTime = Time.time;
							}
							//
							if (t.compensateForDeviceRoll) 
							{
								float alpha = Mathf.Deg2Rad * MSP_Input.GyroAccel.GetRoll ();
								float cosin = Mathf.Cos (alpha);
								float sinus = Mathf.Sin (alpha);
								delta.x = -(touch.deltaPosition.y / sh) * sinus + (touch.deltaPosition.x / sw) * cosin;
								delta.y = (touch.deltaPosition.x / sw) * sinus + (touch.deltaPosition.y / sh) * cosin;
							} else {
								delta = touch.deltaPosition;
								delta.x /= sw;
								delta.y /= sh;
							} 
						}
						//
						if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && touch.fingerId == t.touchID) 
						{
							t.touchID = -1;
							t.doubleTap = false;
							t.doubleTapHold = false;
							tempDoubleTapHold = false;
						}
					}
					
					// Editor: Mouse 
					if ((Application.isEditor || !Input.touchSupported) && !t.isActive)
					{
						Vector2 mouseScreenPoint = new Vector2 (Input.mousePosition.x, sh - Input.mousePosition.y);
						if (Input.GetMouseButton (0) && t.touchpadScreenRect.Contains (mouseScreenPoint)) 
						{
							t.isActive = true;
							if (t.doubleTapHold) 
							{
								tempDoubleTapHold = true;
							}
							//
							if (Input.GetMouseButtonDown (0)) 
							{
								if ((Time.time - t.previousTouchTime) < deltaTimeMax && Time.time > 0f) 
								{
									t.doubleTap = true;
									tempDoubleTapHold = true;
								} 
								t.previousTouchTime = Time.time;
							}
							Vector2 mouseDelta = mouseScreenPoint - mouseScreenPointOld;
							mouseDelta.x /= sw;
							mouseDelta.y /= sh;
							delta += mouseDelta;
						} else {
							t.doubleTap = false;
							tempDoubleTapHold = false;
						}
						mouseScreenPointOld = mouseScreenPoint;
					}
					
					// Editor: Simulation keys
					if (Application.isEditor)
					{
						if (t.editorTouchpadSimulation.simulationMode != VirtualTouchpad.EditorTouchpadSimulationMode.None)
						{
							switch (t.editorTouchpadSimulation.simulationMode)
							{
							case VirtualTouchpad.EditorTouchpadSimulationMode.Keyboard_WASD:
								t.editorTouchpadSimulation.keyCode_Up = KeyCode.W;
								t.editorTouchpadSimulation.keyCode_Left = KeyCode.A;
								t.editorTouchpadSimulation.keyCode_Down = KeyCode.S;
								t.editorTouchpadSimulation.keyCode_Right = KeyCode.D;
								break;
							case VirtualTouchpad.EditorTouchpadSimulationMode.Keyboard_CursorKeys:
								t.editorTouchpadSimulation.keyCode_Up = KeyCode.UpArrow;
								t.editorTouchpadSimulation.keyCode_Left = KeyCode.LeftArrow;
								t.editorTouchpadSimulation.keyCode_Down = KeyCode.DownArrow;
								t.editorTouchpadSimulation.keyCode_Right = KeyCode.RightArrow;
								break;
							default:
								break;			
							}
							//
							if (Input.GetKey(t.editorTouchpadSimulation.keyCode_Right) ||
								Input.GetKey(t.editorTouchpadSimulation.keyCode_Left) ||
								Input.GetKey(t.editorTouchpadSimulation.keyCode_Up) ||
								Input.GetKey(t.editorTouchpadSimulation.keyCode_Down))
							{
								t.isActive = true;
							}
							//
							Vector2 keyDelta = Vector2.zero;
							if (Input.GetKey(t.editorTouchpadSimulation.keyCode_Up))	
							{
								keyDelta.y += Time.deltaTime;
							}
							if (Input.GetKey(t.editorTouchpadSimulation.keyCode_Down))
							{
								keyDelta.y -= Time.deltaTime;
							}
							if (Input.GetKey(t.editorTouchpadSimulation.keyCode_Right))
							{
								keyDelta.x += Time.deltaTime;
							}
							if (Input.GetKey(t.editorTouchpadSimulation.keyCode_Left))
							{
								keyDelta.x -= Time.deltaTime;
							}
							delta += keyDelta;
						} 
						//
						if (Input.GetKeyDown(t.editorTouchpadSimulation.keyCode_DoubleTap))
						{
							t.doubleTap = true;
						}
						if (Input.GetKey(t.editorTouchpadSimulation.keyCode_DoubleTap))
						{
							tempDoubleTapHold = true;
						}
						if (Input.GetKeyUp(t.editorTouchpadSimulation.keyCode_DoubleTap))
						{
							t.doubleTap = false;
							tempDoubleTapHold = false;
						}
					} // if (Application.isEditor)
					//
					//
					delta.x *= t.axisMultiplier.x * t.touchpadRect.width * 100f;
					delta.y *= t.axisMultiplier.y * t.touchpadRect.height * 100f;
					//
					t.axis = delta;
					//
					t.doubleTapHold = tempDoubleTapHold;
					//
					if (t.autoUpdateEnabled) {
						if (t.autoUpdateGyroAccelScript) {
							MSP_Input.GyroAccel.AddFloatToHeadingOffset (t.axis.x);
							MSP_Input.GyroAccel.AddFloatToPitchOffset (t.axis.y);
						}
						foreach (SendMessageReceiver receiver in t.autoUpdateAxisMessageReceivers) 
						{
							if (receiver.messageReceiver) {
								receiver.messageReceiver.SendMessage (receiver.functionName, t.axis, SendMessageOptions.DontRequireReceiver);
							}
						}
						foreach (SendMessageReceiver receiver in t.autoUpdateDoubleTapMessageReceivers) 
						{
							if (receiver.messageReceiver && t.doubleTap) {
								receiver.messageReceiver.SendMessage (receiver.functionName, SendMessageOptions.DontRequireReceiver);
							}
						}
						foreach (SendMessageReceiver receiver in t.autoUpdateDoubleTapHoldMessageReceivers) 
						{
							if (receiver.messageReceiver && t.doubleTapHold) {
								receiver.messageReceiver.SendMessage (receiver.functionName, SendMessageOptions.DontRequireReceiver);
							}
						}
					}
				} // if (!t.enabled) 
			} // foreach (Touchpad t in virtualTouchpads)

			_virtualTouchpads = new List<Touchpad>(virtualTouchpads);
		} // void UpdateRuntime() 
	
		//================================================================================

		public void CalculateRects() 
		{
			sw = (float)Screen.width;
			sh = (float)Screen.height;
			foreach (Touchpad t in virtualTouchpads) 
			{
				t.touchpadScreenRect.width = (int)(t.touchpadRect.width * sw);
				t.touchpadScreenRect.height = (int)(t.touchpadRect.height * sh); 
				t.touchpadScreenRect.x = (int)(t.touchpadRect.x * sw);
				t.touchpadScreenRect.y = (int)(t.touchpadRect.y * sh);
			}
		}

		//================================================================================

		static public Vector2 GetAxis(string name) 
		{
			foreach (Touchpad _virtualTouchpad in _virtualTouchpads) 
			{
				if (_virtualTouchpad.name == name) 
				{
					return _virtualTouchpad.axis;
				}
			}
			ErrorHandling.LogError("Warning: Touchpad with name '"+name+"' does not exist");
			return Vector2.zero;
		}
		
		//================================================================================
		
		static public bool GetDoubleTap(string name) 
		{
			foreach (Touchpad _virtualTouchpad in _virtualTouchpads) 
			{
				if (_virtualTouchpad.name == name) 
				{
					return _virtualTouchpad.doubleTap;
				}
			}
			ErrorHandling.LogError("Warning: Touchpad with name '"+name+"' does not exist");
			return false;
		}

		//================================================================================
		
		static public bool GetDoubleTapHold(string name) 
		{
			foreach (Touchpad _virtualTouchpad in _virtualTouchpads) 
			{
				if (_virtualTouchpad.name == name) 
				{
					return _virtualTouchpad.doubleTapHold;
				}
			}
			ErrorHandling.LogError("Warning: Touchpad with name '"+name+"' does not exist");
			return false;
		}
		
		//================================================================================
		
		static public void Enable(string name) 
		{
			foreach (Touchpad _virtualTouchpad in _virtualTouchpads) 
			{
				if (_virtualTouchpad.name == name) 
				{
					_virtualTouchpad.enabled = true;
					return;
				}
			}
			ErrorHandling.LogError("Warning: Touchpad with name '"+name+"' does not exist");
			return;
		}
		
		//================================================================================
		
		static public void Disable(string name) 
		{
			foreach (Touchpad _virtualTouchpad in _virtualTouchpads) 
			{
				if (_virtualTouchpad.name == name) 
				{
					_virtualTouchpad.enabled = false;
					return;
				}
			}
			ErrorHandling.LogError("Warning: Touchpad with name '"+name+"' does not exist");
			return;
		}

		//================================================================================
		
		static public void SetRect(string name, Rect newRect)
		{
			foreach (Touchpad _virtualTouchpad in _virtualTouchpads) 
			{
				if (_virtualTouchpad.name == name) 
				{
					_virtualTouchpad.touchpadRect = newRect;
				}
			}
			return;
		}
		
		//================================================================================

	} // public class VirtualTouchpad

} //namespace MSP_Input












