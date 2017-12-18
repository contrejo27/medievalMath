using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class doorHealth : MonoBehaviour {
	public Text doorHealthText;

	public int Health = 100;
	
	private GameStateManager GameManager;
	
	public GameObject[] fences1;
	public GameObject[] fences2;
	public GameObject[] fences3;
	
	bool firstFence = true;
	bool secondFence = true;
	bool thirdFence = true;
	bool invincible = false;
	
	// Use this for initialization
	void Start () {
			GameManager = GameObject.FindObjectOfType<GameStateManager> ();
	}


	//Update health
	public void TakeDamageGate(int damage) {
		if(!invincible){
			Health -= damage;

			//hides fences when gate gets hit enough
			if(Health < 85 && firstFence){
				foreach(GameObject fence in fences1){
					fence.GetComponent<Renderer> ().enabled = false;
				}
				firstFence = false;
			}
			if(Health < 40 && secondFence){
				foreach(GameObject fence in fences2){
					fence.GetComponent<Renderer> ().enabled = false;
				}
				secondFence = false;
			}
			if(Health < 0 && thirdFence){
				foreach(GameObject fence in fences3){
					fence.GetComponent<Renderer> ().enabled = false;
				}
				thirdFence = false;
				gameObject.GetComponent<Renderer> ().enabled = false;
				GameManager.LoseState ();
			}
		}
	}
	
	//set invincibility and change the fence color
	public void InvinciblePowerUp(){
		invincible = true;
		Color originalColor = fences1[0].GetComponent<Renderer>().material.color;
		foreach(GameObject fence in fences1){
			fence.GetComponent<Renderer>().material.color = new Color(.4f,.3f,.8f);
		}
		StartCoroutine(invincibleTimed(originalColor));
	}
	
	IEnumerator invincibleTimed(Color ogColor)
	{
		yield return new WaitForSeconds (15f);
		foreach(GameObject fence in fences1){
			fence.GetComponent<Renderer>().material.color = ogColor;
		}
		invincible = false;
	}
}
