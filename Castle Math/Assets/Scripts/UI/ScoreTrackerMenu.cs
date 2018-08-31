using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrackerMenu : MonoBehaviour {
    public UIDropDownMenu statsMenu;
    Canvas canvas;
    //USE RECTTRANSFORM OF MAIN CANVAS
    public RectTransform rectTransform;
    public float xMargin;
    public float yMargin;
    public float lineSpacing;
    public float scrollSpeed;

    RectTransform localRectTransform;
    Vector2 initPosition;

    float lastMouseYPosition;
    float mouseYDelta;
    
	// Use this for initialization
	void Start () {

        canvas = GetComponent<Canvas>();
        localRectTransform = GetComponent<RectTransform>();
        initPosition = new Vector2();
        //rectTransform = GetComponent<RectTransform>();
        int counter = 0;
	    foreach(QuestionData qd in GameStateManager.instance.tracker.questionData.Values)
        {

            GameObject o = Instantiate(Resources.Load("UI/DropDownButton")) as GameObject;
            o.transform.parent = transform;

            UIDropDown dd = o.GetComponent<UIDropDown>();
            dd.rectTransform.anchoredPosition = new Vector2(-rectTransform.rect.width / 2 + xMargin, rectTransform.rect.height / 2 - yMargin - counter * lineSpacing);

            if (initPosition == Vector2.zero) initPosition = new Vector2(-rectTransform.rect.width / 2 + xMargin, rectTransform.rect.height / 2 - yMargin - counter * lineSpacing); 
           
            
            dd.title.text = qd.type;
            statsMenu.AddDropDownElement(dd);

            counter++;

            //dd.rectTransform.anchoredPosition 

            foreach(QuestionData.DataLine dl in qd.data.Values)
            {
                dd.AddDropDownEntry(dl.ToString());
            }
        }
        Debug.Log("initpos: " + initPosition);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            lastMouseYPosition = Input.mousePosition.y;
        }
        Scroll();
	}

    void Scroll()
    {
        float scrollAxis = Input.GetAxis("Mouse ScrollWheel");
        //Debug.Log("ScrolAxis: " + scrollAxis);
        if (scrollAxis == 0)
        {
            if (Input.GetButton("Fire1"))
            {
                scrollAxis = Mathf.Clamp(Input.mousePosition.y - lastMouseYPosition, -1, 1)*.1f;
                lastMouseYPosition = Input.mousePosition.y;
            }

        }

        if(scrollAxis == 0) return;
        
        if(statsMenu.dropDowns[0].rectTransform.anchoredPosition.y + scrollSpeed* scrollAxis < initPosition.y)
        {
            /*
            statsMenu.dropDowns[0].rectTransform.anchoredPosition = new Vector2(statsMenu.dropDowns[0].rectTransform.anchoredPosition.x, initPosition.y);
            float diff = statsMenu.dropDowns[0].rectTransform.anchoredPosition.y - initPosition.y;
            for (int i = 1; i < statsMenu.dropDowns.Count; i++)
            {
                statsMenu.dropDowns[i].rectTransform.anchoredPosition += new Vector2(0, diff);
            }
            */
            return;
        }
        
        UIDropDown finalDD = statsMenu.dropDowns[statsMenu.dropDowns.Count-1];
        float finalDDSize = statsMenu.scaleModifier*(finalDD.GetSubHeight() + finalDD.rectTransform.rect.height / 2 + statsMenu.lineSpacing);
        float maxPos = localRectTransform.anchoredPosition.y - localRectTransform.rect.height / 2 + finalDDSize;
        if (finalDD.rectTransform.anchoredPosition.y + scrollSpeed * scrollAxis > maxPos)
        {
            /*
            finalDD.rectTransform.anchoredPosition = new Vector2(finalDD.rectTransform.anchoredPosition.x, maxPos);
            float diff = maxPos - finalDD.rectTransform.anchoredPosition.y;
            for(int i = 0; i< statsMenu.dropDowns.Count-1; i++)
            {
                statsMenu.dropDowns[i].rectTransform.anchoredPosition += new Vector2(0, diff);
            }
            */
            return;
        }

        foreach(UIDropDown dd in statsMenu.dropDowns)
        {
            dd.rectTransform.anchoredPosition += new Vector2(0, scrollAxis * scrollSpeed);
        }
    }

    float GetFullMenuHeight()
    {
        float combinedHeight = statsMenu.dropDowns[0].rectTransform.rect.height / 2 + statsMenu.lineSpacing + ((statsMenu.dropDowns[0].isExpanded) ? statsMenu.dropDowns[0].GetSubHeight() : 0);
        for(int i = 1; i< statsMenu.dropDowns.Count; i++)
        {
            combinedHeight += statsMenu.dropDowns[i].rectTransform.rect.height + statsMenu.lineSpacing + ((statsMenu.dropDowns[i].isExpanded) ? statsMenu.dropDowns[i].GetSubHeight() : 0);
        }

        return combinedHeight * statsMenu.scaleModifier;
    }
}
