using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberLineTarget : BaseInteractableObject {

    float clickCooldown = .2f;
    float clickCooldownTimer = 0;
    bool shotsFired;

    public Text numberText;
    public NumberLineManager nlm;

    int value;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (shotsFired)
        {
            if (clickCooldownTimer < clickCooldown)
            {
                clickCooldownTimer += Time.deltaTime;
            }
            else
            {
                clickCooldownTimer = 0;
                shotsFired = false;
            }
        }
    }

    public override void OnInteract()
    {
        if (!shotsFired)
        {
            StartCoroutine(DelaySetNumber());
        }

    }

    public void SetNumber(int targetString)
    {
        numberText.text = targetString.ToString();
        value = targetString;
    }

    /*
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("NLTargetHit!");
        nlm.SlideSlider(value);

    }
    */

    IEnumerator DelaySetNumber()
    {
        yield return new WaitForSeconds(.15f);

        nlm.SlideSlider(value);
    }
}
