using UnityEngine;
using System.Collections;

public class MSP_PlayerCameraBehaviour_b : MonoBehaviour 
{
	private float fieldOfViewMin = 20f;
	private float fieldOfViewMax = 70f;
	private float fieldOfView = 70f;
	private Camera mainCamera;
	private AnimationCurve gyroAmplifierCurve; 

	//================================================================================

	void Start () 
	{
		mainCamera = gameObject.GetComponent<Camera>() as Camera;
		gyroAmplifierCurve = new AnimationCurve(new Keyframe(fieldOfViewMin, 0.1f), new Keyframe(fieldOfViewMax, 1.3f));
	}
	
	//================================================================================

	void Update () 
	{
		if (MSP_Input.VirtualButton.GetButtonDown("ZoomButton")) 
		{
			EnableCameraZoom();
		}
				
		if (MSP_Input.VirtualButton.GetButtonUp("ZoomButton")) 
		{
			DisableCameraZoom();
		}

		// smoothly lerp FOV towards target value
		mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView,fieldOfView, Time.deltaTime * 5f);

		// when zoomed in, the gyro movements will be slowed down, for more accurate aiming
		float gyroAmplifier = gyroAmplifierCurve.Evaluate(fieldOfView);
		MSP_Input.GyroAccel.SetGyroHeadingAmplifier(gyroAmplifier);
		MSP_Input.GyroAccel.SetGyroPitchAmplifier(gyroAmplifier);

		// Use the current value of the pitch to adjust the camera's position, relative to the turret 
		transform.localPosition = new Vector3(0f,1.7f,-Mathf.Cos(Mathf.Deg2Rad*MSP_Input.GyroAccel.GetPitch()));
	}

	//================================================================================

	void EnableCameraZoom() 
	{
		fieldOfView = fieldOfViewMin;
	}

	//================================================================================

	void DisableCameraZoom() 
	{
		fieldOfView = fieldOfViewMax;
	}

}