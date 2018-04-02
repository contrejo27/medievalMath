using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class mathController : MonoBehaviour {

	public bool add_sub;
	public bool mult_divide;
	public bool fractions;
	public bool preAlgebra;
	public bool wordProblems;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);
		GameObject.Find("add/sub").GetComponent<Toggle>().isOn = true;
	}

	public void UpdateSelection(){
		add_sub = GameObject.Find("add/sub").GetComponent<Toggle>().isOn;
		mult_divide = GameObject.Find("mult/divide").GetComponent<Toggle>().isOn;
		fractions = GameObject.Find("Fractions").GetComponent<Toggle>().isOn;
		preAlgebra = GameObject.Find("Pre-Algebra").GetComponent<Toggle>().isOn;
		wordProblems = GameObject.Find("Word Problems").GetComponent<Toggle>().isOn;
	}
}
