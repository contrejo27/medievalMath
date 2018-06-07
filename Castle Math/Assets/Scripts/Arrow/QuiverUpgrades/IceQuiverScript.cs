using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceQuiverScript : MonoBehaviour {
    /*Different Arrow Upgrades 
    Upgrade 1 is Icicle Arrows - 2 damage, slows by 3 for 4 seconds
    Upgrade 2A is Blizzard Arrows - 2 damage, slows by 4 for 6 seconds, AoE Radius 8
    Upgrade 2B is Frost Bite Arrows - 3 damage, slows by 6  for 8 seconds
    */
    public enum IceArrowModifier { UpgradeOne, UpgradeTwoA, UpgradeTwoB }
    IceArrowModifier currentUpgradeLevel;

    [Header("Damages")]
    public int upgradeOneDmg = 2;
    public int upgradeTwoADmg = 2;
    public int upgradeTwoBDmg = 3;

    [Header("Time of Slow Effect")]
    public float upgradeOneSlowTime = 4;
    public float upgradeTwoASlowTime = 6;
    public float upgradeTwoBSlowTime = 8;

    [Header("Slow Amounts")]
    public float upgradeOneSlowAmount = 3;
    public float upgradeTwoASlowAmount = 4;
    public float upgradeTwoBSlowAmount = 6;

    [Header("Blizzard AoE")]
    public float blizzardAreaOfEffect = 8;

    [Header("Audio")]
    public AudioClip projectileHit;
    public AudioClip blizzardHit;
    public AudioClip powerUpActivated;
    private AudioSource A_Source;

    //For Blizzard Arrow Upgrade
    public GameObject blizzardEffect;
    bool activated = false;

    // Use this for initialization
    void Start () {
		
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
            case IceArrowModifier.UpgradeOne:
            /*
            Upgrade 1 Effects: Damages for 2, Slows by 3, Lasts for 4 seconds
            */

                if (otherCollision.transform.tag == "Enemy")
                {
                    A_Source.clip = projectileHit;
                    A_Source.Play();
                    EnemyBehavior EB = otherCollision.gameObject.GetComponent<EnemyBehavior>();
                    EB.TakeDamage(upgradeOneDmg);
                    EB.SlowsEnemy(upgradeOneSlowAmount, upgradeOneSlowTime);
                    this.transform.parent = otherCollision.transform;
                }
                break;

            case IceArrowModifier.UpgradeTwoA:
                /*
                Upgrade 2A Effects: Spawns overlap Sphere of radius 8
                Which Damages for 2, Slows by 4, Lasts for 6 seconds
                */
                Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, blizzardAreaOfEffect);
                int enemiesCollided = 0;
                A_Source.clip = blizzardHit;
                A_Source.Play();
                while (enemiesCollided < hitColliders.Length)
                {
                    if(hitColliders[enemiesCollided].gameObject.tag == "Enemy")
                    {
                        EnemyBehavior EB = hitColliders[enemiesCollided].gameObject.GetComponent<EnemyBehavior>();
                        EB.TakeDamage(upgradeTwoADmg);
                        EB.SlowsEnemy(upgradeTwoASlowAmount, upgradeTwoASlowTime);
                    }
                    enemiesCollided++;
                }
                break;

            case IceArrowModifier.UpgradeTwoB:
                /*
                Upgrade 2B Effects: Damages for 3, Slows by 6, Lasts for 8
                */

                if(otherCollision.transform.tag == "Enemy")
                {
                    A_Source.clip = projectileHit;
                    A_Source.Play();
                    EnemyBehavior EB = otherCollision.gameObject.GetComponent<EnemyBehavior>();
                    EB.TakeDamage(upgradeTwoBDmg);
                    EB.SlowsEnemy(upgradeTwoBSlowAmount, upgradeTwoBSlowTime);
                    this.transform.parent = otherCollision.transform;
                }
                break;
            default:
                break;
        }
        Destroy(this.gameObject, 1f);
    }
}
