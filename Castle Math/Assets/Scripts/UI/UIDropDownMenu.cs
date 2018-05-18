using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDropDownMenu : MonoBehaviour {
    [SerializeField]
    public List<UIDropDown> dropDowns = new List<UIDropDown>();

    void Awake()
    {
        foreach (UIDropDown dd in dropDowns)
        {
            dd.OnExpand += UpdateListPositions;
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
        for(int i = index+1; i < dropDowns.Count; i++)
        {
            UtilityFunctions.instance.UILerp(dropDowns[i].rectTransform,
                .5f,
                dropDowns[i].rectTransform.anchoredPosition,
                dropDowns[i].rectTransform.anchoredPosition + new Vector2(0,dropDowns[i].rectTransform.anchoredPosition.y-dd.GetSubHeight()),
                true);
        }
    }
}
