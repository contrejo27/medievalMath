using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour {

    public GameObject redGem;
    public GameObject yellowGem;
    public GameObject purpleGem;
    public GameObject cyanGem;
    public GameObject greenGem;

    GameObject gemToSpawn;
    Transform parentTransform;

    Dictionary<EnumManager.GemType, GameObject> gems;

    int numberOfGemsToSpawn; 

    void Awake()
    {
        gems = new Dictionary<EnumManager.GemType, GameObject>()
        {
            {EnumManager.GemType.Red, redGem },
            {EnumManager.GemType.Yellow, yellowGem },
             {EnumManager.GemType.Purple, purpleGem },
             {EnumManager.GemType.Cyan, cyanGem },
             {EnumManager.GemType.Green, greenGem }
        };
    }

	// Use this for initialization
	void Start () {
        
        
	}

    public void SetGemAndStartSpawn(EnumManager.GemType type, Transform parent, int numGems)
    {
        gemToSpawn = gems[type];
        numberOfGemsToSpawn = numGems;
        // Following this manually instead of subbing it to the object it
        // spawns on in case the parent despawns before gems finish spawning
        parentTransform = parent;
        StartCoroutine(SpawnGems());
    }
	
	// Update is called once per frame
	void Update () {
        if (parentTransform != null)
        {
            transform.position = parentTransform.position;
        }
	}

    IEnumerator SpawnGems()
    {
        for(int i = 0; i<numberOfGemsToSpawn; i++)
        {
            Instantiate(gemToSpawn, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(.3f);
        }
        Destroy(gameObject);
    }
}
