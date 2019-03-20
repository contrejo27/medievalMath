using UnityEngine;
using System.Collections;

public class MSP_GunBehaviour_b : MonoBehaviour 
{
	public GameObject bullitPrefab;
	public AudioClip gunSound;
	private AudioSource gunAudio;
	private float bullitSpeed = 100f;
	private float bullitsPerSecond = 5f;
	public Transform leftBarrelSpawnPoint;
	public Transform rightBarrelSpawnPoint;
	private float timeOfPreviousShot = 0f;

	//================================================================================

	void Start() 
	{
		// find the AudioSource component, for later use
		gunAudio = gameObject.GetComponent<AudioSource>() as AudioSource;
	}

	//================================================================================

	void Update()
	{
		if (MSP_Input.VirtualButton.GetButton("FireButton")) 
		{
			Fire();
		}
	}

	//================================================================================

	void Fire() 
	{
		if (Time.time > timeOfPreviousShot+(1f/bullitsPerSecond)) 
		{
			timeOfPreviousShot = Time.time;
			// Instantiate a bullit at the left and right barrel's spawnpoints
			GameObject bullitLeft = Instantiate(bullitPrefab,leftBarrelSpawnPoint.position, leftBarrelSpawnPoint.rotation) as GameObject;
			bullitLeft.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f,0f,bullitSpeed), ForceMode.VelocityChange);
			GameObject bullitRight = Instantiate(bullitPrefab,rightBarrelSpawnPoint.position, rightBarrelSpawnPoint.rotation) as GameObject;
			bullitRight.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f,0f,bullitSpeed), ForceMode.VelocityChange);
			// and play some audio
			gunAudio.PlayOneShot(gunSound); 
		}
	}
}
