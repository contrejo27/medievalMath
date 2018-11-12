using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour {

	public static QuitButton instance;

	private GameObject GameManager;

	public Button ExitButton;

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
		Button btn = ExitButton.GetComponent<Button> ();

		btn.onClick.AddListener (QuitGame);
	}

	// Update is called once per frame
	void Update () {

	}

	public void QuitGame() {
		GameManager.GetComponent<GameStateManager> ().Quit ();
	}
}
