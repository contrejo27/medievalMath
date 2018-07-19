using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathFlipper : MonoBehaviour {

    [HideInInspector]
    public bool isOn;

    public FractionTargets fractionTargets;

    IEnumerator coroutine;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

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
