/*
 Electric Quiver Upgrades
 Added StunsEnemy within the EnemyBehaviorScript 
 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricQuiverUpgrades : MonoBehaviour {
    /*Different Arrow Upgrades 
    Upgrade 1 is Static Shock - 2 damage, stuns for 2 seconds
    Upgrade 2A is Electric Storm - 3 damage, stuns for 2 seconds, AoE Radius 6
    Upgrade 2B is Lightning Burst - 5 damage, stuns for 5 seconds
    */
    public enum ElectricArrowModifier {UpgradeOne, UpgradeTwoA, UpgradeTwoB}
    ElectricArrowModifier currentUpgradeLevel;

    [Header("Damages")]
    public int upgradeOneDmg = 2;
    public int upgradeTwoADmg = 3;
    public int upgradeTwoBDmg = 5;

    [Header("Stun Times")]
    public float upgradeOneStunTime = 2;
    public float upgradeTwoAStunTime = 2;
    public float upgradeTwoBStunTime = 5;

    [Header("Audio")]
    public AudioClip projectileHit;
    public AudioClip powerUpActivated;
    private AudioSource A_Source;

    //For Electric Storm Arrow Upgrade
    public GameObject electricBurst;
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

        switch (currentUpgradeLevel) {
            case ElectricArrowModifier.UpgradeOne:
                /*
                    Upgrade 1 Effects: Damages for 2 and Stuns for 2 seconds
                    */
                if (otherCollision.transform.tag == "Enemy")
                {
                    EnemyBehavior EB = otherCollision.gameObject.GetComponent<EnemyBehavior>();
                    EB.TakeDamage(upgradeOneDmg);
                    EB.StunsEnemy(upgradeOneStunTime);
                    this.transform.parent = otherCollision.transform;
                }
                break;
            case ElectricArrowModifier.UpgradeTwoA:
            /*
            Upgrade 2A Effects: Spawns an overlap sphere of radius 6 
            which Damages for 3 and Stuns for 2 seconds all enemies within range
            */
                Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 6);
                int enemiesCollided = 0;
                while (enemiesCollided < hitColliders.Length)
                {
                    if (hitColliders[enemiesCollided].gameObject.tag == "Enemy")
                    {
                        EnemyBehavior EB = hitColliders[enemiesCollided].gameObject.GetComponent<EnemyBehavior>();
                        EB.TakeDamage(upgradeTwoADmg);
                        EB.StunsEnemy(upgradeTwoAStunTime);
                    }
                    enemiesCollided++;
                }
                break;
            case ElectricArrowModifier.UpgradeTwoB:
                /*
                Upgrade 2B Effects: Damages for 5 and Stuns for 5
                */
                if (otherCollision.transform.tag == "Enemy")
                {
                    EnemyBehavior EB = otherCollision.gameObject.GetComponent<EnemyBehavior>();
                    EB.TakeDamage(upgradeTwoBDmg);
                    EB.StunsEnemy(upgradeTwoBStunTime);
                }
                break;
            default:
                break;
        }

        Destroy(this.gameObject, 1f);
    }
}


/*IEnumerator StunTimer(float stunTime)
    {

        //temp speeds
        float normalNavMeshSpeed = navMeshAgent.speed;
        float normalAnimSpeed = animator.speed;
        
        //Setting speeds to 0
        navMeshAgent.speed = 0f;
        animator.speed = 0;
        yield return new WaitForSeconds(stunTime);

        //returning the speeds to normal
        navMeshAgent.speed = normalNavMeshSpeed;
        animator.speed = normalAnimSpeed;
    }

        public void StunsEnemy(float timeStunned)
    {

        //starts coroutine which sets speeds to 0 for the TimeStunned Seconds
        StartCoroutine(StunTimer(timeStunned));
    }
*/