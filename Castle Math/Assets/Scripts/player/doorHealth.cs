using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class doorHealth : MonoBehaviour {
	public Text doorHealthText;

	public int Health = 100;
	
	private GameStateManager GameManager;
	
	public GameObject fence1;
	public GameObject fence2;
	public GameObject fence3;
	public GameObject post1;
	public GameObject post2;
	public GameObject post3;

	bool firstFence = true;
	bool secondFence = true;
	bool thirdFence = true;
	bool invincible = false;
	
	// Use this for initialization
	void Start () {
			GameManager = GameObject.FindObjectOfType<GameStateManager> ();
	}

void Update(){doorHealthText.text=Health.ToString();}
	//Update health
	public void TakeDamageGate(int damage) {
		if(!invincible){
			Health -= damage;

			//hides fences when gate gets hit enough
			if(Health < 85 && firstFence){
				fence1.GetComponent<Renderer> ().enabled = false;
				firstFence = false;
			}
			if(Health < 40 && secondFence){
				fence2.GetComponent<Renderer> ().enabled = false;
				secondFence = false;
			}
			if(Health < 0 && thirdFence){
				fence3.GetComponent<Renderer> ().enabled = false;
				post1.GetComponent<Renderer> ().enabled = false;
				post2.GetComponent<Renderer> ().enabled = false;
				post3.GetComponent<Renderer> ().enabled = false;

				thirdFence = false;
				gameObject.GetComponent<Renderer> ().enabled = false;
				GameManager.LoseState ();

			}
		}
	}
	public void UpdateHealth(int extraHealth){
			Health += extraHealth;
			if(Health>100) Health = 100;

			//checks fences to see if we should add them back on
			if(Health > 85 && !firstFence){
				fence1.GetComponent<Renderer> ().enabled = true;
				firstFence = true;
			}
			if(Health > 40 && !secondFence){
				fence2.GetComponent<Renderer> ().enabled = true;
				secondFence = true;
			}

	}
	
	//set invincibility and change the fence color
	public void InvinciblePowerUp(){
		invincible = true;
		Color originalColor = fence1.GetComponent<Renderer>().material.color;

		fence1.GetComponent<Renderer> ().material.color = new Color(.4f,.3f,.8f);
		fence2.GetComponent<Renderer> ().material.color = new Color(.4f,.3f,.8f);
		fence3.GetComponent<Renderer> ().material.color = new Color(.4f,.3f,.8f);
		post1.GetComponent<Renderer> ().material.color = new Color(.4f,.3f,.8f);
		post2.GetComponent<Renderer> ().material.color = new Color(.4f,.3f,.8f);
		post3.GetComponent<Renderer> ().material.color = new Color(.4f,.3f,.8f);
				
		fence1.GetComponent<Renderer>().sharedMaterial.color = new Color(.4f,.3f,.8f);
		StartCoroutine(invincibleTimed(originalColor));
	}
	
	IEnumerator invincibleTimed(Color ogColor)
	{
		yield return new WaitForSeconds (15f);
		print("this happened");
		fence1.GetComponent<Renderer> ().material.color = ogColor;
		fence2.GetComponent<Renderer> ().material.color = ogColor;
		fence3.GetComponent<Renderer> ().material.color = ogColor;
		post1.GetComponent<Renderer> ().material.color = ogColor;
		post2.GetComponent<Renderer> ().material.color = ogColor;
		post3.GetComponent<Renderer> ().material.color = ogColor;
		
		invincible = false;
	}
}
