using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumerLineManager : MonoBehaviour {

    public Text[] numbersOnLine;
    public Transform[] sliderTicks;
    public int currentSliderPos;
    public Transform slider;
    public int currentValue;
    public int targetValue;

    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void WriteNumbers(int start)
    {
        currentValue = start + 5;
        for(int i = 0; i<=10; i++)
        {
            numbersOnLine[i].text = (start + i).ToString();
        }
    }

    public void SlideSlider(int amountToSlide)
    {
        if (currentSliderPos + amountToSlide > 0 && currentSliderPos + amountToSlide < 11)
        {
            currentValue += amountToSlide;
            
            StartCoroutine(LerpSlider(currentSliderPos + amountToSlide));

            if(currentValue == targetValue)
            {
                //do something
            }
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
