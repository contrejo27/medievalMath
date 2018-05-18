﻿using System.Collections;
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

    public void UILerp(RectTransform rt, float time, Vector3 startPos, Vector3 targetPos, bool isLocal)
    {
        StartCoroutine(UILerpCoroutine(rt, time, startPos, targetPos));
    }

    IEnumerator UILerpCoroutine(RectTransform rt, float time, Vector3 initPos, Vector3 targetPos)
    {
        float timer = 0;
        while(timer < time)
        {
            timer += Time.deltaTime;
            rt.anchoredPosition = Vector3.Lerp(initPos, targetPos, timer/time);
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
