using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberLineManager : MonoBehaviour {

    //public Text[] numbersOnLine;
    public RectTransform[] sliderTicks;
    public List<Transform> targetSpots;
    public RectTransform slider;
    public GameObject targetPrefab;
    public NumberLineQuestion nlq;


    [HideInInspector]
    public List<NumberLineTarget> targetObjects;
    public int currentSliderPos;
    public int currentValue;
    public int targetValue;
    [HideInInspector]
    public int maxAttempts;

    int currentAttempts = 0;



    public void SpawnTargets(int startNumber, int initSliderPos)
    {
        ShuffleList(targetSpots);
        currentValue = startNumber + initSliderPos;
        Debug.Log("init slide pos: " + initSliderPos);
        currentSliderPos = initSliderPos;
        Debug.Log("current slide pos: " + currentValue);
        Debug.Log("target slide pos: " + targetValue);
        StartCoroutine(LerpSlider(initSliderPos));
       
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

        for(int i = 0; i<sliderTicks.Length; i++)
        {
            sliderTicks[i].gameObject.GetComponentInChildren<Text>().text = (start+i).ToString();
        }

        targetObjects[0].SetNumber(1);
        targetObjects[1].SetNumber(-1);
        targetObjects[2].SetNumber(2);
        targetObjects[3].SetNumber(-2);
        targetObjects[4].SetNumber(3);
        targetObjects[5].SetNumber(-3);
        targetObjects[6].SetNumber(Random.Range(4, 10));
        targetObjects[7].SetNumber(Random.Range(-9, -3));
        targetObjects[8].SetNumber(Random.Range(4, 10));
        targetObjects[9].SetNumber(Random.Range(-9, -3));
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
        Debug.Log("Sliding: " + amountToSlide);
        currentAttempts++;
        Debug.Log("Attempt #" + currentAttempts + ", Max attempts: " + maxAttempts);
        if (currentSliderPos + amountToSlide >= 0 && currentSliderPos + amountToSlide <= 21)
        {
            currentValue += amountToSlide;
            currentSliderPos += amountToSlide;

            Debug.Log("(in manager) CurrentValue: " + currentValue + ", target value: " + targetValue);

            StartCoroutine(LerpSlider(Mathf.Clamp(currentSliderPos, 0, 21)));

           
        } else
        {
			Debug.Log ("Invalid!");
            // some kind of feedback that shows this is invalid
        }
        
        
    }

    public void OnDisable()
    {
        slider.anchoredPosition3D = new Vector3(0, slider.anchoredPosition3D.y, slider.anchoredPosition3D.z);
        WipeTargets();
    }

    IEnumerator LerpSlider(int slideToPos)
    {
        float timer = 0;
        float speed = .5f;
        float initPos = slider.anchoredPosition3D.x;
        while(timer < speed)
        {
            timer += Time.deltaTime;
            slider.anchoredPosition3D = new Vector3(Mathf.SmoothStep(initPos, sliderTicks[slideToPos].anchoredPosition3D.x + 0.45f, timer/speed),slider.anchoredPosition3D.y,slider.anchoredPosition3D.z);

            yield return null;
        }
        if (currentValue == targetValue)
        {
            nlq.CheckAnswer(currentValue, false);
        }else if (currentAttempts == maxAttempts+10)
        {
            nlq.CheckAnswer(currentValue, true);
        }
    }

}
