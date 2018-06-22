using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonSelect : MonoBehaviour {
    public EnumManager.Upgrades upgradeSelection;
    public Button buttonSelected;
    public Image buttonImage;

    [Header("Requirements to Unlock")]
    //Level Dependency is based on the current level which the player has completed
    //Dependent is for Upgrade _____ 2A/2B, locking the alternate out on selection for purchase of the former
    //Prereq is Level 1 requirment to unlock 2 / 2A / 2B
    public int levelDependency = -1;
    public UpgradeButtonSelect dependent;
    public UpgradeButtonSelect prerequisite;

    [Header("Sprites")]
    public Sprite purchasedGraphic;
    public Sprite lockedGraphic;
    public Sprite unlockGraphic;

    [Header("Star Info")]
    public int starCost;
    public Text starText;
    public Image starPanel;
    // Use this for initialization
    void Start () {
        buttonSelected = GetComponent<Button>();

        if (starText != null)
        {
            starText.text = starCost.ToString();
        }
        if (SaveData.unlockedUpgrades[upgradeSelection])
        {
            ShowPurchased();
        }
        ShowLocked();
    }
	
	// Update is called once per frame
	void Update () {
        ShowLocked();
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
            if (SaveData.unlockedUpgrades[prerequisite.upgradeSelection] && !SaveData.unlockedUpgrades[upgradeSelection])
            {
               buttonSelected.enabled = true;
               buttonImage.sprite = unlockGraphic;
                if(starPanel != null)
                {
                    starPanel.gameObject.SetActive(true);
                    starText.enabled = true;
                }
            }
        }
        if(dependent != null)
        {
            if (SaveData.unlockedUpgrades[dependent.upgradeSelection])
            {
                buttonSelected.enabled = false;
                buttonImage.sprite = lockedGraphic;
                starPanel.gameObject.SetActive(false);
                starText.enabled = false;
                return;
            }
        }
        
        if(levelDependency > SaveData.levelsCompleted)
        {
            buttonSelected.enabled = false;
            buttonImage.sprite = lockedGraphic;
        }
    }
    void ShowPurchased()
    {
        buttonImage.sprite = purchasedGraphic;
        buttonSelected.enabled = false;
        if (starText != null)
        {
            starPanel.gameObject.SetActive(false);
            starText.enabled = false;
        }
        Debug.Log("Purchased");
    }
    public void SetPurchased()
    {
        Debug.Log("EnteredSetUnlocked");
        if(starCost <= SaveData.numStars)
        {
            SaveData.unlockedUpgrades[upgradeSelection] = true;
            GameStateManager.instance.SpendStars(starCost);
            ShowPurchased();

            if(dependent != null)
            {
                dependent.ShowLocked();
            }
            
            //SaveData.SaveDataToJSon();
        }
        
    }
}
