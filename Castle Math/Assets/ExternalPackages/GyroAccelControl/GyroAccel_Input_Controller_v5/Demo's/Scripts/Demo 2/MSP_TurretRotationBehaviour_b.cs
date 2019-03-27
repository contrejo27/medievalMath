using UnityEngine;
using System.Collections;

public class MSP_TurretRotationBehaviour_b : MonoBehaviour 
{
	public Transform playerCamera;
	public Transform turret;
	public Transform gun;

	//================================================================================

	void Update()
	{
		// the turret base only copy's the gyro's heading
		float turretHeading = MSP_Input.GyroAccel.GetHeading();
		turret.rotation = MSP_Input.GyroAccel.GetQuaternionFromHeadingPitchRoll(turretHeading,0f,0f);

		// the guns copy heading and pitch from the gyro (not roll)
		float gunPitch = MSP_Input.GyroAccel.GetPitch();
		gunPitch = Mathf.Clamp(gunPitch,-70f,70f);
		gun.rotation = MSP_Input.GyroAccel.GetQuaternionFromHeadingPitchRoll(turretHeading,gunPitch,0f);

		// the camera uses the full gyro's rotation
		playerCamera.rotation = MSP_Input.GyroAccel.GetRotation();
		playerCamera.localPosition = new Vector3(0f,1.7f,-Mathf.Cos(Mathf.Deg2Rad*MSP_Input.GyroAccel.GetPitch()));
	}
}