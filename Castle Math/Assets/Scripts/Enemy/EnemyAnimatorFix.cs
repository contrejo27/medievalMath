using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorFix : MonoBehaviour {

    public EnemyBehavior eb;

	public void DamageForward(int damage)
    {
        eb.DamageGate(damage);
    }
}
