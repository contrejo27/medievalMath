using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorFix : MonoBehaviour {

    public EnemyBehavior eb;

	public void DamageForward(int damage)
    {
        eb.DamageGate(damage);
    }

    public void HaltNavmeshAgent()
    {
        eb.HaltEnemyMovement();
    }

    public void ResumeNavmeshAgent()
    {
        eb.ResumeEnemyMovement();
    }
}
