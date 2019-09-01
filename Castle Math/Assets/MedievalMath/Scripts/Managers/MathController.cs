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
    public GameObject contentUnlockedNotice;
    public GameObject unlockMathNotice;

    public static MathController instance;

    //current MathTypes
    MathType add = new MathType("add", false, EnumManager.QuestionCategories.Add);
    MathType sub = new MathType("sub", false, EnumManager.QuestionCategories.Subtract);
    MathType mult_divide = new MathType("mult_divide", false, EnumManager.QuestionCategories.MultiplyOrDivide);
    MathType fractions = new MathType("fractions", false, EnumManager.QuestionCategories.Fractions);
    MathType preAlgebra = new MathType("preAlgebra", false, EnumManager.QuestionCategories.Algebra);
    MathType factFamilies = new MathType("factFamilies", false, EnumManager.QuestionCategories.FactFamilies);

    public int startingGrade = 1;

    public Text mathInstructions;
    public float startTime;

    public MathType[] mathList = new MathType[6];

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

        mathList[0] = add;
        mathList[1] = sub;
        mathList[2] = mult_divide;
        mathList[3] = fractions;
        mathList[4] = preAlgebra;
        mathList[5] = factFamilies;
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
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
    {
        contentUnlockedNotice.SetActive(true);

        GameObject[] mathSkills = GameObject.FindGameObjectsWithTag("MathSkillButton");
        foreach (GameObject GO in mathSkills)
        {
            GO.GetComponent<Toggle>().interactable = true;
        }
        unlockMathNotice.SetActive(false);
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
