using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemBillboard : MonoBehaviour {
    float risingSpeed = .7f;
    float risignFriction = .8f;
    float lifeTime = 1.5f;
    public SpriteRenderer sr;
    
	// Use this for initialization
	void Start () {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position, Vector3.up);
        StartCoroutine(FadeAndDie());
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += new Vector3(0, risingSpeed, 0);
        risingSpeed *= risignFriction;
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position, Vector3.up);
	}

    IEnumerator FadeAndDie()
    {
        yield return new WaitForSeconds(lifeTime / 2);
        float newLT = lifeTime / 2;
        float timer = 0;
        float alphaVal = 1;
        Color currentColor = sr.color;
        while (timer < newLT)
        {
            timer += Time.deltaTime;
            alphaVal = Mathf.Lerp(1, 0, timer / newLT);
            sr.color = new Color(currentColor.r, currentColor.g, currentColor.b, alphaVal);
            yield return null;
        }
        Destroy(gameObject);
    }
}
