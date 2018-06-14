using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathFlipper : MonoBehaviour {

    [HideInInspector]
    public bool on;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision other)
    {

    }

    IEnumerator Flip()
    {
        Quaternion target = on ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 359.9f, 0);
        Quaternion initial = transform.rotation;
        float timer = 0;
        Debug.Log("Target y in euler: " + target.eulerAngles.y);
        while (transform.eulerAngles.y < target.eulerAngles.y)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(initial, target, timer/3);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, on ? 180 : 0, 0);
    }
}
