using UnityEngine;
using System.Collections;
using MSP_Input;

namespace MSP_Input 
{
	[ExecuteInEditMode]  
	public class GUIDraw : MonoBehaviour 
	{
		static private VirtualTouchpad virtuaTouchpadScript;
		static private VirtualJoystick virtualJoystickScript;
		static private VirtualButton virtualButtonScript;
		
		public bool showInEditMode = true;
		
		//================================================================================
		
		void Awake()
		{
			virtuaTouchpadScript = GetComponent<VirtualTouchpad>();
			virtualJoystickScript = GetComponent<VirtualJoystick>();
			virtualButtonScript = GetComponent<VirtualButton>();
		}
		
		//================================================================================
		
		void OnGUI() 
		{
			if (!Application.isPlaying && showInEditMode) 
			{
				virtuaTouchpadScript = GetComponent<VirtualTouchpad> ();
				virtuaTouchpadScript.CalculateRects();
				foreach (VirtualTouchpad.Touchpad t in virtuaTouchpadScript.virtualTouchpads) {
					if (t.backgroundTexture && t.enabled) 
					{
						GUI.DrawTexture (t.touchpadScreenRect, t.backgroundTexture);
					}
				}
				
				virtualJoystickScript = GetComponent<VirtualJoystick>();
				virtualJoystickScript.CalculateRects();
				foreach (VirtualJoystick.Joystick j in virtualJoystickScript.virtualJoysticks) 
				{
					if (j.backgroundTexture && j.enabled)
					{
						GUI.DrawTexture (j.backgroundRect, j.backgroundTexture);
					}
					if (j.buttonTexture && j.enabled)
					{
						GUI.DrawTexture (j.buttonRect, j.buttonTexture);
					}
				}
				
				virtualButtonScript = GetComponent<VirtualButton>();
				virtualButtonScript.CalculateRects();
				foreach (VirtualButton.Button b in virtualButtonScript.virtualButtons) 
				{
					if (b.buttonTexture && b.enabled)
					{
						GUI.DrawTexture (b.buttonRect, b.buttonTexture);
					}
				}	
			} 
		} // void OnGUI() 
		
		//================================================================================
		
		void OnRenderObject() 
		{
			if (Application.isPlaying) {
				GL.PushMatrix ();
				GL.LoadPixelMatrix (0, Screen.width, Screen.height, 0);
				
				foreach (VirtualTouchpad.Touchpad t in virtuaTouchpadScript.virtualTouchpads) 
				{
					if (!t.hideBackgroundTexture && t.enabled) 
					{
						if (t.backgroundTexture) 	Graphics.DrawTexture (t.touchpadScreenRect, t.backgroundTexture);
					}
				}
				
				foreach (VirtualJoystick.Joystick j in virtualJoystickScript.virtualJoysticks) 
				{
					if ((j.isActive || j.alwaysVisible) && (j.enabled))
					{
						if (j.backgroundTexture)	Graphics.DrawTexture (j.backgroundRect, j.backgroundTexture);
						if (j.buttonTexture)		Graphics.DrawTexture (j.buttonRect, j.buttonTexture);
					}
				}
				
				foreach (VirtualButton.Button b in virtualButtonScript.virtualButtons) 
				{
					if (b.enabled)
					{ 
						if (b.buttonTexture)	Graphics.DrawTexture (b.buttonRect, b.buttonTexture);
					}
				}	
				
				GL.PopMatrix ();
			}
		} // void OnRenderObject()
		
		//================================================================================
		
	} // public class MSP_GUI_draw : MonoBehaviour 
	
} // namespace MSP_Input 


