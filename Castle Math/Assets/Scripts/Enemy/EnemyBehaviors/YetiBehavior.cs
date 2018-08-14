using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YetiBehavior : EnemyBehavior {

    public int postHitSpeed = 14;

    public override void TakeDamage(int DMG)
    {
        base.TakeDamage(DMG);
        navMeshAgent.speed = postHitSpeed;
    }
}
