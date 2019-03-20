using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace MSP_Input 
{
	public class VirtualButton : MonoBehaviour 
	{
		public enum ButtonStatus {Up, Down, GoingUp, GoingDown};

		[System.Serializable]
		public class SendMessageReceiver
		{
			public GameObject messageReceiver;
			public string functionName;
		}
		
		[System.Serializable]
		public class Button 
		{
			public string name = "virtual button";
			public bool enabled = true;
			public Texture textureUnpressed;
			public Texture texturePressed;
			[HideInInspector]
			public Texture buttonTexture;
			public Vector2 centerDefault = new Vector2 (0.5f, 0.5f);
			[HideInInspector]
			public Vector2 center = new Vector2 (0.5f, 0.5f);
			[HideInInspector]
			public Vector2 centerNew = new Vector2 (0.5f, 0.5f);
			public Vector2 buttonSize = new Vector2(0.1f, 0.1f);
			[HideInInspector] 
			public Rect buttonRect = new Rect(1,1,10,10);
			public bool forceSquareButton = true;
			public bool useAsSwitch = false;
			public bool moveWithTouch = false;
			public bool autoUpdateEnabled = false;
			public List<SendMessageReceiver> autoUpdateButtonDownMessageReceivers = new List<SendMessageReceiver>();
			public List<SendMessageReceiver> autoUpdateButtonGoingDownMessageReceivers = new List<SendMessageReceiver>();
			public List<SendMessageReceiver> autoUpdateButtonGoingUpMessageReceivers = new List<SendMessageReceiver>();
			public List<SendMessageReceiver> autoUpdateButtonUpMessageReceivers = new List<SendMessageReceiver>();
			[HideInInspector]
			public bool isActive = false;
			[HideInInspector]
			public ButtonStatus status = ButtonStatus.Up;
			[HideInInspector]
			public int touchID = -1;
			[HideInInspector]
			public bool showSettingsInInspector = true;
			[Range(0.0f,1.0f)]
			public float smoothingTime = 0.1f;
			public KeyCode editorSimulationKey = KeyCode.None;
		}
		
		public List<Button> virtualButtons = new List<Button>();	
		static public List<Button> _virtualButtons = new List<Button>();	

		private float sw;
		private float sh;

		//================================================================================

		void Update() 
		{
			if (Application.isPlaying) {
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
			foreach (Button b in virtualButtons) 
			{
				if (!b.enabled)
				{
					b.touchID = -1;
					b.isActive = false;
					b.status = ButtonStatus.Up;
					b.buttonTexture = b.textureUnpressed;
				} else {
					// Touch input
					foreach (Touch touch in Input.touches) 
					{
						Vector2 touchScreenPoint = new Vector2(touch.position.x, sh-touch.position.y);
						Vector2 centerTouch = new Vector2(touchScreenPoint.x/sw, 1f-touchScreenPoint.y/sh);
						//
						if (touch.phase == TouchPhase.Began) 
						{
							if (b.buttonRect.Contains (touchScreenPoint)) 
							{
								b.touchID = touch.fingerId;
								b.isActive = true;
								b.centerNew = b.moveWithTouch ? centerTouch : b.centerDefault;
							}
						}
						//
						if (b.touchID == touch.fingerId) 
						{
							if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended) 
							{
								b.isActive = false;
								b.touchID = -1;
								b.centerNew = b.centerDefault;
							} else {
								b.centerNew = b.moveWithTouch ? centerTouch : b.centerDefault;
							}
						}
					}

					// Mouse input in editor
					if (Application.isEditor || !Input.touchSupported) 
					{
						Vector2 mouseScreenPoint = new Vector2(Input.mousePosition.x, sh-Input.mousePosition.y);
						Vector2 centerTouch = new Vector2(mouseScreenPoint.x/sw, 1f-mouseScreenPoint.y/sh);
						if (b.touchID == -1) { 
							if (Input.GetMouseButtonDown (0)) 
							{
								if (b.buttonRect.Contains (mouseScreenPoint)) 
								{
									b.touchID = 100;
									b.isActive = true;
									b.centerNew = b.moveWithTouch ? centerTouch : b.centerDefault;
								}
							}
						}
						if (b.touchID == 100) 
						{
							if (Input.GetMouseButtonUp (0)) 
							{
								b.isActive = false;
								b.touchID = -1;
								b.centerNew = b.centerDefault;
							} else {
								b.centerNew = b.moveWithTouch ? centerTouch : b.centerDefault;
							}
						}
					}
					
					// Editor Simulation Key
					if (Application.isEditor || !Input.touchSupported) 
					{
						if (!b.useAsSwitch)
						{
							if (Input.GetKeyDown(b.editorSimulationKey))
							{
								b.isActive = true;
							}
							if (Input.GetKeyUp(b.editorSimulationKey))
							{
								b.isActive = false;
							}
						} else {
							if (Input.GetKeyDown(b.editorSimulationKey))
							{
								b.isActive = !b.isActive;
							}
						}
					}
					
					// 
					float smoothFactor = (b.smoothingTime > Time.deltaTime) ? Time.deltaTime / b.smoothingTime : 1f;
					b.center = Vector2.Lerp(b.center,b.centerNew,smoothFactor);

					// handle button status
					if (b.status == ButtonStatus.GoingUp) 
					{
						b.status = ButtonStatus.Up;
					}
					if (b.status == ButtonStatus.GoingDown) 
					{
						b.status = ButtonStatus.Down;
					}
					
					if (!b.useAsSwitch) 
					{
						if (b.isActive) 
						{
							if (b.status == ButtonStatus.Up) 
							{
								b.status = ButtonStatus.GoingDown;
							}
						} else {
							if (b.status == ButtonStatus.Down) 
							{
								b.status = ButtonStatus.GoingUp;
							}
						}
					} else {
						if (b.isActive) 
						{
							if (b.status == ButtonStatus.Up) 
							{
								b.status = ButtonStatus.GoingDown;
							}
							if (b.status == ButtonStatus.Down) 
							{
								b.status = ButtonStatus.GoingUp;
							}
							b.isActive = false;
						}
					}

					b.buttonTexture = (b.status == VirtualButton.ButtonStatus.Down || b.status == VirtualButton.ButtonStatus.GoingDown) ? b.texturePressed : b.textureUnpressed;

					// autoUpdate

					if (b.autoUpdateEnabled)
					{
						foreach (SendMessageReceiver receiver in b.autoUpdateButtonDownMessageReceivers) 
						{
							if (receiver.messageReceiver && b.status == ButtonStatus.Down) 
							{
								receiver.messageReceiver.SendMessage(receiver.functionName,SendMessageOptions.DontRequireReceiver);
							}					
						}
						foreach (SendMessageReceiver receiver in b.autoUpdateButtonGoingDownMessageReceivers) 
						{
							if (receiver.messageReceiver && b.status == ButtonStatus.GoingDown) 
							{
								receiver.messageReceiver.SendMessage(receiver.functionName,SendMessageOptions.DontRequireReceiver);
							}					
						}
						foreach (SendMessageReceiver receiver in b.autoUpdateButtonGoingUpMessageReceivers) 
						{
							if (receiver.messageReceiver && b.status == ButtonStatus.GoingUp) 
							{
								receiver.messageReceiver.SendMessage(receiver.functionName,SendMessageOptions.DontRequireReceiver);
							}					
						}
						foreach (SendMessageReceiver receiver in b.autoUpdateButtonUpMessageReceivers) 
						{
							if (receiver.messageReceiver && b.status == ButtonStatus.Up) 
							{
								receiver.messageReceiver.SendMessage(receiver.functionName,SendMessageOptions.DontRequireReceiver);
							}					
						}
					}
				} // if (!b.enabled)

			} // foreach (Button b in virtualButtons)

			_virtualButtons = new List<Button> (virtualButtons);

		} // void UpdateRuntime() 

		//================================================================================

		public void CalculateRects() 
		{
			sw = (float)Screen.width;
			sh = (float)Screen.height;
			foreach (Button b in virtualButtons) 
			{
				b.buttonRect.width = (int)(b.buttonSize.x * sw);
				b.buttonRect.height = b.forceSquareButton ? b.buttonRect.width : (int)(b.buttonSize.y * sh);
				b.buttonRect.center = new Vector2 ((int)(sw * b.center.x), (int)(sh * (1f - b.center.y)));
			}
		}

		//================================================================================
		
		static public bool GetButton(string name) 
		{
			foreach (Button _virtualButton in _virtualButtons) 
			{
				if (_virtualButton.name == name) 
				{
					if (_virtualButton.status == ButtonStatus.Down || _virtualButton.status == ButtonStatus.GoingDown) 
					{
						return true;
					} else {
						return false;
					}
				}
			}
			ErrorHandling.LogError("Warning: Button with name '"+name+"' does not exist");
			return false;
		}

		//================================================================================

		static public bool GetButtonUp(string name) 
		{
			foreach (Button _virtualButton in _virtualButtons) 
			{
				if (_virtualButton.name == name) 
				{
					if (_virtualButton.status == ButtonStatus.GoingUp) 
					{
						return true;
					} else {
						return false;
					}
				}
			}
			ErrorHandling.LogError("Warning: Button with name '"+name+"' does not exist");
			return false;
		}

		//================================================================================

		static public bool GetButtonDown(string name) 
		{
			foreach (Button _virtualButton in _virtualButtons) 
			{
				if (_virtualButton.name == name) 
				{
					if (_virtualButton.status == ButtonStatus.GoingDown) 
					{
						return true;
					} else {
						return false;
					}
				}
			}
			ErrorHandling.LogError("Warning: Button with name '"+name+"' does not exist");
			return false;
		}

		//================================================================================
		
		static public void Enable(string name) 
		{
			foreach (Button _virtualButton in _virtualButtons) 
			{
				if (_virtualButton.name == name) 
				{
					_virtualButton.enabled = true;
					return;
				}
			}
			ErrorHandling.LogError("Warning: Button with name '"+name+"' does not exist");
			return;
		}
		
		//================================================================================
		
		static public void Disable(string name) 
		{
			foreach (Button _virtualButton in _virtualButtons) 
			{
				if (_virtualButton.name == name) 
				{
					_virtualButton.enabled = false;
					return;
				}
			}
			ErrorHandling.LogError("Warning: Button with name '"+name+"' does not exist");
			return;
		}
		
		//================================================================================

		static public void SetCenter(string name, Vector2 newCenter)
		{
			foreach (Button _virtualButton in _virtualButtons) 
			{
				if (_virtualButton.name == name) 
				{
					_virtualButton.centerDefault = newCenter;
					_virtualButton.centerNew = newCenter;
				}
			}
			return;
		}
		
		//================================================================================
		
		static public void SetSize(string name, Vector2 newSize)
		{
			foreach (Button _virtualButton in _virtualButtons) 
			{
				if (_virtualButton.name == name) 
				{
					_virtualButton.buttonSize = newSize;
				}
			}
			return;
		}

		//================================================================================

	} // public class VirtualButton

	} //namespace MSP_Input