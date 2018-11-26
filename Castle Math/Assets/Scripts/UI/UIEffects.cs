using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	
	public void fadeIn(float speed){
        if (disabling) return;
        //	transform.localScale = new Vector3(1f,1f,1f);
        if(coroutine != null) StopCoroutine(coroutine);

        coroutine = FadeInPanelCoroutine(speed);
        StartCoroutine(coroutine);
	}
	 
	private IEnumerator FadeInPanelCoroutine(float speed){
			while(GetComponent<CanvasGroup>().alpha < 1){
			GetComponent<CanvasGroup>().alpha += Time.deltaTime * speed;
			yield return null;
		}
	}
    void OnEnable()
    {
        coroutine = FadeInPanelCoroutine(.5f);
        disabling = false;
    }
    void OnDisable(){
        coroutine = null;
        disabling = true;
    }
}
