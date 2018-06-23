using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour {

    GameObject gemToSpawn;
    int numberOfGemsToSpawn; 

	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnGems());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator SpawnGems()
    {
        for(int i = 0; i<numberOfGemsToSpawn; i++)
        {
            Instantiate(gemToSpawn, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(.2f);
        }
        Destroy(gameObject);
    }
}
