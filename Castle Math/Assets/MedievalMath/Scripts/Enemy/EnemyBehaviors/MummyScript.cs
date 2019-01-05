using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyScript : EnemyBehavior {

    bool hasFirstDeath;
    bool pausedForHit;
    int flaseDeathHPThreshold = 3;


    protected override void OnReceiveDamage()
    {
        base.OnReceiveDamage();
        if(hitPoints <= 3 && !hasFirstDeath)
        {
            DieFirstDeath();
        }else if (!pausedForHit)
        {
            StartCoroutine(PauseMovement());
        }
    }

    IEnumerator PauseMovement()
    {
        haltAttackAnimation = true;
        pausedForHit = true;
        PauseEnemy();
        yield return new WaitForSeconds(1.09f);
        ResumeEnemy();
        pausedForHit = false;
        haltAttackAnimation = false;
    }

    void DieFirstDeath()
    {
        hasFirstDeath = true;
        ignoreDamage = true;
        animator.Play("death");
        StartCoroutine(RiseFromDead());
    }

    IEnumerator RiseFromDead()
    {
        pausedForHit = true;
        navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(4);
        // reverse death animation?
        // play get up animation?
        animator.Play("Special1");
        yield return new WaitForSeconds(1.08f);
        ignoreDamage = false;

        pausedForHit = false;
        navMeshAgent.isStopped = false;
    }
}
