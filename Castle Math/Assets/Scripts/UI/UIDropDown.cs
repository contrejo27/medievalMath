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

    [HideInInspector]
    public float titleHeight;
    float fullHeight;
    float lineSpacing = 1.2f;

    // Delegates
    public delegate void CheckToggle(UIDropDown dropDown);
    public event CheckToggle OnExpand;
    public event CheckToggle OnClose;

    [HideInInspector]
    public bool isExpanded = false;

    // Fuctions:
    // GetFullHeight
    // AddEntry
    // Expand
    // Collapse

    // Additional scripts:
    // Scroll

    // Use this for initialization
    void Awake()
    {
        textGen = new TextGenerator();
    }

    void Start () {
        //title = GetComponent<Text>();
        //rectTransform = GetComponent<RectTransform>();
        
        textGenSettings = title.GetGenerationSettings(rectTransform.rect.size);

        //titleHeight = textGen.GetPreferredHeight(title.text, textGenSettings);
        titleHeight = title.gameObject.GetComponent<RectTransform>().rect.height;
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
        entryRT.rect.Set(rectTransform.rect.x+GetComponent<RectTransform>().rect.width*1.5f, rectTransform.rect.y, 300, rectTransform.rect.height);
        entryRT.anchoredPosition =  new Vector3(0,-entryRT.rect.height,0)*(2f/3f);
        
        entryHeights.Add(entryRT, /*textGen.GetPreferredHeight(entryText.text, entryText.GetGenerationSettings(entryRT.rect.size))*/ entryRT.rect.height);
        fullHeight += entryHeights[entryRT];

        dropDownInfo.Add(entryRT);
        entryObject.SetActive(false);

    }

    public void ToggleExapnd()
    {
        if (!isExpanded)
        {
            isExpanded = true;
            Expand();
        }
        else
        {
            isExpanded = false;
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
            Debug.Log(totalHeight);
            UtilityFunctions.instance.UIPositionLerp(dropDownInfo[i], .2f,new Vector3(GetComponent<RectTransform>().rect.width * 1.5f, -dropDownInfo[i].rect.height*(2f/3f),0), new Vector3(GetComponent<RectTransform>().rect.width * 1.5f, -totalHeight - dropDownInfo[i].rect.height * (2f / 3f), 0), true);
            
        }
    }

    public void Close()
    {
        OnClose(this);
        for (int i = 0; i < dropDownInfo.Count; i++)
        {
            //dropDownInfo[i].anchoredPosition = new Vector3(GetComponent<RectTransform>().rect.width * 1.5f, -dropDownInfo[i].rect.height * (2f / 3f), 0);
            //dropDownInfo[i].gameObject.SetActive(false);
            UtilityFunctions.instance.UIPositionLerp(dropDownInfo[i], .2f, dropDownInfo[i].anchoredPosition, new Vector3(GetComponent<RectTransform>().rect.width * 1.5f, -dropDownInfo[i].rect.height * (2f / 3f), 0), true, true);

        }
    }

    /*
    IEnumerator StaggeredExpand()
    {
        
    }
    */

    public float GetSubHeight()
    {
        Debug.Log("Subheight: " + ((fullHeight) * lineSpacing));
        return (fullHeight)*lineSpacing;
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
