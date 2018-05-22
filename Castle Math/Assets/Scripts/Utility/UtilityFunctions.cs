using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityFunctions : MonoBehaviour {

    public static UtilityFunctions instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UIPositionLerp(RectTransform rt, float time, Vector3 startPos, Vector3 targetPos, bool isLocal, bool setInactive = false)
    {
        StartCoroutine(UIPositionLerpCoroutine(rt, time, startPos, targetPos, setInactive));
    }

    IEnumerator UIPositionLerpCoroutine(RectTransform rt, float time, Vector3 initPos, Vector3 targetPos, bool setInactive)
    {
        float timer = 0;
        while(timer < time && rt.gameObject.activeSelf)
        {
            timer += Time.deltaTime;
            rt.anchoredPosition = Vector3.Lerp(initPos, targetPos, timer/time);
            yield return null;
        }
        if (setInactive)
        {
            rt.gameObject.SetActive(false);
        }
    }

    //IEnumerator UITextFadeCoroutine
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
