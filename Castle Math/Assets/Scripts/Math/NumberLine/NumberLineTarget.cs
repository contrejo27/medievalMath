using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberLineTarget : MonoBehaviour {

    public Text numberText;
    public NumberLineManager nlm;

    int value;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetNumber(int targetString)
    {
        numberText.text = targetString.ToString();
        value = targetString;
    }

    private void OnCollisionEnter(Collision other)
    {
        nlm.SlideSlider(value);

    }
}
