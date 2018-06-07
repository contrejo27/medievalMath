using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEffects : MonoBehaviour {
     IEnumerator coroutine;
     bool disabling;

	 public void fadeOut(float speed)
	 {
        if(coroutine != null) StopCoroutine(coroutine);
        coroutine = fadePanelCoroutine(speed);

        StartCoroutine(coroutine);
	 }
	 
	private IEnumerator fadePanelCoroutine(float speed)    {
		
		while(GetComponent<CanvasGroup>().alpha > 0){
			GetComponent<CanvasGroup>().alpha -= Time.deltaTime * speed;
			yield return null;
		}

	//	transform.localScale = new Vector3(0f,0f,0f);
	}
	
	public void fadeIn(float speed)
	{
        if (disabling) return;
        //	transform.localScale = new Vector3(1f,1f,1f);
        if(coroutine != null) StopCoroutine(coroutine);

        coroutine = fadeInPanelCoroutine(speed);
        StartCoroutine(coroutine);
	}
	 
	private IEnumerator fadeInPanelCoroutine(float speed)    {
		
		while(GetComponent<CanvasGroup>().alpha < 1){
			GetComponent<CanvasGroup>().alpha += Time.deltaTime * speed;
			yield return null;
		}
	}
	void OnDisable()
    {
        coroutine = null;
        disabling = true;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
