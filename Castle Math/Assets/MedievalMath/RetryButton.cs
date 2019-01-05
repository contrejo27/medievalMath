using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class RetryButton : MonoBehaviour {

	public static RetryButton instance;

	private GameObject GameManager;

	public Button ResetButton;

	// Use this for initialization
	void Awake () {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
	}

	void Start () {
		GameManager = GameObject.Find ("GameManager");
		Button btn = ResetButton.GetComponent<Button> ();

		btn.onClick.AddListener (Reset);
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void Reset() {
		GameManager.GetComponent<GameStateManager> ().Retry ();
	}
}
