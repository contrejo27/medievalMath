using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MSP_Input 
{
	public class GyroAccel : MonoBehaviour 
	{
		public bool forceAccelerometer = false;
		public float smoothingTime = 0.1f;
		public float headingOffset;
		
		public float pitchOffset = 30.0f;
		public float pitchOffsetMinimum = -70f;
		public float pitchOffsetMaximum = 70f;
		
		public float gyroHeadingAmplifier = 1f;
		public float gyroPitchAmplifier = 1f;
		//
		[HideInInspector]
		private Quaternion rotation = Quaternion.identity;
		[HideInInspector]
		public float heading;
		[HideInInspector]
		public float pitch;
		[HideInInspector]
		public float roll;
		
		[System.Serializable]
		public class AutoUpdateList 
		{
			public Transform targetTransform;
			public bool copyHeading = true;
			public bool copyPitch = true;
			public bool copyRoll = true;
			public bool enabled = true;
			public float headingMin = -180f;
			public float headingMax =  180f;
			public float headingDefault = 0f;
			public float pitchMin = -90f;
			public float pitchMax =  90f;
			public float pitchDefault = 0f;
			public float rollMin = -180f;
			public float rollMax =  180f;
			public float rollDefault = 0f;
			public float smoothingTime = 0f;
			public bool pushEdge = false;
			public float h, p, r = 0f;
			public Quaternion rotOld = Quaternion.identity;
			public Quaternion rotNew = Quaternion.identity;
		}
		
		public AutoUpdateList selfUpdate;
		public List<AutoUpdateList> autoUpdateList = new List<AutoUpdateList>();	
		
		public enum SimulationMode {None, Mouse};
		[System.Serializable]
		public class EditorSimulation 
		{
			public SimulationMode simulationMode = SimulationMode.None;
			public Vector2 mouseSenisitivity = new Vector2(1f,1f);
			public bool invertMouseY = false;
		}
		public EditorSimulation editorSimulation;
		
		// STATIC VARIABLES:
		static private bool _forceAccelerometer;
		static private float _smoothingTime;
		static private float _headingOffset;
		static private float _pitchOffset;
		static private float _pitchOffsetMinimum;
		static private float _pitchOffsetMaximum;
		static private float _gyroHeadingAmplifier;
		static private float _gyroPitchAmplifier;
		//
		static private Quaternion _rotation = Quaternion.identity;
		static private float _heading;
		static private float _pitch;
		static private float _roll;
		//
		static private string _transformName;
		static private AutoUpdateList _selfUpdate;
		static private List<AutoUpdateList> _autoUpdateList = new List<AutoUpdateList>();
        //
        static public float _cameraHeadingOffset;

		//================================================================================
		
		void Awake() 
		{
			Input.compensateSensors = true;
			Input.gyro.enabled = true;
			_forceAccelerometer = forceAccelerometer;
			_smoothingTime = smoothingTime;
			_headingOffset = headingOffset + _cameraHeadingOffset;
			_pitchOffset = pitchOffset;
			_pitchOffsetMinimum = pitchOffsetMinimum;
			_pitchOffsetMaximum = pitchOffsetMaximum;
			_gyroHeadingAmplifier = gyroHeadingAmplifier;
			_gyroPitchAmplifier = gyroPitchAmplifier;
			_selfUpdate = selfUpdate;
			_autoUpdateList.Clear();
			_autoUpdateList = autoUpdateList;
			_transformName = transform.name;
		}

		//================================================================================
		
		void Update() 
		{
			if (!SystemInfo.supportsGyroscope) 
			{
				forceAccelerometer = true;
				ErrorHandling.LogError("Warning [GyroAccel]: No gyroscope available -> forcing accelerometer");
			}
			if (Application.isEditor && Input.gyro.attitude == Quaternion.identity) 
			{
				MSP_Input.ErrorHandling.LogError("Warning [GyroAccel]: There seems to be a problem reading the gyroscope: did you set up Unity Remote correctly?");
			}
			//
			if (Application.isEditor && editorSimulation.simulationMode == SimulationMode.Mouse)
			{
				AddFloatToHeadingOffset(Input.GetAxis("Mouse X")*editorSimulation.mouseSenisitivity.x*Time.deltaTime);
				if (!editorSimulation.invertMouseY)
				{
					AddFloatToPitchOffset(Input.GetAxis("Mouse Y")*editorSimulation.mouseSenisitivity.y*Time.deltaTime);
				} else {
					AddFloatToPitchOffset(-Input.GetAxis("Mouse Y")*editorSimulation.mouseSenisitivity.y*Time.deltaTime);					
				}
				CheckPitchOffsetBoundaries();
			}
			//
			forceAccelerometer = _forceAccelerometer;
			smoothingTime = _smoothingTime;
			headingOffset = _headingOffset + _cameraHeadingOffset;
			pitchOffset = _pitchOffset;
			pitchOffsetMinimum = _pitchOffsetMinimum;
			pitchOffsetMaximum = _pitchOffsetMaximum;
			gyroHeadingAmplifier = _gyroHeadingAmplifier;
			gyroPitchAmplifier = _gyroPitchAmplifier;
			selfUpdate = _selfUpdate;
			autoUpdateList = _autoUpdateList;
			//
			if (!_forceAccelerometer && SystemInfo.supportsGyroscope) {
				UpdateGyroscopeOrientation ();
			} else {
				UpdateAccelerometerOrientation ();
			}
			//
			_rotation = rotation;
			_heading = heading;
			_pitch = pitch;
			_roll = roll;
			_headingOffset = headingOffset + _cameraHeadingOffset;
			_pitchOffset = pitchOffset;
			//
			AutoUpdate ();
		}

		//================================================================================

		void UpdateGyroscopeOrientation() 
		{
			Input.gyro.enabled = true;
			Quaternion gyroQuat = GetDeviceOrientationFromGyroscope();
			//
			float devicePitch;
			devicePitch = 90f - Vector3.Angle(Vector3.down,gyroQuat*Vector3.forward);
			//
			Vector3 gravityProjectedOnXYplane = Vector3.ProjectOnPlane(Input.gyro.gravity,Vector3.forward);
			float deviceRoll = Vector3.Angle (gravityProjectedOnXYplane, Vector3.down) * Mathf.Sign (Vector3.Cross (gravityProjectedOnXYplane, Vector3.down).z);
			AnimationCurve rollAdjustmentCurve = new AnimationCurve (new Keyframe (-90f, 0f), new Keyframe (-80f, 1f), new Keyframe (80f, 1f), new Keyframe (90f, 0f));
			deviceRoll *= rollAdjustmentCurve.Evaluate (devicePitch);
			//
			float rcosin = Mathf.Cos(Mathf.Deg2Rad * deviceRoll);
			float rsinus = Mathf.Sin(Mathf.Deg2Rad * deviceRoll);
			//
			float deltaHeading;
			deltaHeading = (-Input.gyro.rotationRateUnbiased.x * rsinus - Input.gyro.rotationRateUnbiased.y * rcosin);
			gyroHeadingAmplifier = Mathf.Clamp(gyroHeadingAmplifier,0.1f,4f);
			deltaHeading *= (gyroHeadingAmplifier-1f);
			headingOffset += deltaHeading;
			//
			float deltaPitch;
			deltaPitch = (-Input.gyro.rotationRateUnbiased.y * rsinus + Input.gyro.rotationRateUnbiased.x * rcosin);
			gyroPitchAmplifier = Mathf.Clamp(gyroPitchAmplifier,0.1f,4f);
			deltaPitch *= (gyroPitchAmplifier-1f);
			if (devicePitch > pitchOffsetMinimum && devicePitch < pitchOffsetMaximum) 
			{
				pitchOffset += deltaPitch;
			}
			if (devicePitch <= pitchOffsetMinimum) 
			{
				pitchOffset -= Mathf.Abs(deltaPitch);
			}
			if (devicePitch >= pitchOffsetMaximum) 
			{
				pitchOffset += Mathf.Abs(deltaPitch);
			}
			//
			CheckPitchOffsetBoundaries();
			// PITCH OFFSET:
			Vector3 gyro_forward = gyroQuat * Vector3.forward;
			Vector3 rotAxis = Vector3.Cross(Vector3.up,gyro_forward);
			AnimationCurve devicePitchAdjustmentCurve = new AnimationCurve(new Keyframe(-90f, 0f), new Keyframe(pitchOffset, -pitchOffset), new Keyframe(90f, 0f));
			Quaternion extra_pitch = Quaternion.AngleAxis(devicePitchAdjustmentCurve.Evaluate(devicePitch),rotAxis);
			gyroQuat = extra_pitch * gyroQuat;
			// HEADING OFFSET:
			Quaternion extra_heading = Quaternion.AngleAxis(headingOffset,Vector3.up);
			gyroQuat = extra_heading * gyroQuat;
			// Smooth gyro quaternion
			float smoothFactor = (smoothingTime > Time.deltaTime) ? Time.deltaTime / smoothingTime : 1f;
			rotation = Quaternion.Slerp(rotation, gyroQuat, smoothFactor);
			// Compute heading, pitch, roll
			Vector3 rf = rotation * Vector3.forward;
			Vector3 prf = Vector3.Cross(Vector3.up,Vector3.Cross(rf,Vector3.up));
			float newHeading = Vector3.Angle(Vector3.forward,prf) * Mathf.Sign (rf.x);
			AnimationCurve headingSmoothCurve = new AnimationCurve(new Keyframe(-90f, 0f, 0f, 0f), 
			                                                       new Keyframe(-85, smoothFactor, 0f, 0f),
			                                                       new Keyframe(85f, smoothFactor, 0f, 0f),
			                                                       new Keyframe(90f,0f,0f,0f));
			heading = Mathf.LerpAngle(heading,newHeading,headingSmoothCurve.Evaluate(pitch));
			pitch = Mathf.LerpAngle(pitch,devicePitch+devicePitchAdjustmentCurve.Evaluate(devicePitch),smoothFactor);
			roll = Mathf.LerpAngle(roll,deviceRoll,smoothFactor);
		} // void UpdateGyroscopeOrientation()
		
		//================================================================================
		
		void UpdateAccelerometerOrientation() 
		{
			float devicePitch;
			float deviceRoll;
			GetDevicePitchAndRollFromGravityVector(out devicePitch, out deviceRoll);
			//
			AnimationCurve devicePitchAdjustmentCurve = new AnimationCurve(new Keyframe(-90f, 0f), new Keyframe(pitchOffset, -pitchOffset), new Keyframe(90f, 0f));
			Quaternion accelQuat = Quaternion.identity;
			accelQuat = GetQuaternionFromHeadingPitchRoll(headingOffset, devicePitch+devicePitchAdjustmentCurve.Evaluate(devicePitch), deviceRoll);
			// Smooth gyro quaternion
			float smoothFactor = (smoothingTime > Time.deltaTime) ? Time.deltaTime / smoothingTime : 1f;
			rotation = Quaternion.Slerp(rotation, accelQuat, smoothFactor);
			// Compute heading, pitch, roll
			heading = Mathf.LerpAngle(heading,headingOffset,smoothFactor);
			pitch = Mathf.LerpAngle(pitch,devicePitch+devicePitchAdjustmentCurve.Evaluate(devicePitch),smoothFactor);
			roll = Mathf.LerpAngle(roll,deviceRoll,smoothFactor);
		} // void UpdateAccelerometerOrientation()
		
		//================================================================================
		
		void AutoUpdate()
		{
			bool atEdge = false;

			if (selfUpdate.enabled)
			{
				if (selfUpdate.enabled) 
				{
					if (!selfUpdate.copyHeading)
					{
						selfUpdate.h = selfUpdate.headingDefault;
					} else {
						if (selfUpdate.headingMin == -180f && selfUpdate.headingMax == 180f)
						{
							selfUpdate.h = GetHeading();
						} else {
							if (selfUpdate.pushEdge && (GetHeadingUnclamped() < selfUpdate.headingMin || GetHeadingUnclamped() > selfUpdate.headingMax))
							{
								atEdge = true;
								selfUpdate.h = Mathf.Clamp(GetHeadingUnclamped(),selfUpdate.headingMin,selfUpdate.headingMax);
								SetHeading (selfUpdate.h);
							} else {
								selfUpdate.h = Mathf.Clamp(GetHeadingUnclamped(),selfUpdate.headingMin,selfUpdate.headingMax);
							}
						}
					}
					//
					if (!selfUpdate.copyPitch)
					{
						selfUpdate.p = selfUpdate.pitchDefault;
					} else {
						if (selfUpdate.pitchMin == -90f && selfUpdate.pitchMax == 90f)
						{
							selfUpdate.p = GetPitch();
						} else {
							if (selfUpdate.pushEdge && (GetPitch() < selfUpdate.headingMin || GetPitch() > selfUpdate.headingMax))
							{
								atEdge = true;
								selfUpdate.p = Mathf.Clamp(GetPitch(),selfUpdate.pitchMin,selfUpdate.pitchMax);
								SetPitch (selfUpdate.p);
							} else {
								selfUpdate.h = Mathf.Clamp(GetPitch(),selfUpdate.pitchMin,selfUpdate.pitchMax);
							}
						}
					}
					//
					if (!selfUpdate.copyRoll)
					{
						selfUpdate.r = selfUpdate.rollDefault;
					} else {
						if (selfUpdate.rollMin == -180f && selfUpdate.rollMax == 180f)
						{
							selfUpdate.r = GetRoll();
						} else {
							selfUpdate.r = GetRollUnclamped();
							selfUpdate.r = Mathf.Clamp(selfUpdate.r,selfUpdate.rollMin,selfUpdate.rollMax);
						}
					}
					//
					if (selfUpdate.copyHeading && selfUpdate.copyPitch && selfUpdate.copyRoll 
					    && selfUpdate.headingMin == -180f && selfUpdate.headingMax == 180f 
					    && selfUpdate.pitchMin == -90f && selfUpdate.pitchMax == 90f 
					    && selfUpdate.rollMin == -180f && selfUpdate.rollMax == 180f
					    && !atEdge)
					{
						selfUpdate.rotNew = GetRotation(); 
					} else {
						selfUpdate.rotNew = MSP_Input.GyroAccel.GetQuaternionFromHeadingPitchRoll (selfUpdate.h, selfUpdate.p, selfUpdate.r);
					}
				}
			}
			//
			//
			//
			foreach (AutoUpdateList aut in autoUpdateList) 
			{
				if (aut.targetTransform && aut.enabled) 
				{
					if (!aut.copyHeading)
					{
						aut.h = aut.headingDefault;
					} else {
						if (aut.headingMin == -180f && aut.headingMax == 180f)
						{
							aut.h = GetHeading();
						} else {
							if (aut.pushEdge && (GetHeadingUnclamped() < aut.headingMin || GetHeadingUnclamped() > aut.headingMax))
							{
								atEdge = true;
								aut.h = Mathf.Clamp(GetHeadingUnclamped(),aut.headingMin,aut.headingMax);
								SetHeading (aut.h);
							} else {
								aut.h = Mathf.Clamp(GetHeadingUnclamped(),aut.headingMin,aut.headingMax);
							}
						}
					}
					//
					if (!aut.copyPitch)
					{
						aut.p = aut.pitchDefault;
					} else {
						if (aut.pitchMin == -90f && aut.pitchMax == 90f)
						{
							aut.p = GetPitch();
						} else {
							if (aut.pushEdge && (GetPitch() < aut.headingMin || GetPitch() > aut.headingMax))
							{
								atEdge = true;
								aut.p = Mathf.Clamp(GetPitch(),aut.pitchMin,aut.pitchMax);
								SetPitch (aut.p);
							} else {
								aut.p = Mathf.Clamp(GetPitch(),aut.pitchMin,aut.pitchMax);
							}
						}
					}
					//
					if (!aut.copyRoll)
					{
						aut.r = aut.rollDefault;
					} else {
						if (aut.rollMin == -180f && aut.rollMax == 180f)
						{
							aut.r = GetRoll();
						} else {
							aut.r = GetRollUnclamped();
							aut.r = Mathf.Clamp(aut.r,aut.rollMin,aut.rollMax);
						}
					}
					//
					if (aut.copyHeading && aut.copyPitch && aut.copyRoll 
					    && aut.headingMin == -180f && aut.headingMax == 180f 
					    && aut.pitchMin == -90f && aut.pitchMax == 90f 
					    && aut.rollMin == -180f && aut.rollMax == 180f
					    && !atEdge)
					{
						aut.rotNew = GetRotation(); 
					} else {
						aut.rotNew = MSP_Input.GyroAccel.GetQuaternionFromHeadingPitchRoll (aut.h, aut.p, aut.r);
					}
				}
			}
			//
			//
			//
			float smoothFactor;
			if (selfUpdate.enabled) 
			{
				smoothFactor = (selfUpdate.smoothingTime > Time.deltaTime) ? Time.deltaTime / selfUpdate.smoothingTime : 1f;
				selfUpdate.rotOld = transform.rotation;
				transform.rotation = Quaternion.Lerp (selfUpdate.rotOld, selfUpdate.rotNew, smoothFactor);
			}
			//
			foreach (AutoUpdateList aut in autoUpdateList) 
			{
				if (aut.targetTransform && aut.enabled) 
				{
					aut.rotOld = aut.targetTransform.rotation;
				}
			}
			//
			foreach (AutoUpdateList aut in autoUpdateList) 
			{
				if (aut.targetTransform && aut.enabled) 
				{
					smoothFactor = (aut.smoothingTime > Time.deltaTime) ? Time.deltaTime / aut.smoothingTime : 1f;
					aut.targetTransform.rotation = Quaternion.Lerp (aut.rotOld, aut.rotNew, smoothFactor);
				}
			}
		} // void AutoUpdate()
		
		//================================================================================

		private Quaternion GetDeviceOrientationFromGyroscope()
		{
			Quaternion gyroRotFix = new Quaternion(-0.5f,0.5f,0.5f,0.5f);
			Quaternion gyroRot = gyroRotFix * Input.gyro.attitude;
			gyroRot = new Quaternion(gyroRot.x, gyroRot.y, -gyroRot.z, -gyroRot.w);
			return gyroRot;
		}
		
		//================================================================================
		
		static public void GetDevicePitchAndRollFromGravityVector(out float devicePitch, out float deviceRoll) 
		{
			if (!SystemInfo.supportsGyroscope && !SystemInfo.supportsAccelerometer) 
			{
				devicePitch = 0f;
				deviceRoll = 0f;
			} else {
				// Vector holding the direction of gravity
				Vector3 gravity = Input.acceleration;
				// find the projections of the gravity vector on the YZ-plane
				Vector3 gravityProjectedOnXYplane = Vector3.Cross (Vector3.forward, Vector3.Cross (gravity, Vector3.forward));
				// calculate the pitch = rotation around x-axis ("dive forward/backward")
				devicePitch = Vector3.Angle (gravity, Vector3.forward) - 90;
				// calculate the roll = rotation around z-axis ("steer left/right")
				deviceRoll = Vector3.Angle (gravityProjectedOnXYplane, -Vector3.up) * Mathf.Sign (Vector3.Cross (gravityProjectedOnXYplane, Vector3.down).z);
				AnimationCurve rollAdjustmentCurve = new AnimationCurve (new Keyframe (-90f, 0f), new Keyframe (-80f, 1f), new Keyframe (80f, 1f), new Keyframe (90f, 0f));
				deviceRoll *= rollAdjustmentCurve.Evaluate (devicePitch);
			}
		} // static public void GetDevicePitchAndRollFromGravityVector(out float devicePitch, out float deviceRoll)
		
		//================================================================================
		
		void CheckPitchOffsetBoundaries() 
		{
			if (pitchOffset < pitchOffsetMinimum) 
			{
				pitchOffset = pitchOffsetMinimum;
			}
			if (pitchOffset > pitchOffsetMaximum) 
			{
				pitchOffset = pitchOffsetMaximum;
			}
		} // void CheckHeadingPitchRollBoundaries()
		
		//================================================================================
		
		static public Quaternion GetQuaternionFromHeadingPitchRoll(float inputHeading, float inputPitch, float inputRoll) 
		{
            
			Quaternion returnQuat = Quaternion.Euler(0f,inputHeading,0f) * Quaternion.Euler(inputPitch,0f,0f) * Quaternion.Euler(0f,0f,inputRoll);
			return returnQuat;
		} // static public Quaternion GetQuaternionFromHeadingPitchRoll(float inputHeading, float inputPitch, float inputRoll)

		//================================================================================

		static public float GetSignedAngleBetweenVectors(Vector3 from, Vector3 to, Vector3 normal){
			float angle = Vector3.Angle(from,to);
			float sign = Mathf.Sign(Vector3.Dot(normal,Vector3.Cross(from,to)));
			float signed_angle = angle * sign;
			//float angle360 =  (signed_angle + 360) % 360;
			return signed_angle;
		}

		//================================================================================
		// public Get functions:
		//================================================================================
		
		static public Quaternion GetRotation() 
		{
			return _rotation;
		}	
		
		//================================================================================
		
		static public float GetHeading() 
		{
			float h = _heading;
			while (h>180f) h-=360f;
			while (h<-180f) h+=360f;
			return h;
		}	

		//================================================================================
		
		static public float GetHeadingUnclamped() 
		{
			float h = _heading;
			return h;
		}	

		//================================================================================
		
		static public float GetPitch() 
		{
			return _pitch;
		}	
		
		//================================================================================
		
		static public float GetRoll() 
		{
			float r = _roll;
			while (r>180f) r-=360f;
			while (r<-180f) r+=360f;
			return r;
		}	

		//================================================================================
		
		static public float GetRollUnclamped() 
		{
			float r = _roll;
			return r;
		}	

		//================================================================================
		
		static public void GetHeadingPitchRoll(out float h, out float p, out float r) 
		{
			h = GetHeading();
			p = GetPitch();
			r = GetRoll();
		}	
		
		//================================================================================
		
		static public float GetHeadingOffset() 
		{
			return _headingOffset;
		}
		
		//================================================================================
		
		static public float GetPitchOffset() 
		{
			return _pitchOffset;
		}

		//================================================================================

		static public float GetPitchOffsetMinimum() 
		{
			return _pitchOffsetMinimum;
		}

		//================================================================================

		static public float GetPitchOffsetMaximum() 
		{
			return _pitchOffsetMaximum;
		}

		//================================================================================
		
		static public float GetSmoothingTime() 
		{
			return _smoothingTime;
		}
		
		//================================================================================
		
		static public float GetGyroHeadingAmplifier() 
		{
			return _gyroHeadingAmplifier;
		}
		
		//================================================================================
		
		static public float GetGyroPitchAmplifier() 
		{
			return _gyroPitchAmplifier;
		}
		
		//================================================================================
		
		static public bool GetForceAccelerometer() 
		{
			return _forceAccelerometer;
		}

		//================================================================================
		// public Set/Add functions:
		//================================================================================

		static public void SetHeading(float newHeading) 
		{
			while (newHeading < -180f) 
			{
				newHeading += 360f;
			}
			while (newHeading > 180f) 
			{
				newHeading -= 360f;
			}
			SetHeadingOffset(_headingOffset - _heading + newHeading);
			_heading = newHeading;
		}

		//================================================================================

		static public void SetHeadingOffset(float newHeadingOffset) 
		{
			_headingOffset = newHeadingOffset + _cameraHeadingOffset;
            
		}
		
		//================================================================================

		static public void SetPitch(float newPitch) 
		{
			newPitch = Mathf.Clamp (newPitch, -90f, 90f);
			SetPitchOffset (_pitchOffset + _pitch - newPitch);
			_pitch = newPitch;
		}

		//================================================================================

		static public void SetPitchOffset(float newPitchOffset) 
		{
			_pitchOffset = newPitchOffset;
		}
		
		//================================================================================
		
		static public void SetPitchOffsetMinumumMaximum(float newPitchOffsetMinimum, float newPitchOffsetMaximum) 
		{
			if (newPitchOffsetMinimum > newPitchOffsetMaximum) 
			{
				float tempValue = newPitchOffsetMinimum;
				newPitchOffsetMinimum = newPitchOffsetMaximum;
				newPitchOffsetMaximum = tempValue;
			}
			_pitchOffsetMinimum = Mathf.Clamp (newPitchOffsetMinimum, -90f, newPitchOffsetMaximum);
			_pitchOffsetMaximum = Mathf.Clamp (newPitchOffsetMaximum, newPitchOffsetMinimum, 90f);
		}
		
		//================================================================================
		
		static public void SetGyroHeadingAmplifier(float newValue) 
		{
			_gyroHeadingAmplifier = newValue;
		}
		
		//================================================================================
		
		static public void SetGyroPitchAmplifier(float newValue) 
		{
			_gyroPitchAmplifier = newValue;
		}
		
		//================================================================================
		
		static public void SetSmoothingTime(float smoothTime) 
		{
			_smoothingTime = smoothTime;
		}
		
		//================================================================================
		
		static public void SetForceAccelerometer(bool newValue) 
		{
			_forceAccelerometer = newValue;
		}
		
		//================================================================================
		
		static public void AddFloatToHeadingOffset(float extraHeadingOffset) 
		{
			_headingOffset += extraHeadingOffset;
		}
		
		//================================================================================
		
		static public void AddFloatToPitchOffset(float extraPitchOffset) 
		{
			_pitchOffset += extraPitchOffset;
			_pitchOffset = Mathf.Clamp (_pitchOffset, _pitchOffsetMinimum, _pitchOffsetMaximum);
		}

        //================================================================================

        static public void SetCameraHeadingOffset(float extraHeadingOffset)
        {
            _cameraHeadingOffset = extraHeadingOffset;
        }
		
		//================================================================================

		static public void EnableAutoUpdate() 
		{
			_selfUpdate.enabled = true;
			return;
		}

		//================================================================================
		
		static public void EnableAutoUpdate(string name) 
		{
			if (name == "self" || name == "Self" || name == _transformName)
			{
				_selfUpdate.enabled = true;
				return;
			}
			foreach (AutoUpdateList aut in _autoUpdateList) 
			{
				if (name == aut.targetTransform.name)
				{
					aut.enabled = true;
					return;
				}
			}
			MSP_Input.ErrorHandling.LogError("Warning [GyroAccel]: You are trying to enable AutoUpdate on object "+name+", but this object doesn't exist in the AutoUpdateList.");
			return;
		}

		//================================================================================

		static public void DisableAutoUpdate() 
		{
			_selfUpdate.enabled = false;
			return;
		}
		
		//================================================================================
		
		static public void DisableAutoUpdate(string name) 
		{
			if (name == "self" || name == "Self" || name == _transformName)
			{
				_selfUpdate.enabled = false;
				return;
			}
			foreach (AutoUpdateList aut in _autoUpdateList) 
			{
				if (name == aut.targetTransform.name)
				{
					aut.enabled = false;
					return;
				}
			}
			MSP_Input.ErrorHandling.LogError("Warning [GyroAccel]: You are trying to disable AutoUpdate on object "+name+", but this object doesn't exist in the AutoUpdateList.");
			return;
		}
		
		//================================================================================
	} // public class GyroAccel : MonoBehaviour
	
	} // namespace MSP_Input