using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerBehavior : EnemyBehavior{

    public GameObject tigerPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



    IEnumerator RoarAndSpawn()
    {
        animator.Play("Roar");
        yield return new WaitForSeconds (3);
        if (!GetIsDead())
        {
            List<int> spawnPoints = new List<int>() { 1, 2, 3 };
            int i = Random.Range(0, spawnPoints.Count);
            GameStateManager.instance.levelManager.WaveManager.SpawnEnemy(tigerPrefab, null, spawnPoints[i]);
            spawnPoints.RemoveAt(i);
            i = Random.Range(0, spawnPoints.Count);
            GameStateManager.instance.levelManager.WaveManager.SpawnEnemy(tigerPrefab, null, spawnPoints[i]);
        }
    }
}
