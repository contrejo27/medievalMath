using System.Collections;
//using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MathController : MonoBehaviour {
    private MathController instance;

    public bool add_sub;
    public bool mult_divide;
    public bool fractions;
    public bool preAlgebra;
    //public bool wordProblems;
    public Text mathInstructions;
    public float startTime;

    bool hasStarted;

    Color textColor;

    //temp bools just to see if level is completed
    public bool level1_Completed;
    public bool level2_Completed;
    public bool level3_Completed;
    public bool level4_Completed;

    //Gets Current Scene and assigns a string to it

    // Use this for initialization
    void Awake() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
        // SaveData.LoadDataFromJSon();
    }

    void Start () {
        DontDestroyOnLoad(this.gameObject);

        level1_Completed = false;
        level2_Completed = false;
        level3_Completed = false;
        level4_Completed = false;

        if (Debug.isDebugBuild || Application.isEditor) {
            Debug.Log("IN EDITOR/DEBUG");
            GameObject.Find("add/sub").GetComponent<Toggle>().isOn = SaveData.activeQuestionCategories[EnumManager.ActiveQuestionCategories.AddOrSubtract];
            GameObject.Find("mult/divide").GetComponent<Toggle>().isOn = SaveData.activeQuestionCategories[EnumManager.ActiveQuestionCategories.MultiplyOrDivide];
            GameObject.Find("Pre-Algebra").GetComponent<Toggle>().isOn = SaveData.activeQuestionCategories[EnumManager.ActiveQuestionCategories.Algebra];
        }
        else if (LocalUserData.IsLoggedIn() == false) {
            GameObject.Find("add/sub").GetComponent<Toggle>().isOn = true;

            GameObject multGO = GameObject.Find("mult/divide");
            mult_divide = multGO.GetComponent<Toggle>().interactable = false;
            multGO.transform.Find("Text").GetComponent<Text>().color = Color.grey;

            GameObject fractionGO = GameObject.Find("Fractions");
            fractions = fractionGO.GetComponent<Toggle>().interactable = false;
            fractionGO.transform.Find("Text").GetComponent<Text>().color = Color.grey;

            // GameObject wordGO = GameObject.Find("Word Problems");
            // wordProblems = wordGO.GetComponent<Toggle>().interactable = false;
            // wordGO.transform.Find("Text").GetComponent<Text>().color = Color.grey;

            GameObject PreAlgGO = GameObject.Find("Pre-Algebra");
            preAlgebra = PreAlgGO.GetComponent<Toggle>().interactable = false;
            PreAlgGO.transform.Find("Text").GetComponent<Text>().color = Color.grey;

            mathInstructions.text = "Subscribe to unlock all lessons";
            textColor = mathInstructions.color;
            mathInstructions.color = new Color(.62f,.2f,.27f);
        }
        else {
            GameObject addSubGO = GameObject.Find("add/sub");
            addSubGO.GetComponent<Toggle>().isOn = true;
            add_sub = addSubGO.GetComponent<Toggle>().interactable = true;
       }

        hasStarted = true;
    }

    public void unlockMath() {
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

    public void Update() {
        if (SceneManager.GetActiveScene().name == "MathTest") {
            if (GameObject.Find ("WaveManager").GetComponent<WaveManager> ().levelComplete) {
                level1_Completed = true;
            }
        }
        if (SceneManager.GetActiveScene().name == "frostLevel") {
            if (GameObject.Find ("WaveManager").GetComponent<WaveManager> ().levelComplete) {
                level2_Completed = true;
            }
        }
        if (SceneManager.GetActiveScene().name == "desertLevel") {
            if (GameObject.Find ("WaveManager").GetComponent<WaveManager> ().levelComplete) {
                level3_Completed = true;
            }
        }
        if (SceneManager.GetActiveScene().name == "bossLevel") {
            if (GameObject.Find ("WaveManager").GetComponent<WaveManager> ().levelComplete) {
                level4_Completed = true;
            }
        }
    }

    public void UpdateSelection() {
        add_sub = GameObject.Find("add/sub").GetComponent<Toggle>().isOn;
        print("addsub " + add_sub);
        mult_divide = GameObject.Find("mult/divide").GetComponent<Toggle>().isOn;
        print("mult_divide " + mult_divide);
        fractions = GameObject.Find("Fractions").GetComponent<Toggle>().isOn;
        print("fractions " + fractions);
        preAlgebra = GameObject.Find("Pre-Algebra").GetComponent<Toggle>().isOn;
        print("preAlgebra " + preAlgebra);

        if (hasStarted) {
            SaveData.activeQuestionCategories[EnumManager.ActiveQuestionCategories.AddOrSubtract] = add_sub;
            SaveData.activeQuestionCategories[EnumManager.ActiveQuestionCategories.MultiplyOrDivide] = mult_divide;
            SaveData.activeQuestionCategories[EnumManager.ActiveQuestionCategories.Algebra] = preAlgebra;
        }

        SaveData.SaveDataToJSon();
        // wordProblems = GameObject.Find("Word Problems").GetComponent<Toggle>().isOn;
    }

    public IEnumerator ActivatorVR(string vrToggle) {
        SceneManager.LoadScene(1);
        yield return new WaitForSeconds(.5f);
        UnityEngine.XR.XRSettings.LoadDeviceByName(vrToggle);
        yield return null;
        UnityEngine.XR.XRSettings.enabled = true;
    }

    public void StartGame() {
        Debug.Log ("Starting game");
        StartCoroutine(ActivatorVR("Cardboard"));
        startTime = Time.time;
    }    
}
