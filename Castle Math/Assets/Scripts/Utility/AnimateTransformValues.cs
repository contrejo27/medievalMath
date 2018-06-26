using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnimateTransformValues : MonoBehaviour {
    [Header("Position Bools")]
    public bool animatePosX;
    public bool animatePosY;
    public bool animatePosZ;

    [Header("Rotation Bools")]
    public bool animateRotX;
    public bool animateRotY;
    public bool animateRotZ;

    [Header("Scale Bools")]
    public bool animateScaleX;
    public bool animateScaleY;
    public bool animateScaleZ;

    [Header("Position Values")]
    public float posX;
    public float posY;
    public float posZ;

    [Header("Rotation Values")]
    public float rotX;
    public float rotY;
    public float rotZ;

    [Header("Scale Values")]
    public float scaleX;
    public float scaleY;
    public float scaleZ;

    void Awake()
    {
        
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (animatePosX || animatePosY || animatePosZ)
        {
            transform.position =
                new Vector3
                (
                    animatePosX ? posX : transform.position.x,
                    animatePosY ? posY : transform.position.y,
                    animatePosZ ? posZ : transform.position.z
                );
        }

        if (animateRotX || animateRotY || animateRotZ)
        {
            transform.rotation = 
                Quaternion.Euler
                (
                    new Vector3
                    (
                        animateRotX ? rotX : transform.rotation.eulerAngles.x,
                        animateRotY ? rotY : transform.rotation.eulerAngles.y,
                        animateRotZ ? rotZ : transform.rotation.eulerAngles.z
                    )
                );
        }

        if (animateScaleX || animateScaleY || animateScaleZ)
        {
            transform.localScale =
                new Vector3
                (
                    animateScaleX ? scaleX : transform.localScale.x,
                    animateScaleY ? scaleY : transform.localScale.y,
                    animateScaleZ ? scaleZ : transform.localScale.z
                );
        }
    }
}

/*
[CustomEditor(typeof(AnimateTransformValues))]
public class AnimateTransformValuesEditor : Editor
{

    public override void OnInspectorGUI()
    {
        var animateTransformValues = target as AnimateTransformValues;


        base.OnInspectorGUI();
    }
}
*/