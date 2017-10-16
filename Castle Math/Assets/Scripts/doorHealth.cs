using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class doorHealth : MonoBehaviour {
	public Text doorHealthText;

	public int Health = 100;
	
	private gameStateManager GameManager;
	private ExampleGUIAspectsController doorHealthBar;

	// Use this for initialization
	void Start () {
			GameManager = GameObject.FindObjectOfType<gameStateManager> ();
			doorHealthBar = GameObject.FindObjectOfType<ExampleGUIAspectsController> ();

	}
	
	// Update is called once per frame
	void Update () {
		if(Health<1) GameManager.LoseState ();

	}

	//Update health
	public void TakeDamageGate(int damage) {
		doorHealthBar.updateHealth(damage);
		Health -= damage;

		if (Health < 0) {
			this.gameObject.GetComponent<Renderer> ().enabled = false;
			Debug.Log ("destroyed dh");
			this.destroyed = true;
		}
	}
}
