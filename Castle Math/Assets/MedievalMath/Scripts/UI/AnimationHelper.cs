using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelper : MonoBehaviour {
    
    public enum AnimationType { None, Rotate, SetLookAtCam };
    bool loopAnim = true;
    public AnimationType animationType;
    float x;
    Animator anim;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }
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
                transform.localRotation = Quaternion.Euler(gameObject.transform.localRotation.x, gameObject.transform.localRotation.y, x);
            }
            if (animationType == AnimationType.SetLookAtCam)
            {
                transform.LookAt(Camera.main.transform.position, transform.up);
                loopAnim = false;
            }
            yield return new WaitForSeconds(.06f);
        }
    }

    /// <summary>
    /// run animation sent through string
    /// </summary>
    public void TriggerAnimation (string selectedAnimation) {
        anim.Play(selectedAnimation);

    }
}
