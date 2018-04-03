using UnityEngine;
using System.Collections;

public class debugText : MonoBehaviour
{
	float deltaTime = 0.0f;
	 bool initialScene =false;

	void Start(){
			/* potential fix for having to load from scene 0
			DontDestroyOnLoad(this.gameObject);
	 		if(!initialScene){
				initialScene = true;
				SceneManager.LoadScene (0);
			}
			*/
	}

	void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}
 
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
 
		GUIStyle style = new GUIStyle();
 
		Rect rect = new Rect(50, 50, w, h * 3 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 3 / 100;
		style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}
}