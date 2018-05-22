using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDropDownMenu : MonoBehaviour {
    [SerializeField]
    public List<UIDropDown> dropDowns = new List<UIDropDown>();

    public float lineSpacing = 1.5f;

    public float scaleModifier = .4f;

    void Awake()
    {
        foreach (UIDropDown dd in dropDowns)
        {
            dd.OnExpand += UpdateListPositions;
            dd.OnClose += UpdateListPositions;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void UpdateListPositions(UIDropDown dd)
    {
        int index = dropDowns.IndexOf(dd);
        float combinedInitHeight = dd.rectTransform.rect.height / 2 + lineSpacing;//(dd.isExpanded) ? 0 : dd.rectTransform.rect.height; //dd.rectTransform.rect.height*lineSpacing;//(index < dropDowns.Count-1) ? dropDowns[index+1].titleHeight*lineSpacing : dropDowns[index].titleHeight*lineSpacing;
        for(int i = index+1; i < dropDowns.Count; i++)
        {
            combinedInitHeight += dropDowns[i].rectTransform.rect.height / 2;
            //combinedHeight = 0;
            if (dd.isExpanded)
            {
                Debug.Log("Subheight: " + dd.GetSubHeight() + ", combinedhight: " + combinedInitHeight + ", added: " + (combinedInitHeight + dd.GetSubHeight()));
                UtilityFunctions.instance.UIPositionLerp(dropDowns[i].rectTransform,
                    .2f,
                    dropDowns[i].rectTransform.anchoredPosition,
                    dropDowns[index].rectTransform.anchoredPosition + new Vector2(0, -((dd.GetSubHeight() + combinedInitHeight)*scaleModifier)),
                    true);
            }
            else
            {
                //if(combinedInitHeight == 0) combinedInitHeight = 
                UtilityFunctions.instance.UIPositionLerp(dropDowns[i].rectTransform,
                    .2f,
                    dropDowns[i].rectTransform.anchoredPosition,
                    dropDowns[index].rectTransform.anchoredPosition - new Vector2(0, combinedInitHeight*scaleModifier/*lineSpacing*/),
                    true);
            }
            combinedInitHeight += dropDowns[i].rectTransform.rect.height / 2 + lineSpacing + ((dropDowns[i].isExpanded) ? dropDowns[i].GetSubHeight() : 0);
        }
    }

    public void AddDropDownElement(UIDropDown dd)
    {
        dropDowns.Add(dd);
        dd.OnExpand += UpdateListPositions;
        dd.OnClose += UpdateListPositions;
    }
}
