using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunArrow : ArrowClass {

	private ProjectileBehavior ArrowBehavior;
	private Transform Target;
	bool activated = false;
	// Use this for initialization
	void Start () {

	}
	
	public void activate(bool isActivated){
		activated = isActivated;
	}
	
	public override void ArrowLaunched()
	{
		if(activated){
		GameObject Arrow = this.gameObject;
		DelayCreate(Arrow);
		}
	}
	void DelayCreate(GameObject Arrow)
	{
		GameObject[] spreadArrows = new GameObject[3];
		
		//shoots an arrow toward a random direction
		for (int i = 0; i < 3; i++) {
			spreadArrows[i] = Instantiate (Arrow, this.transform.position , this.transform.rotation);
			spreadArrows[i].transform.Rotate(Random.Range(-4.0f, 4.0f), Random.Range(-8.0f, -6.0f), Random.Range(0.0f, 0.0f));
			spreadArrows[i].GetComponent<ProjectileBehavior> ().isGrounded = false;
			spreadArrows[i].GetComponent<Rigidbody> ().AddForce (spreadArrows[i].transform.forward * 5000);
			spreadArrows[i].GetComponent<BoxCollider> ().enabled = true; 
			Destroy(spreadArrows[i],1.2f);
		}
	}
}
