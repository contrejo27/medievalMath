using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class doorHealth : MonoBehaviour {
	public Text doorHealthText;

	public int Health = 100;
	
	private gameStateManager GameManager;

	// Use this for initialization
	void Start () {
			GameManager = GameObject.FindObjectOfType<gameStateManager> ();

	}
	
	// Update is called once per frame
	void Update () {
		doorHealthText.text = "Door Health " + Health.ToString();
		if(Health<1) GameManager.LoseState ();

	}

	//Update health
	public void TakeDamageGate(int damage) {
		Debug.Log ("Health: " + Health);
		Health -= damage;
	}
}
