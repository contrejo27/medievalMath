using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShieldController : MonoBehaviour {
    MeshRenderer mr;
    Material m;
    float t = 0;
    float ammount = 0.8f;
    public float fadeRate = 5f;
    float min = 0.8f;
    float max = 2.3f;
    float time = 0;

    Vector3 InitialScale = new Vector3(3.5f, 3.5f, 3.5f);
    Vector3 FinalScale = new Vector3(5f, 5f, 5f);
    public bool fadeIn  = false;
    public bool fadeOut = false;
    public bool setT    = false;
    public float sizescale = 10f;

    // Use this for initialization
    void Start () {
       // InitialScale = transform.localScale;
        mr = GetComponent<MeshRenderer>();
        m  = GetComponent<MeshRenderer>().sharedMaterial;
	}
	
	// Update is called once per frame
	void Update () {

        //testing calls
        /////////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.Q))
        {
            FadeIn();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            FadeOut();
        }
        ////////////////////////////////////////


        if (fadeIn && !fadeOut) {
            LerpFadeIn();
        }
        if (fadeOut && !fadeIn) {
            LerpFadeOut();
        }

            if (fadeIn) {
                transform.localScale = Vector3.Lerp(InitialScale, FinalScale, Time.deltaTime * sizescale);
            }
            if (fadeOut) {
                transform.localScale = Vector3.Lerp(InitialScale, FinalScale, Time.deltaTime * sizescale);
            }


    }

    public void FadeIn() {
        setT = true;
        fadeIn = true;
    }

    public void FadeOut() {
        setT = true;
        fadeOut = true;
    }

    private void LerpFadeOut()
    {
        if (setT) {
            t = 0;
            setT = false;
        }
        ammount = Mathf.Lerp(min, max, t);
        m.SetFloat("_RimPower", ammount);
        t += 0.5f * (fadeRate * Time.deltaTime);

        if(ammount >= 2.3f) {
            mr.enabled = false;
            fadeOut = false;
        }

    }

    private void LerpFadeIn() {
        if (mr.enabled == false) {
            mr.enabled = true;
        }
        if (setT) {
            t = 0;
            setT = false;
        }
        ammount = Mathf.Lerp(max, min, t);
        m.SetFloat("_RimPower", ammount);
        t += 0.5f * (fadeRate * Time.deltaTime);

        if (ammount <= 0.9f) {
            fadeIn = false;
        }
    }

    }
