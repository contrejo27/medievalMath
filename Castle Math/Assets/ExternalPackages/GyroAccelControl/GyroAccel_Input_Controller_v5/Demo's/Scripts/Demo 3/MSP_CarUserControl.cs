using UnityEngine;
using System;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
using MSP_Input;

[RequireComponent(typeof (CarController))]
public class MSP_CarUserControl : MonoBehaviour
{
	[Serializable]
	public class ControlSettings
	{
		public string acceleratorButtonName = "";
		public string brakeButtonName = "";
		[HideInInspector]
		public CarController carController;
		[HideInInspector]
		public AnimationCurve steeringVsRollCurve = new AnimationCurve(
			new Keyframe(-180f,  0f), 
			new Keyframe( -90f, -1f),
			new Keyframe(   0f,  0f),
			new Keyframe(  90f,  1f),
			new Keyframe( 180f,  0f));

	}
	public ControlSettings controlSettings = new ControlSettings();

	[Serializable]
	public class CameraSettings
	{
		public Transform playerCamera;
		public Vector3 cameraOffset = new Vector3(0.0f,2.5f,0.0f);
		public AnimationCurve cameraDistanceVsPitchCurve = new AnimationCurve(
			new Keyframe(-90f, 0f), 
			new Keyframe(-30f, 0f),
			new Keyframe(  0f, 8f),
			new Keyframe( 90f, 0f));
		public float smoothingTime = 0.5f;
	}
	public CameraSettings cameraSettings = new CameraSettings();

	private float carHeading = 0f;

	//==========================================

	private void Awake()
	{
		// Get the car controller
		controlSettings.carController = gameObject.GetComponent<CarController>();

		// Find the car heading at startup
		Vector3 carDirection = transform.forward;
		Vector3 carFlatDirection = new Vector3(carDirection.x, 0f, carDirection.z);
		if (carFlatDirection.sqrMagnitude > 0f)
		{
			float carCurrentHeading = Vector3.Angle(Vector3.forward,carFlatDirection) * Mathf.Sign(carFlatDirection.x);
			carHeading = Mathf.LerpAngle(carHeading,carCurrentHeading,Time.deltaTime);
		} else {
			carHeading = 0f;
		}
	}
	
	//==========================================

	private void FixedUpdate()
	{
		CarControlUpdate();
	}

	//==========================================

	private void LateUpdate()
	{
		CameraPositionAndRotationUpdate();
	}

	//==========================================

	private void CarControlUpdate()
	{
		// Use the device's roll to compute the steering angle of the (front) wheels.
		float deviceRoll = GyroAccel.GetRoll();
		float steering = controlSettings.steeringVsRollCurve.Evaluate(-deviceRoll);
		
		// Get acceleration and braking from the VirtualButtons
		float accel = 0f;
		if (controlSettings.acceleratorButtonName == "" || controlSettings.acceleratorButtonName == "")
		{
			ErrorHandling.LogError("Warning [CarUSerControl]: " +
				"please specify the correct name of the VirtualButtons used for accelerating and/or braking.");
		} else {
			if (VirtualButton.GetButton(controlSettings.brakeButtonName))
			{
				accel -=1f;
			}
			if (VirtualButton.GetButton(controlSettings.acceleratorButtonName))
			{
				accel +=1f;
			}
		}

		// Send the steering and accel values to the carController
		controlSettings.carController.Move(steering, accel, accel, 0f);

	} // private void CarControlUpdate()

	//==========================================

	private void CameraPositionAndRotationUpdate()
	{
		// the camera heading will align with the car's heading
		Vector3 carDirection = transform.forward;
		Vector3 carFlatDirection = new Vector3(carDirection.x, 0f, carDirection.z);
		if (carFlatDirection.sqrMagnitude > 0f)
		{
			float carCurrentHeading = Vector3.Angle(Vector3.forward,carFlatDirection) * Mathf.Sign(carFlatDirection.x);
			float smoothFactor = 1f;
			if (cameraSettings.smoothingTime > Time.deltaTime)
			{
				smoothFactor = Time.deltaTime / cameraSettings.smoothingTime;
			}
			carHeading = Mathf.LerpAngle(carHeading,carCurrentHeading,smoothFactor);
		} 

		// Set the camera rotation
		float camHeading = carHeading;
		float camPitch = GyroAccel.GetPitch();
		float camRoll = GyroAccel.GetRoll();
		cameraSettings.playerCamera.rotation = GyroAccel.GetQuaternionFromHeadingPitchRoll(camHeading,camPitch,camRoll);

		// Set the camera position
		cameraSettings.playerCamera.position = transform.position + cameraSettings.cameraOffset - cameraSettings.cameraDistanceVsPitchCurve.Evaluate(camPitch)*cameraSettings.playerCamera.forward;
	} // private void CameraPositionAndRotationUpdate()

} // public class MSP_CarUserControl : MonoBehaviour