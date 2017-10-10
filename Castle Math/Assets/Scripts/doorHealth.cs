using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class doorHealth : MonoBehaviour {
	public Text doorHealthText;

	public int health = 100;
	
	private gameStateManager GameManager;

	// Use this for initialization
	void Start () {
				GameManager = GameObject.FindObjectOfType<gameStateManager> ();

	}
	
	// Update is called once per frame
	void Update () {
		doorHealthText.text = "Door Health " + health.ToString();
		if(health<1) GameManager.LoseState ();

	}
}
