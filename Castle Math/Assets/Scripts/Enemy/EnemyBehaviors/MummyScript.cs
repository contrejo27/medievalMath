using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyScript : EnemyBehavior {

    bool hasFirstDeath;
    int flaseDeathHPThreshold = 3;


    protected override void OnReceiveDamage()
    {
        base.OnReceiveDamage();
        if(hitPoints <= 3)
        {
            DieFirstDeath();
        }
    }

    void DieFirstDeath()
    {
        ignoreDamage = true;
        animator.Play("death");
        StartCoroutine(RiseFromDead());
    }

    IEnumerator RiseFromDead()
    {
        yield return new WaitForSeconds(4);
        // reverse death animation?
        // play get up animation?
        animator.Play("Special1");
        yield return new WaitForSeconds(1.08f);
        ignoreDamage = false;
    }
}
