using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDropDownMenu : MonoBehaviour {
    [SerializeField]
    public List<UIDropDown> dropDowns = new List<UIDropDown>();

    public float lineSpacing = 1.5f;

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
        float combinedInitHeight = (index < dropDowns.Count-1) ? dropDowns[index+1].titleHeight*lineSpacing : dropDowns[index].titleHeight*lineSpacing;
        for(int i = index+1; i < dropDowns.Count; i++)
        {
            
            //combinedHeight = 0;
            if (dd.isExapnded)
            {
                UtilityFunctions.instance.UILerp(dropDowns[i].rectTransform,
                    .5f,
                    dropDowns[i].rectTransform.anchoredPosition,
                    dropDowns[index].rectTransform.anchoredPosition + new Vector2(0, -(dd.GetSubHeight() + combinedInitHeight)),
                    true);
            }
            else
            {
                UtilityFunctions.instance.UILerp(dropDowns[i].rectTransform,
                    .5f,
                    dropDowns[i].rectTransform.anchoredPosition,
                    dropDowns[index].rectTransform.anchoredPosition - new Vector2(0, combinedInitHeight/*lineSpacing*/),
                    true);
            }
            combinedInitHeight += (dropDowns[i].isExapnded) ? dropDowns[i].GetSubHeight() + lineSpacing : dropDowns[i].titleHeight*lineSpacing;
        }
    }
}
