using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillAllEnemies : MonoBehaviour
{
    public Button startButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("By using the tester script: 'KillAllEnemies' from buttom 'M', you vanquish all enemies.");
            KillAll();
        } else if (Input.GetKeyDown(KeyCode.N))
        {
            if(!startButton.IsActive())
            {
                startButton.gameObject.SetActive(true);
            }
            startButton.onClick.Invoke();
            Debug.Log("By using the tester script: 'KillAllEnemies' from buttom 'N', you skip initial tutorial.");
        }
    }

    public void KillAll()
    {
        GameObject[] allEnemies;
        allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        for(int i = 0; i < allEnemies.Length; i++)
        {
            allEnemies[i].GetComponent<EnemyBehavior>().Killed();
        }
    }
}
