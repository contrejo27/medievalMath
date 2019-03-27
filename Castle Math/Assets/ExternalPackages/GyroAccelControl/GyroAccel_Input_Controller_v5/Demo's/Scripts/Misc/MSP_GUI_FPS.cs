//================================================================================
// GUI_FPS
//================================================================================
//
// Display "Frames Per Second" GUI item
//
//================================================================================

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//================================================================================

public class MSP_GUI_FPS : MonoBehaviour 
{	
	public float FPSupdateTime = 0.5f;
	private Text text;

	//================================================================================

	void Awake() 
	{
		text = gameObject.GetComponent<Text>();
	}

	//================================================================================

	void Update () 
	{
		if (Time.time > FPSupdateTime) 
		{
			string FPSstring = "FPS: "+(1f/Time.deltaTime).ToString("#.0");
			FPSupdateTime = Time.time + FPSupdateTime; 
			text.text = FPSstring.ToString();
		}
	}
}