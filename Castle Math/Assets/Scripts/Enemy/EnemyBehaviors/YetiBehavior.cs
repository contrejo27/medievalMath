using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YetiBehavior : EnemyBehavior {

    int postHitSpeed = 14;

	bool hasEnraged = false;

	Renderer rend;

    public override void TakeDamage(int DMG)
    {
		if (hitPoints == 5 && !hasEnraged)
		{
			navMeshAgent.speed = 0;

			StartCoroutine (Angry ());
		}

		if (hitPoints == 1) {
			gameObject.transform.Find ("yeti").GetComponent<SkinnedMeshRenderer> ().material.color = Color.white;
		}

        base.TakeDamage(DMG);
    }
		
	IEnumerator Angry () {

		//navMeshAgent.speed = 0;

		Color lightred = new Color(1f, .3f, .3f, .3f);;

		animator.speed = 0.5f;

		animator.Play ("wound1");

		yield return new WaitForSeconds (.5f);

		//GetComponentInChildren<SkinnedMeshRenderer> ().material.color = Color.red;

		gameObject.transform.Find ("yeti").GetComponent<SkinnedMeshRenderer> ().material.color = lightred;

		animator.speed = 1.8f;

		navMeshAgent.speed = postHitSpeed;

		hasEnraged = true;
	}
}
