using System.Collections;
//using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mathController : MonoBehaviour {

	public bool add_sub;
	public bool mult_divide;
	public bool fractions;
	public bool preAlgebra;
	//public bool wordProblems;
    public Text mathInstructions;


    Color textColor; 
    // Use this for initialization
    void Start () {
        PlayerPrefs.SetInt("LoggedIn",1); //for debugging 

        DontDestroyOnLoad(this.gameObject);
        if (PlayerPrefs.GetInt("LoggedIn") == 0) {
            GameObject.Find("add/sub").GetComponent<Toggle>().isOn = true;

            GameObject multGO = GameObject.Find("mult/divide");
            mult_divide = multGO.GetComponent<Toggle>().interactable = false;
            multGO.transform.Find("Text").GetComponent<Text>().color = Color.grey;

            GameObject fractionGO = GameObject.Find("Fractions");
            fractions = fractionGO.GetComponent<Toggle>().interactable = false;
            fractionGO.transform.Find("Text").GetComponent<Text>().color = Color.grey;

           // GameObject wordGO = GameObject.Find("Word Problems");
            //wordProblems = wordGO.GetComponent<Toggle>().interactable = false;
            //wordGO.transform.Find("Text").GetComponent<Text>().color = Color.grey;

            GameObject PreAlgGO = GameObject.Find("Pre-Algebra");
            preAlgebra = PreAlgGO.GetComponent<Toggle>().interactable = false;
            PreAlgGO.transform.Find("Text").GetComponent<Text>().color = Color.grey;

            mathInstructions.text = "Subscribe to unlock all lessons";
            textColor = mathInstructions.color;
            mathInstructions.color = new Color(.62f,.2f,.27f);
        }
        else{
            GameObject multGO = GameObject.Find("mult/divide");
            multGO.GetComponent<Toggle>().isOn = true;
            mult_divide = multGO.GetComponent<Toggle>().interactable = true;
        }
    }

	public void unlockMath(){
        print("unlockingMath");
        Color mathOrange = new Color(0.91F, 0.58F, 0.264F, 1.0F);
        GameObject addGO = GameObject.Find("add/sub");
        add_sub = addGO.GetComponent<Toggle>().interactable = true;
        addGO.transform.Find("Text").GetComponent<Text>().color = mathOrange;

        GameObject multGO = GameObject.Find("mult/divide");
        mult_divide = multGO.GetComponent<Toggle>().interactable = true;
        multGO.transform.Find("Text").GetComponent<Text>().color = mathOrange;

        GameObject fractionGO = GameObject.Find("Fractions");
        fractions = fractionGO.GetComponent<Toggle>().interactable = true;
        fractionGO.transform.Find("Text").GetComponent<Text>().color = mathOrange;

       // GameObject wordGO = GameObject.Find("Word Problems");
       // wordProblems = wordGO.GetComponent<Toggle>().interactable = true;
       // wordGO.transform.Find("Text").GetComponent<Text>().color = mathOrange;

        GameObject PreAlgGO = GameObject.Find("Pre-Algebra");
        preAlgebra = PreAlgGO.GetComponent<Toggle>().interactable = true;
        PreAlgGO.transform.Find("Text").GetComponent<Text>().color = mathOrange;

        mathInstructions.text = "Select in-game lessons";
        mathInstructions.color = textColor;

    }

    public void UpdateSelection()
    {
        add_sub = GameObject.Find("add/sub").GetComponent<Toggle>().isOn;
        print("addsub " + add_sub);
        mult_divide = GameObject.Find("mult/divide").GetComponent<Toggle>().isOn;
        print("mult_divide " + mult_divide);
        fractions = GameObject.Find("Fractions").GetComponent<Toggle>().isOn;
        print("fractions " + fractions);
        preAlgebra = GameObject.Find("Pre-Algebra").GetComponent<Toggle>().isOn;
        print("preAlgebra " + preAlgebra);
        //wordProblems = GameObject.Find("Word Problems").GetComponent<Toggle>().isOn;
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
