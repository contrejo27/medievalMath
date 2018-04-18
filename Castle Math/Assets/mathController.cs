using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mathController : MonoBehaviour {

	public bool add_sub;
	public bool mult_divide;
	public bool fractions;
	public bool preAlgebra;
	public bool wordProblems;
    public Text mathInstructions;

    // Use this for initialization
    void Start () {
		DontDestroyOnLoad(this.gameObject);

        if (PlayerPrefs.GetInt("LoggedIn") == 0) {
            GameObject.Find("add/sub").GetComponent<Toggle>().isOn = true;

            GameObject multGO = GameObject.Find("mult/divide");
            mult_divide = multGO.GetComponent<Toggle>().interactable = false;
            multGO.transform.Find("Text").GetComponent<Text>().color = Color.grey;

            GameObject fractionGO = GameObject.Find("Fractions");
            fractions = fractionGO.GetComponent<Toggle>().interactable = false;
            fractionGO.transform.Find("Text").GetComponent<Text>().color = Color.grey;

            GameObject wordGO = GameObject.Find("Word Problems");
            wordProblems = wordGO.GetComponent<Toggle>().interactable = false;
            wordGO.transform.Find("Text").GetComponent<Text>().color = Color.grey;

            GameObject PreAlgGO = GameObject.Find("Pre-Algebra");
            preAlgebra = PreAlgGO.GetComponent<Toggle>().interactable = false;
            PreAlgGO.transform.Find("Text").GetComponent<Text>().color = Color.grey;

            mathInstructions.text = "Subscribe to unlock all lessons";
        }
        else{
            GameObject.Find("mult/divide").GetComponent<Toggle>().isOn = true;
        }
    }

	public void UpdateSelection(){
		add_sub = GameObject.Find("add/sub").GetComponent<Toggle>().isOn;
		mult_divide = GameObject.Find("mult/divide").GetComponent<Toggle>().isOn;
		fractions = GameObject.Find("Fractions").GetComponent<Toggle>().isOn;
		preAlgebra = GameObject.Find("Pre-Algebra").GetComponent<Toggle>().isOn;
		wordProblems = GameObject.Find("Word Problems").GetComponent<Toggle>().isOn;
	}


    public IEnumerator ActivatorVR(string vrToggle)
    {
        SceneManager.LoadScene(1);
        yield return new WaitForSeconds(.5f);
        UnityEngine.VR.VRSettings.LoadDeviceByName(vrToggle);
        yield return null;
        UnityEngine.VR.VRSettings.enabled = true;
    }

    public void StartGame()
    {
        StartCoroutine(ActivatorVR("Cardboard"));

    }    
}
