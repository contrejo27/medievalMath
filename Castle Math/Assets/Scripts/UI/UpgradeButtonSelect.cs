using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonSelect : MonoBehaviour {
    public EnumManager.Upgrades upgradeSelection;
    public Button buttonSelected;
    public Image buttonImage;
    public Sprite unlockGraphic;
    public int starCost;
    public int levelDependency = -1;
    public UpgradeButtonSelect dependent;
    public UpgradeButtonSelect prerequisite;
    public Sprite lockedGraphic;
    public Text starText;
	// Use this for initialization
	void Start () {
        if (starText != null)
        {
            starText.text = starCost.ToString();
        }
        buttonSelected = GetComponent<Button>();
        ShowLocked();
        if (SaveData.unlockedUpgrades[upgradeSelection])
        {
            ShowUnlocked();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ShowLocked()
    {
        if(prerequisite != null)
        {
            if (!SaveData.unlockedUpgrades[prerequisite.upgradeSelection])
            {
                buttonSelected.enabled = false;
                buttonImage.sprite = lockedGraphic;
                return;
            }
         
            
        }
        if(dependent != null)
        {
            if (SaveData.unlockedUpgrades[dependent.upgradeSelection])
            {
                buttonSelected.enabled = false;
                buttonImage.sprite = lockedGraphic;
                return;
            }
        }
        
        if(levelDependency > SaveData.levelsCompleted)
        {
            buttonSelected.enabled = false;
            buttonImage.sprite = lockedGraphic;
        }
    }
    void ShowUnlocked()
    {
        buttonSelected.enabled = false;
        buttonImage.sprite = unlockGraphic;
    }
    public void SetUnlocked()
    {
        Debug.Log("EnteredSetUnlocked");
        if(starCost <= SaveData.numStars)
        {
            SaveData.unlockedUpgrades[upgradeSelection] = true;
            GameStateManager.instance.SpendStars(starCost);
            ShowUnlocked();

            if(dependent != null)
            {
                dependent.ShowLocked();
            }
            
            //SaveData.SaveDataToJSon();
        }
    }
    void CheckElementalUpgradesUnlocked()
    {
        if (SaveData.levelsCompleted > 0)
        {
            
        }
        if (SaveData.levelsCompleted > 2)
        {

        }
        if(SaveData.levelsCompleted > 4)
        {

        }
    }
}
