using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberLineManager : MonoBehaviour {

    //public Text[] numbersOnLine;
    public Transform[] sliderTicks;
    public Transform[] targetSpots;
    public Transform slider;
    public GameObject targetPrefab;
    public int maxAttempts;

    [HideInInspector]
    public List<NumberLineTarget> targetObjects;
    [HideInInspector]
    public int currentSliderPos;
    [HideInInspector]
    public int currentValue;
    [HideInInspector]
    public int targetValue;

    int currentAttempts = 0;

    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnTargets(int startNumber)
    {
        ShuffleList(targetObjects);
        for(int i = 0; i< 10; i++)
        {
            GameObject temp = Instantiate(targetPrefab, targetSpots[i]) as GameObject;
            temp.transform.localPosition = Vector3.zero;
            temp.transform.rotation = Quaternion.identity;
            targetObjects.Add(temp.GetComponent<NumberLineTarget>());
            targetObjects[i].nlm = this;
        }
        WriteNumbers(startNumber);
    }

    public void WipeTargets()
    {
        int initSize = targetObjects.Count;
        for(int i = 0; i<initSize; i++)
        {
            Destroy(targetObjects[0].gameObject);
            targetObjects.RemoveAt(0);
        }
        currentAttempts = 0;
    }

    public void WriteNumbers(int start)
    {
        currentValue = start + 5;
        for(int i = 0; i<=10; i++)
        {
            targetObjects[i].SetNumber(start + i);
        }
    }

    public void ShuffleList<T>(List<T> list)
    {
        for(int i = 0; i<list.Count; i++)
        {
            T temp = list[i];
            int r = Random.Range(i, list.Count);
            list[i] = list[r];
            list[r] = temp;

        }
    }

    public void SlideSlider(int amountToSlide)
    {
        currentAttempts++;
        if (currentSliderPos + amountToSlide > 0 && currentSliderPos + amountToSlide < 11)
        {
            currentValue += amountToSlide;
            
            StartCoroutine(LerpSlider(currentSliderPos + amountToSlide));

            if(currentValue == targetValue)
            {
                //do something
            }
        }
        if(currentAttempts == maxAttempts)
        {
            // do something;
        }
    }

    IEnumerator LerpSlider(int slideToPos)
    {
        float timer = 0;
        float initPos = slider.position.x;
        while(timer < 1)
        {
            slider.position = new Vector3(Mathf.SmoothStep(initPos, sliderTicks[slideToPos].position.x, timer),slider.position.y,slider.position.z);
            yield return null;
        }
    }

}
