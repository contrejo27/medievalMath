using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDropDown : MonoBehaviour {

    public Text title;

    public RectTransform rectTransform;

    Button dropDownButton;

    List<RectTransform> dropDownInfo = new List<RectTransform>();    
    TextGenerator textGen;
    TextGenerationSettings textGenSettings;
    Dictionary<RectTransform, float> entryHeights = new Dictionary<RectTransform, float>();

    public float titleHeight;
    float fullHeight;
    float lineSpacing = 1.2f;

    // Delegates
    public delegate void CheckToggle(UIDropDown dropDown);
    public event CheckToggle OnExpand;
    public event CheckToggle OnClose;

    [HideInInspector]
    public bool isExapnded = false;

    // Fuctions:
    // GetFullHeight
    // AddEntry
    // Expand
    // Collapse
    
    // Additional scripts:
    // Scroll

	// Use this for initialization
	void Start () {
        title = GetComponent<Text>();
        rectTransform = GetComponent<RectTransform>();

        textGen = new TextGenerator();
        textGenSettings = title.GetGenerationSettings(rectTransform.rect.size);

		titleHeight = textGen.GetPreferredHeight(title.text, textGenSettings);

        //TestAdd();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddDropDownEntry(string entry)
    {
        GameObject entryObject = Instantiate(Resources.Load("UI/Text")) as GameObject;
        
        entryObject.transform.parent = transform;
        entryObject.transform.position = Vector3.zero;


        Text entryText = entryObject.GetComponent<Text>();
        entryText.text = entry;

        RectTransform entryRT = entryObject.GetComponent<RectTransform>();
        entryRT.rect.Set(rectTransform.rect.x, rectTransform.rect.y, rectTransform.rect.width, rectTransform.rect.height);
        entryRT.anchoredPosition = rectTransform.anchoredPosition;
        entryHeights.Add(entryRT, textGen.GetPreferredHeight(entryText.text, entryText.GetGenerationSettings(entryRT.rect.size)));
        fullHeight += entryHeights[entryRT];

        dropDownInfo.Add(entryRT);
        entryObject.SetActive(false);

    }

    public void ToggleExapnd()
    {
        if (!isExapnded)
        {
            isExapnded = true;
            Expand();
        }
        else
        {
            isExapnded = false;
            Close();
        }
    }

    public void Expand()
    {
        OnExpand(this);
        float totalHeight = (fullHeight  +titleHeight/2)*lineSpacing;
        for (int i = dropDownInfo.Count - 1; i >= 0; i--)
        {
            dropDownInfo[i].gameObject.SetActive(true);
            if (i == dropDownInfo.Count - 1)
            {
                totalHeight -= entryHeights[dropDownInfo[i]] * lineSpacing / 2;
            }
            else
            {
                totalHeight -= entryHeights[dropDownInfo[i]] * lineSpacing / 2 + entryHeights[dropDownInfo[i]] * lineSpacing / 2;
            }
            UtilityFunctions.instance.UILerp(dropDownInfo[i], .5f,Vector3.zero, new Vector3(0, -totalHeight, 0), true);
            
        }
    }

    public void Close()
    {
        OnClose(this);
        for (int i = 0; i < dropDownInfo.Count; i++)
        {
            
            UtilityFunctions.instance.UILerp(dropDownInfo[i], .5f, dropDownInfo[i].anchoredPosition, Vector3.zero, true);

        }
    }

    /*
    IEnumerator StaggeredExpand()
    {
        
    }
    */

    public float GetSubHeight()
    {
        return (fullHeight+titleHeight)*lineSpacing;
    }

    public void TestAdd()
    {
        AddDropDownEntry("To");
        AddDropDownEntry("be");
        AddDropDownEntry("fair");
        AddDropDownEntry("you");
        AddDropDownEntry("have");
        AddDropDownEntry("to");
        AddDropDownEntry("have");
        AddDropDownEntry("a");/*
        AddDropDownEntry("very");
        AddDropDownEntry("high");
        AddDropDownEntry("IQ");
        AddDropDownEntry("to");
        AddDropDownEntry("understand");
        AddDropDownEntry("Rick");
        AddDropDownEntry("and");
        AddDropDownEntry("Morty");
        */
    }

    void OnDisable()
    {
        foreach(RectTransform rt in dropDownInfo)
        {
            rt.anchoredPosition = Vector3.zero;
            rt.gameObject.SetActive(false);
        }
    }
}
