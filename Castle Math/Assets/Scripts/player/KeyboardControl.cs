using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class KeyboardControl : MonoBehaviour {

    public enum ControlMethod { keyboard, mouse};
    public ControlMethod controls;

    // Mouse look reference http://wiki.unity3d.com/index.php/SmoothMouseLook
    // - James

    public float sensitivityX = 10;
    public float sensitivityY = 10;

    public float minX  = -360;
    public float maxX = 360;
    public float minY = -80;
    public float maxY = 80;

    float rotationX = 0;
    float rotationY = 0;

    private List<float> rotArrX = new List<float>();
    float rotAvgX = 0;

    private List<float> rotArrY = new List<float>();
    float rotAvgY = 0;

    public float frameCounter = 20;

    Quaternion originalRotation;
    
	// Use this for initialization
	void Start () {
        originalRotation = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {

        #region key controls
        if (controls == ControlMethod.keyboard)
        {
            //updates the rotation of the camera. 
            float CurrentY = this.transform.rotation.eulerAngles.y + Input.GetAxis("Horizontal");

            float CurrentX = this.transform.rotation.eulerAngles.x - Input.GetAxis("Vertical");

            this.transform.rotation = Quaternion.Euler(new Vector3(CurrentX, CurrentY, 0));
        }
        #endregion

        #region mouseControls
        else if (controls == ControlMethod.mouse)
        {
            rotAvgX = 0;
            rotAvgY = 0;

            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

            rotArrX.Add(rotationX);
            rotArrY.Add(rotationY);

            if (rotArrX.Count > frameCounter)
            {
                rotArrX.RemoveAt(0);
            }

            if (rotArrY.Count > frameCounter)
            {
                rotArrY.RemoveAt(0);
            }

            foreach(float f in rotArrX)
            {
                rotAvgX += f;
            }
            foreach (float f in rotArrY)
            {
                rotAvgY += f;
            }

            rotAvgX /= rotArrX.Count;
            rotAvgY /= rotArrY.Count;

            rotAvgX = ClampAngle(rotAvgX, minX, maxX);
            rotAvgY = ClampAngle(rotAvgY, minY, maxY);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotAvgX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotAvgY, Vector3.left);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;

        }
        #endregion

        
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        angle %= 360;
        if (angle < -360)
        {
            angle += 360;
        }
        else if (angle > 360)
        {
            angle -= 360;
        }

        return Mathf.Clamp(angle, min, max);
    }
}

// Just want to try this out; planning on showing different publics for keyboard vs mouse
// - James
/*
[CustomEditor(typeof(KeyboardControl))]
public class ControlsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

    }
}
*/