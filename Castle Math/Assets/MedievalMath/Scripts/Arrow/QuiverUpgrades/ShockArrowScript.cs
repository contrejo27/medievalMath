/*
 Electric Quiver Upgrades
 Added StunsEnemy within the EnemyBehaviorScript 
 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockArrowScript :ElementalArrow {
    /*Different Arrow Upgrades 
    Upgrade 1 is Static Shock - 2 damage, stuns for 2 seconds
    Upgrade 2A is Electric Storm - 3 damage, stuns for 2 seconds, AoE Radius 6
    Upgrade 2B is Lightning Burst - 5 damage, stuns for 5 seconds
    */
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
    public AudioClip electricStormHit;
    public AudioClip powerUpActivated;
    private AudioSource A_Source;

    //For Electric Storm Arrow Upgrade
    public GameObject electricBurst;
    bool activated = false;

	// Use this for initialization
	void Start () {
        if (SaveData.unlockedUpgrades[EnumManager.Upgrades.ShockArrows1])
        {
            currentUpgradeLevel = ElementalArrowModifier.UpgradeOne;
        }

        if (SaveData.currentLoadout[EnumManager.Loadout.ShockArrowsA])
        {
            currentUpgradeLevel = ElementalArrowModifier.UpgradeTwoA;
        }else if (SaveData.currentLoadout[EnumManager.Loadout.ShockArrowsB])
        {
            currentUpgradeLevel = ElementalArrowModifier.UpgradeTwoB;
        }

        quiver = (BaseQuiver)GameStateManager.instance.inventory.GetComponentInChildren<ShockQuiver>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void ArrowLaunched()
    {
        GameStateManager.instance.player.RemoveModifier(ArrowModifier.Shock);
        base.ArrowLaunched();
    }

    private void OnCollisionEnter(Collision otherCollision)
    {
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        GameObject root = otherCollision.transform.root.gameObject;
        switch (currentUpgradeLevel) {
            case ElementalArrowModifier.UpgradeOne:
                    /*  
                    Upgrade 1 Effects: Damages for 2 and Stuns for 2 seconds
                    */
                if (root.tag == "Enemy")
                {
                    //A_Source.clip = projectileHit;
                    //A_Source.Play();
                    EnemyBehavior EB = root.GetComponent<EnemyBehavior>();
                    EB.TakeDamage(upgradeOneDmg);
                    EB.StunsEnemy(upgradeOneStunTime);
                    EB.SetOnShock(upgradeOneStunTime);
                    this.transform.parent = otherCollision.transform;
                }
                break;
            case ElementalArrowModifier.UpgradeTwoA:
            /*
            Upgrade 2A Effects: Spawns an overlap sphere of radius 6 
            which Damages for 3 and Stuns for 2 seconds all enemies within range
            */
                Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 6);
                int enemiesCollided = 0;
                GameObject AoEEffect = Instantiate(Resources.Load("Conveyance/ShockAOE"), transform.position + new Vector3(0, 9, 0), Quaternion.identity) as GameObject;
                AOEEffectSpawner aoe = AoEEffect.GetComponent<AOEEffectSpawner>();
                aoe.SetUp(4, .7f, 6, .15f);
                //A_Source.clip = electricStormHit;
                //A_Source.Play();

                List<EnemyBehavior> affectedEBs = new List<EnemyBehavior>();
                while (enemiesCollided < hitColliders.Length)
                {
                    if (hitColliders[enemiesCollided].transform.root.tag == "Enemy")
                    {
                        EnemyBehavior EB = hitColliders[enemiesCollided].transform.root.GetComponent<EnemyBehavior>();
                        if (affectedEBs.Contains(EB))
                        {
                            enemiesCollided++;
                            continue;
                        }
                        affectedEBs.Add(EB);
                        EB.TakeDamage(upgradeTwoADmg);
                        EB.StunsEnemy(upgradeTwoAStunTime);
                        EB.SetOnShock(upgradeTwoAStunTime);
                    }
                    enemiesCollided++;
                }
                break;
            case ElementalArrowModifier.UpgradeTwoB:
                /*
                Upgrade 2B Effects: Damages for 5 and Stuns for 5
                */
                if (root.tag == "Enemy")
                {
                    //A_Source.clip = projectileHit;
                    //A_Source.Play();
                    EnemyBehavior EB = root.GetComponent<EnemyBehavior>();
                    EB.TakeDamage(upgradeTwoBDmg);
                    EB.StunsEnemy(upgradeTwoBStunTime);
                    EB.SetOnShock(upgradeTwoBStunTime);
                }
                break;
            default:
                break;
        }

        Destroy(this.gameObject, 1f);
    }
}


/*
*/