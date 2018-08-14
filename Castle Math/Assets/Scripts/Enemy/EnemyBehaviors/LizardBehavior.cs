using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardBehavior : EnemyBehavior{

    bool isBurrowing;
    public float burrowingSwitchTime = 8;
    float burrowTimer = 0;
    public GameObject lizardMesh;
    public GameObject burrowEffect;

    public override void DoOnUpdate()
    {
        base.DoOnUpdate();
        if (navMeshAgent.remainingDistance > 10)
        {
            burrowTimer += Time.deltaTime;
            if (burrowTimer > burrowingSwitchTime)
            {
                if (isBurrowing)
                    EndBurrow();
                else
                    StartBurrow();
            }
        }
        else if (isBurrowing && navMeshAgent.remainingDistance < 10)
            EndBurrow();
    }

    void StartBurrow()
    {
        burrowTimer = 0;
        isBurrowing = true;
        StartCoroutine(SinkAndDisable());
    }

    void EndBurrow()
    {
        burrowTimer = 0;
        isBurrowing = false;
        StartCoroutine(EnableAndRise());
    }


    // Maybe also have the groudn effect rise/fall inverse to the lizard
    IEnumerator SinkAndDisable()
    {
        float initHeight = lizardMesh.transform.localPosition.y;
        float timer = 0;
        while(timer < 2)
        {
            timer += Time.deltaTime;
            lizardMesh.transform.localPosition = new Vector3(lizardMesh.transform.localPosition.x, Mathf.Lerp(initHeight, -4, timer), lizardMesh.transform.localPosition.z);
            yield return null;
        }
        lizardMesh.SetActive(false);
    }

    IEnumerator EnableAndRise()
    {
        lizardMesh.SetActive(true);
        float initHeight = lizardMesh.transform.localPosition.y;
        float timer = 0;
        while (timer < 2)
        {
            timer += Time.deltaTime;
            lizardMesh.transform.localPosition = new Vector3(lizardMesh.transform.localPosition.x, Mathf.Lerp(initHeight, 0, timer), lizardMesh.transform.localPosition.z);
            yield return null;
        }
    }
}
