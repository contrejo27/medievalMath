using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEEffectSpawner : MonoBehaviour {

    public float effectLifetime;
    // lifetime of individual lightning strikes, flmaes, etc.
    public float spawnedEffectLifetime;

    public GameObject[] effectToSpawn;

    public float spawnRadius;
    public float spawnFrequency;

	// Use this for initialization
	void Start () {
        Debug.Log("IS THIS HONKEY EVEN STARTING!?");
        StartCoroutine(SpawnEffects());
        Destroy(this, effectLifetime);

	}

    public void SetUp(float _effectLifetime, float _spawnedEffectLifetime, float _spawnRadius, float _spawnFrequency)
    {
        effectLifetime = _effectLifetime;
        spawnedEffectLifetime = _spawnedEffectLifetime;
        spawnRadius = _spawnRadius;
        spawnFrequency = _spawnFrequency;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator SpawnEffects()
    {
        Debug.Log("Starting lightning storm");
        while (true)
        {
            GameObject temp = Instantiate(effectToSpawn[Random.Range(0, effectToSpawn.Length)], transform.position, Quaternion.identity, transform.parent) as GameObject;

            float randomAngle = Random.Range(0, 2*Mathf.PI);
            float randomMagnitude = Random.Range(1, spawnRadius);
            temp.transform.position += new Vector3(Mathf.Cos(randomAngle)*randomAngle, 0, Mathf.Sin(randomAngle) * randomAngle);

            Destroy(temp, spawnedEffectLifetime);

            yield return new WaitForSeconds(spawnFrequency);
        }
    }

}
