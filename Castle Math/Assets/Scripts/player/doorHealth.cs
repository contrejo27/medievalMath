using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorHealth : MonoBehaviour {

    public int baseHealth = 100;
    [HideInInspector]
	public int currentHealth;
    [HideInInspector]
    public int maxHealth;
	
	private GameStateManager GameManager;
	
	public GameObject fence1;
	public GameObject fence2;
	public GameObject fence3;
    public GameObject fence4;
	public GameObject post1;
	public GameObject post2;
	GameObject jiggleFence;
	
	bool firstFence = true;
	bool secondFence = true;
	bool thirdFence = true;
    bool fourthFence = true;
	bool invincible = false;
	bool gameLost = false;
	// Use this for initialization
	void Start () {
        if (SaveData.unlockedUpgrades[EnumManager.Upgrades.Barricade2])
            maxHealth = currentHealth = baseHealth + baseHealth/2;
        else if (SaveData.unlockedUpgrades[EnumManager.Upgrades.Barrricade1])
            maxHealth = currentHealth = baseHealth + baseHealth / 4;
        else
            maxHealth = currentHealth = baseHealth;
        
		jiggleFence = gameObject;
		GameManager = GameObject.FindObjectOfType<GameStateManager> ();
	}

	//Update health
	public void TakeDamageGate(int damage) {
		if(!invincible){
			Animator Anim = jiggleFence.GetComponent<Animator> ();
			Anim.Play("Jiggle");

			currentHealth -= damage;
            //Debug.Log("Current " + gameObject.name+ " Health: " + currentHealth);

			//hides fences when gate gets hit enough
			if((float)currentHealth/maxHealth < .75f && firstFence){
                //fence1.GetComponent<Renderer> ().enabled = false;
                fence1.GetComponent<Animator>().Play("Drop");
                //fence1.GetComponent<Animator>().enabled = false;
                //fence1.transform.parent = null;
				firstFence = false;
			}
			if((float)currentHealth/maxHealth < .5f && secondFence){
                //fence2.GetComponent<Renderer> ().enabled = false;
                fence2.GetComponent<Animator>().Play("Drop");
                //fence2.GetComponent<Animator>().enabled = false;
                //fence2.transform.parent = null;
				secondFence = false;
			}
            if((float)currentHealth/maxHealth < .25f && thirdFence)
            {
                //fence3.GetComponent<Renderer>().enabled = false;
                fence3.GetComponent<Animator>().Play("Drop");
                //fence3.GetComponent<Animator>().enabled = false;
                //fence3.transform.parent = null;
                thirdFence = false;
            }
			if(currentHealth <= 0 && fourthFence){
				fence4.GetComponent<Renderer> ().enabled = false;
				post1.GetComponent<Renderer> ().enabled = false;
				post2.GetComponent<Renderer> ().enabled = false;
				//post3.GetComponent<Renderer> ().enabled = false;

				fourthFence = false;
				//gameObject.GetComponent<Renderer> ().enabled = false;
				if(!gameLost) {
					GameManager.LoseState ();
					gameLost = true;
				}
			}
		}
	}
	
	public void UpdateHealth(int extraHealth){
			currentHealth += extraHealth;
			if(currentHealth>100) currentHealth = 100;

			//checks fences to see if we should add them back on
			if(currentHealth > 85 && !firstFence){
				fence1.GetComponent<Renderer> ().enabled = true;
				firstFence = true;
			}
			if(currentHealth > 40 && !secondFence){
				fence2.GetComponent<Renderer> ().enabled = true;
				secondFence = true;
			}

	}
	public void loseFences(){
		fence3.GetComponent<Renderer> ().enabled = false;
		post1.GetComponent<Renderer> ().enabled = false;
		post2.GetComponent<Renderer> ().enabled = false;
		//post3.GetComponent<Renderer> ().enabled = false;
		fence2.GetComponent<Renderer> ().enabled = false;
		fence1.GetComponent<Renderer> ().enabled = false;
		//fence2.GetComponent<BoxCollider> ().enabled = false;

		gameLost = true;
		currentHealth = 0;
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
		//post3.GetComponent<Renderer> ().material.color = new Color(.4f,.3f,.8f);
				
		fence1.GetComponent<Renderer>().sharedMaterial.color = new Color(.4f,.3f,.8f);
		StartCoroutine(InvincibleTimed(originalColor));
	}
	
	IEnumerator InvincibleTimed(Color ogColor)
	{
		yield return new WaitForSeconds (15f);
		fence1.GetComponent<Renderer> ().material.color = ogColor;
		fence2.GetComponent<Renderer> ().material.color = ogColor;
		fence3.GetComponent<Renderer> ().material.color = ogColor;
		post1.GetComponent<Renderer> ().material.color = ogColor;
		post2.GetComponent<Renderer> ().material.color = ogColor;
		//post3.GetComponent<Renderer> ().material.color = ogColor;
		
		invincible = false;
	}

    public float GetPercentHealth()
    {
        return (float)currentHealth / (float)maxHealth;
    }
}
