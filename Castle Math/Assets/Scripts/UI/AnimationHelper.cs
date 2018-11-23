using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelper : MonoBehaviour {
    
    public enum AnimationType { Rotate };
    bool loopAnim = true;
    public AnimationType animationType;
    float x;

    // Use this for initialization
    void OnEnable() {
        loopAnim = true;
        x = 0.0f;

        StartCoroutine(animateObject(animationType));

    }
    private void OnDisable()
    {
        loopAnim = false;
    }

    IEnumerator animateObject(AnimationType type)
    {
        while (loopAnim) { 
            if(animationType == AnimationType.Rotate)
            {
                x += Time.deltaTime*80f;
                gameObject.transform.localRotation = Quaternion.Euler(gameObject.transform.localRotation.x, gameObject.transform.localRotation.y, x);
            }
            yield return new WaitForSeconds(.06f);
        }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
