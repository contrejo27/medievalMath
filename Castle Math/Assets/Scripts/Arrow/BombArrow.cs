using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombArrow : ArrowClass {
	public AudioClip boom;
	public GameObject explosion;
	bool activated = false;
	private AudioSource A_Source;
    public AudioClip powerUpSound;

    public void activate(bool isActivated){
		activated = isActivated;
	}

	public override void ArrowLaunched()
	{

	}
	
	void Start(){
		A_Source = GameObject.Find ("PlayerAudio").GetComponent<AudioSource> ();
        //print("playingSound");
       // A_Source.clip = powerUpSound;
        //A_Source.Play();

    }
	
	public override void ArrowImpact()
	{
		if(activated){
			A_Source.clip = boom;
			A_Source.Play ();
			var expl = Instantiate(explosion, transform.position, Quaternion.identity);
			//get all the colliders within a 10 radius
			Collider[] hitColliders = Physics.OverlapSphere (this.transform.position, 13);
			Destroy(expl, 3);
			int i = 0;
            List<EnemyBehavior> hitEBs = new List<EnemyBehavior>();
			while (i < hitColliders.Length) {
				if (hitColliders [i].transform.root.tag == "Enemy") {
                    GameObject root = hitColliders[i].transform.root.gameObject;
                    EnemyBehavior eb = root.GetComponent<EnemyBehavior>();
                    if(hitEBs.Contains(eb))
                    {
                        i += 1;
                        continue;
                    }
                    else
                    {
                        hitEBs.Add(eb);
                    }

                    eb.TakeDamage (2);
                    if (eb.hitPoints-2 <=0)
                    {
                        eb.hasBombDeath = true;
                        root.GetComponent<Rigidbody>().isKinematic = false;
                        root.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                        float upForceModifier = 7f;
                        root.GetComponent<Rigidbody>().AddExplosionForce(5000, this.transform.position, 15, upForceModifier);
                    }
					
				}
				i += 1;
			}
		}
	}


}
