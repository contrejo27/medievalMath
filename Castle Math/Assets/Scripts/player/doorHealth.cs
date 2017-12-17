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

	// Use this for initialization
	void Start () {
			GameManager = GameObject.FindObjectOfType<GameStateManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Health<1) GameManager.LoseState ();

	}

	//Update health
	public void TakeDamageGate(int damage) {
		Health -= damage;

		if(Health < 85 && firstFence){
			foreach(GameObject fence in fences1){
				fence.GetComponent<Renderer> ().enabled = false;
			}
			firstFence = false;
		}
		if(Health < 60 && secondFence){
			foreach(GameObject fence in fences2){
				fence.GetComponent<Renderer> ().enabled = false;
			}
			secondFence = false;
		}
		if(Health < 40 && thirdFence){
			foreach(GameObject fence in fences3){
				fence.GetComponent<Renderer> ().enabled = false;
			}
			thirdFence = false;
		}
		if (Health < 0) {
			gameObject.GetComponent<Renderer> ().enabled = false;
		}
	}
}
