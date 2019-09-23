using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour
{
    public void KillAll()
    {
        EnemyBehavior[] enemies = GameObject.FindObjectsOfType<EnemyBehavior>();
        foreach(EnemyBehavior enemy in enemies)
        {
            enemy.Killed();
        }
    }

}
