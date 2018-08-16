using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathFlipper : BaseInteractableObject {

    [HideInInspector]
    public bool isOn;

    public FractionTargets fractionTargets;

    float clickCooldown = .2f;
    float clickCooldownTimer = 0;
    bool shotsFired;

    IEnumerator coroutine;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (shotsFired)
        {
            if(clickCooldownTimer < clickCooldown)
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

    public override void OnPassOver(){}

    public override void OnEndPassOver(){}

    public override void OnInteract()
    {
        if (!shotsFired)
        {
            base.OnInteract();
            //Debug.Log("Flipping");
            StartCoroutine(DelayFlip());


        }
        /*
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = Flip();
        StartCoroutine(coroutine);
        */
        //Destroy(other.gameObject);
    }

    /*
    void OnCollisionEnter(Collision other)
    {
        //Debug.Log("Flipping");
        isOn = !isOn;
        if (isOn) fractionTargets.IncrementFlips();
        else fractionTargets.DecrementFlips();

        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = Flip();
        StartCoroutine(coroutine);

        //Destroy(other.gameObject);

    }
    */

    void StartFlip()
    {
        isOn = !isOn;
        if (isOn) fractionTargets.IncrementFlips();
        else fractionTargets.DecrementFlips();

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = Flip();
        StartCoroutine(coroutine);
    }

    IEnumerator DelayFlip()
    {
        yield return new WaitForSeconds(.15f);

        StartFlip();
        
    }

    IEnumerator Flip(bool b = true)
    {
        Quaternion target = isOn ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 359.9f, 0);
        Quaternion initial = transform.rotation;
        float timer = 0;
        //Debug.Log("Initial Rotation: " + transform.eulerAngles.y);
        //Debug.Log("Target y in euler: " + target.eulerAngles.y);
        while ((transform.eulerAngles.y < target.eulerAngles.y && !isOn)
            || ((transform.eulerAngles.y > target.eulerAngles.y || transform.eulerAngles.y == 0) && isOn))
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(initial, target, timer*3);
            //Debug.Log("New rotation: " + transform.eulerAngles.y);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, isOn ? 180 : 0, 0);
    }
}
