using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerBehavior : EnemyBehavior{

    public GameObject tigerPrefab;
    bool pausedForHit;
    bool firstHit = false;

    protected override void OnStartAttacking()
    {
        base.OnStartAttacking();

        if (!isTargetDummy && !firstHit && !isClone) 
        {
            firstHit = true;
            StartCoroutine(RoarAndSpawn());
        }
            
    }

    protected override void OnReceiveDamage()
    {
        base.OnReceiveDamage();/*
        if (!pausedForHit)
        {
            StartCoroutine(PauseMovement());
        }*/
    }

    IEnumerator PauseMovement()
    {
        pausedForHit = true;
        PauseEnemy();
        yield return new WaitForSeconds(1.5f);
        ResumeEnemy();
        pausedForHit = false;
    }

    IEnumerator RoarAndSpawn()
    {
        animator.Play("Roar");
        yield return new WaitForSeconds (3);
        if (!GetIsDead())
        {
            List<int> spawnPoints = new List<int>() { 0, 1, 2 };
            int i = Random.Range(0, spawnPoints.Count);

            //Debug.Log(spawnPoints[i] + " footsteps = " + footstepSound.name);
            GameStateManager.instance.waveManager.SpawnEnemy(tigerPrefab, footstepSound, spawnPoints[i], true);
            spawnPoints.RemoveAt(i);
            i = Random.Range(0, spawnPoints.Count);
            GameStateManager.instance.waveManager.SpawnEnemy(tigerPrefab, footstepSound, spawnPoints[i], true);
        }
    }
}
