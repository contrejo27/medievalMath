using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
//using System;
using UnityEngine.UI;

/// <summary>
///holds math settings.
///NOTE: To add a new question type add a 'MathType' variable to current MathController MathTypes, add 'MathType' to mathList array and add questionCategory to enumManager
/// </summary>
public class MathController : MonoBehaviour
{
    public class MathType
    {
        public string mathName;
        public bool isEnabled;
        public EnumManager.QuestionCategories questionCategory;

        public MathType(string lMathName, bool lIsEnabled, EnumManager.QuestionCategories lQuestionCategory)
        {
            mathName = lMathName;
            isEnabled = lIsEnabled;
            questionCategory = lQuestionCategory;
        }

    }

    public static MathController instance;

    //current MathTypes
    MathType add_sub = new MathType("add_sub", false, EnumManager.QuestionCategories.AddOrSubtract);
    MathType mult_divide = new MathType("mult_divide", false, EnumManager.QuestionCategories.MultiplyOrDivide);
    MathType fractions = new MathType("fractions", false, EnumManager.QuestionCategories.Fractions);
    MathType preAlgebra = new MathType("preAlgebra", false, EnumManager.QuestionCategories.Algebra);

    public Text mathInstructions;
    public float startTime;

    public MathType[] mathList = new MathType[4];
    bool hasStarted;

    Color textColor;


    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mathList[0] = add_sub;
        mathList[1] = mult_divide;
        mathList[2] = fractions;
        mathList[3] = preAlgebra;

    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        hasStarted = true;
    }

    public MathType selectMathType(EnumManager.QuestionCategories mathCategory)
    {
        MathType selectedMathType = null;
        foreach (MathType m in mathList)
        {
            if (m.questionCategory == mathCategory)
            {
                selectedMathType = m;
            }
        }
        return selectedMathType;
    }

    public void unlockMath()
    {/*
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
        */
    }


    public IEnumerator ActivatorVR(string vrToggle)
    {
        SceneManager.LoadScene("LevelSelection");
        yield return new WaitForSeconds(.5f);
        UnityEngine.XR.XRSettings.LoadDeviceByName(vrToggle);
        yield return null;
        UnityEngine.XR.XRSettings.enabled = true;
    }

    public void StartGame()
    {
        Debug.Log("Starting game");
        StartCoroutine(ActivatorVR("Cardboard"));
        startTime = Time.time;
    }
}
