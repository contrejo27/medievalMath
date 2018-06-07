using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireQuiverScript : MonoBehaviour {
    /*
     Upgrade 1 is Ember Arrows - Dmg is 3
     Upgrade 2A is FireBomb Arrows - Dmg 3, AoE is 10
     Upgrade 2B is DragonsBreath - Dmg 7 (Good for immediately taking down dragons)
     */

    public enum FireArrowModifier { UpgradeOne,UpgradeTwoA,UpgradeTwoB}
    FireArrowModifier currentUpgradeLevel;

    [Header("Damages")]
    public int upgradeOneDmg = 3;
    public int upgradeTwoADmg = 3;
    public int upgradeTwoBDmg = 7;

    [Header("Fire Bomb Area Of Effect")]
    public float fireBombAreaOfEffect = 10;

    [Header("Audio")]
    public AudioClip projectileHit;
    public AudioClip fireBombHit;
    public AudioClip powerUpActivated;
    private AudioSource A_Source;

    //For FireBomb Upgrade 
    public GameObject fireBombEffect;
    bool activated = false;

	// Use this for initialization
	void Start () {
        A_Source = GameObject.Find("PlayerAudio").GetComponent<AudioSource>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision otherCollision)
    {
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        switch (currentUpgradeLevel)
        {
            case FireArrowModifier.UpgradeOne:
                /*
                 Upgrade 1 Effects: Damages for 3
                 */ 
                 if(otherCollision.transform.tag == "Enemy")
                {
                    A_Source.clip = projectileHit;
                    A_Source.Play();
                    EnemyBehavior EB = otherCollision.gameObject.GetComponent<EnemyBehavior>();
                    EB.TakeDamage(upgradeOneDmg);
                    this.transform.parent = otherCollision.transform;
                }
                break;
            case FireArrowModifier.UpgradeTwoA:
                /*
                 Upgrade 2A Effects: Spawns Overlap Sphere of radius 10
                 Which damages everyone for 3
                 */
                Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, fireBombAreaOfEffect);
                int enemiesCollided = 0;
                A_Source.clip = fireBombHit;
                A_Source.Play();

                GameObject explosion = Instantiate(fireBombEffect, transform.position, Quaternion.identity);
                Destroy(explosion, 3);

                while(enemiesCollided < hitColliders.Length)
                {
                    if(hitColliders[enemiesCollided].gameObject.tag == "Enemy")
                    {
                        EnemyBehavior EB = hitColliders[enemiesCollided].gameObject.GetComponent<EnemyBehavior>();
                        EB.TakeDamage(upgradeTwoADmg);

                        if(EB.hitPoints-2 <= 0)
                        {
                            EB.hasBombDeath = true;
                            hitColliders[enemiesCollided].gameObject.GetComponent<Rigidbody>().isKinematic = false;
                            hitColliders[enemiesCollided].gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                            float explosionForceModifier = 7f;
                            hitColliders[enemiesCollided].gameObject.GetComponent<Rigidbody>().AddExplosionForce(5000, this.transform.position, 15, explosionForceModifier);
                        }
                    }
                    enemiesCollided++;
                }
                break;
            case FireArrowModifier.UpgradeTwoB:
                /*
                Upgrade 2B Effects: Damages for 7 
                */
                if(otherCollision.transform.tag == "Enemy")
                {
                    A_Source.clip = projectileHit;
                    A_Source.Play();
                    EnemyBehavior EB = otherCollision.gameObject.GetComponent<EnemyBehavior>();
                    EB.TakeDamage(upgradeTwoBDmg);
                    this.transform.parent = otherCollision.transform;
                }
                break;
            default:
                break;
        }
        Destroy(this.gameObject, 1f);
    }

}
