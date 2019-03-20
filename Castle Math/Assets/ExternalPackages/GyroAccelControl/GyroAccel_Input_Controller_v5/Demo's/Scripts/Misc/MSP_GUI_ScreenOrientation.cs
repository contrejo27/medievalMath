//================================================================================
// GUI_ScreenOrientation
//================================================================================
//
// Display "ScreenOrientation" menu button at the upper right part of the screen,
// allowing to change the screen orientation on a handheld device
//
//================================================================================

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//================================================================================

public class MSP_GUI_ScreenOrientation : MonoBehaviour 
{
	private GameObject landscapeLeft;
	private GameObject landscapeRight;
	private GameObject portraitUp;
	private GameObject portraitDown;
	private GameObject editorWarningMessage; 

	//================================================================================

	void Awake() 
	{
		landscapeLeft = GameObject.Find ("LandscapeLeft");
		landscapeLeft.GetComponent<Toggle>().isOn = (Screen.orientation  == ScreenOrientation.LandscapeLeft) ? true : false;
		landscapeLeft.SetActive(false);
		//
		landscapeRight = GameObject.Find ("LandscapeRight");
		landscapeRight.GetComponent<Toggle>().isOn = (Screen.orientation  == ScreenOrientation.LandscapeRight) ? true : false;
		landscapeRight.SetActive(false);
		//
		portraitUp = GameObject.Find ("PortraitUp");
		portraitUp.GetComponent<Toggle>().isOn = (Screen.orientation  == ScreenOrientation.Portrait) ? true : false;
		portraitUp.SetActive(false);
		//
		portraitDown = GameObject.Find ("PortraitDown");
		portraitDown.GetComponent<Toggle>().isOn = (Screen.orientation  == ScreenOrientation.PortraitUpsideDown) ? true : false;
		portraitDown.SetActive(false);
		//
		editorWarningMessage = GameObject.Find ("EditorWarningMessage");
		editorWarningMessage.SetActive(false);
	}

	//================================================================================

	public void GUI_ToggleOrientationMenu() 
	{
		landscapeLeft.SetActive(!landscapeLeft.activeInHierarchy);
		landscapeRight.SetActive(!landscapeRight.activeInHierarchy);
		portraitUp.SetActive(!portraitUp.activeInHierarchy);
		portraitDown.SetActive(!portraitDown.activeInHierarchy);
		if (Application.isEditor) 
		{
			editorWarningMessage.SetActive(!editorWarningMessage.activeInHierarchy);
		}
	}

	//================================================================================

	public void GUI_SetScreenOrientation_LandscapeLeft() 
	{
		Screen.orientation = ScreenOrientation.LandscapeLeft;	
	}

	//================================================================================

	public void GUI_SetScreenOrientation_LandscapeRight() 
	{
		Screen.orientation = ScreenOrientation.LandscapeRight;	
	}

	//================================================================================

	public void GUI_SetScreenOrientation_PortraitUp() 
	{
		Screen.orientation = ScreenOrientation.Portrait;	
	}

	//================================================================================

	public void GUI_SetScreenOrientation_PortraitDown() 
	{
		Screen.orientation = ScreenOrientation.PortraitUpsideDown;	
	}
}
