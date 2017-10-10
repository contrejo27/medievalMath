using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEffects : MonoBehaviour {
	
	 public void fadeOut(float speed)
	 {
		StartCoroutine(fadePanelCoroutine(speed));
	 }
	 
	private IEnumerator fadePanelCoroutine(float speed)    {
		
		while(GetComponent<CanvasGroup>().alpha > 0){
			GetComponent<CanvasGroup>().alpha -= Time.deltaTime * speed;
			yield return null;
		}

		this.gameObject.SetActive (false);
	//	transform.localScale = new Vector3(0f,0f,0f);
	}
	
	public void fadeIn(float speed)
	{
	//	transform.localScale = new Vector3(1f,1f,1f);
		StartCoroutine(fadeInPanelCoroutine(speed));
	}
	 
	private IEnumerator fadeInPanelCoroutine(float speed)    {
		
		while(GetComponent<CanvasGroup>().alpha < 1){
			GetComponent<CanvasGroup>().alpha += Time.deltaTime * speed;
			yield return null;
		}
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
