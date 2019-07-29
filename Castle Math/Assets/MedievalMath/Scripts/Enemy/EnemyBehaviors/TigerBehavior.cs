using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerBehavior : EnemyBehavior{

    public GameObject tigerPrefab;
    bool pausedForHit;
    //bool firstHit = false;
	bool canDodge = true;

	float cooldowntimer;
	float cooldowntime = 3f;

	float lerpTime = .07f;
	float currentlerp = 0f;

	private Vector3 velocity = Vector3.zero;

    protected override void OnStartAttacking()
    {
        base.OnStartAttacking();
        
        if (!isTargetDummy &&  !isClone) //!firstHit &&
        {
			canDodge = false;
            animator.SetBool("isAttacking", true);
            StartCoroutine(RoarAndSpawn());
        }
            
    }

	/*
    protected override void OnReceiveDamage()
    {
        base.OnReceiveDamage();
		
        if (!pausedForHit)
        {
            StartCoroutine(PauseMovement());
        }
    }
	*/

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == 8) {
			StartCoroutine(Dodge ());
		}
	}

	IEnumerator Dodge() {
		if (canDodge) {
			//dodge
			canDodge = false;

			Debug.Log (canDodge);

			float leftRight = Random.Range(0 , 2);

			Debug.Log (transform.position.x);

			if (leftRight == 0) {
				//move left
				while (currentlerp <= lerpTime) {
					currentlerp += Time.deltaTime;

					Debug.Log (transform.position.x);

					//transform.Translate (Vector3.left * 100 * Time.deltaTime);
					//transform.position = Vector3.Lerp(transform.position, transform.position + (Vector3.left * 100), Time.deltaTime);
					transform.localPosition = Vector3.Lerp (transform.localPosition, transform.localPosition - (transform.right * 1f), currentlerp / lerpTime);

					yield return null;
				}
			} 
			else if (leftRight == 1) {
				//move right
				while (currentlerp <= lerpTime) {
					currentlerp += Time.deltaTime;

					Debug.Log ("moving Right!");

					transform.localPosition = Vector3.Lerp (transform.localPosition, transform.localPosition + (transform.right * 1f), currentlerp / lerpTime);

					yield return null;
				}
			}


			StartCoroutine (Cooldown ());
			cooldowntimer = cooldowntime;

			yield return null;
		}
	}

	IEnumerator Cooldown()
	{
		while (cooldowntimer > 0) {
			cooldowntimer -= Time.deltaTime;
			//Debug.Log (cooldowntimer);
		}

		if (cooldowntimer < 0) {
			cooldowntimer = 0;
			canDodge = true;
		}

		//Debug.Log (cooldowntimer);

		currentlerp = 0f;
		yield return null;
	}


	/*
    IEnumerator PauseMovement()
    {
        pausedForHit = true;
        PauseEnemy();
        yield return new WaitForSeconds(1.5f);
        ResumeEnemy();
        pausedForHit = false;
    }
    */

    IEnumerator RoarAndSpawn()
    {
        animator.Play("Roar");
        yield return new WaitForSeconds (3);
       /* if (!GetIsDead())
        {
            List<int> spawnPoints = new List<int>() { 0, 1, 2 };
            int i = Random.Range(0, spawnPoints.Count);

            //Debug.Log(spawnPoints[i] + " footsteps = " + footstepSound.name);
            GameStateManager.instance.waveManager.SpawnEnemy(tigerPrefab, footstepSound, spawnPoints[i], true);
            spawnPoints.RemoveAt(i);
            i = Random.Range(0, spawnPoints.Count);
            GameStateManager.instance.waveManager.SpawnEnemy(tigerPrefab, footstepSound, spawnPoints[i], true);
        }*/   }
}
