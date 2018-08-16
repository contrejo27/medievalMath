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

    [Header("Confirmation Panel")]
    public Image dimmerImage;
    public Image purchasePanel;
    public Text purchasePanelText;
    public Image failPurchasePanel;

    [Header("Load Out")]
    public bool upgradeUnlocked;
    public Image loadOutSelectionIndication;
    public bool selectedForLoadOut;
    public bool immuneFromLoadout;

    [Header("Star Info")]
    public int starCost;
    public Text starText;
    public Image starPanel;
    // Use this for initialization
    void Start () {
        buttonSelected = GetComponent<Button>();
        //if(upgradeUnlocked)
        //{
        //    buttonSelected.enabled = false;
        //}
        if(EnumManager.upgradeToLoadout.ContainsKey(upgradeSelection))
            selectedForLoadOut = SaveData.currentLoadout[EnumManager.upgradeToLoadout[upgradeSelection]];

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
        if (upgradeUnlocked && immuneFromLoadout)
        {
           // buttonSelected.enabled = false;
        }
        if (dependent != null){ 
        if (dependent.selectedForLoadOut == true)
        {
            loadOutSelectionIndication.gameObject.SetActive(false);
        }
        }
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
            if (dependent == null && upgradeUnlocked == true) 
            {
                buttonSelected.enabled = false; 
            }
        }
        //if(dependent != null)
        //{
        //    if (SaveData.unlockedUpgrades[dependent.upgradeSelection])
        //    {
        //        buttonSelected.enabled = false;
        //        buttonImage.sprite = lockedGraphic;
        //        starPanel.gameObject.SetActive(false);
        //        starText.enabled = false;
        //        return;
        //    }
        //}
        
        if(levelDependency > SaveData.levelsCompleted)
        {
            buttonSelected.enabled = false;
            buttonImage.sprite = lockedGraphic;
        }
    }
    void ShowPurchased()
    {
        
        buttonImage.sprite = purchasedGraphic;
        upgradeUnlocked = true;
        //buttonSelected.enabled = false;

        if (starText != null)
        {
            starPanel.gameObject.SetActive(false);
            starText.enabled = false;
        }
        
        Debug.Log("Purchased");
    }
    public void AttemptPurchase()
    {
        if(selectedForLoadOut == true)
        {
            return;
        }
        if (upgradeUnlocked && loadOutSelectionIndication != null && selectedForLoadOut == false)
        {
            Debug.Log("UpgradeUnlocked = true");
            SelectForLoadOut();
            return;
        }
        Debug.Log("EnteredSetUnlocked");
        if(starCost <= SaveData.numStars)
        {
            if (upgradeUnlocked)
            {
               // buttonSelected.enabled = false;
            }
            if (purchasePanel != null && purchasePanelText != null)
            {
                purchasePanelText.text = "Are you sure you wish to purchase " + upgradeSelection + "?";
                dimmerImage.gameObject.SetActive(true);
                purchasePanel.gameObject.SetActive(true);
            }
            if(dependent != null)
            {
                dependent.ShowLocked();
            }
            
           
            
            //SaveData.SaveDataToJSon();
        }
        else if(starCost > SaveData.numStars)
        {
            StartCoroutine(CostFailPanelFade());
        } 
    }
    public void ConfirmPurchase()
    {
        SaveData.unlockedUpgrades[upgradeSelection] = true;
        GameStateManager.instance.SpendStars(starCost);
        dimmerImage.gameObject.SetActive(false);
        purchasePanel.gameObject.SetActive(false);
        ShowPurchased();
    }
    public void DeclinePurchase()
    {
        dimmerImage.gameObject.SetActive(false);
        purchasePanel.gameObject.SetActive(false);
    }
    IEnumerator CostFailPanelFade()
    {
        failPurchasePanel.gameObject.SetActive(true);
        failPurchasePanel.CrossFadeAlpha(0, 2f, false);
        failPurchasePanel.GetComponentInChildren<Text>().CrossFadeAlpha(0, 2f, false);

        yield return new WaitForSeconds(2f);
        failPurchasePanel.gameObject.SetActive(false);
    }

    void SelectForLoadOut()
    {
        
        
        if (dependent != null)
        {
            if (SaveData.unlockedUpgrades[dependent.upgradeSelection] )
            {
                loadOutSelectionIndication.gameObject.SetActive(true);
                SaveData.currentLoadout[EnumManager.upgradeToLoadout[dependent.upgradeSelection]] = dependent.selectedForLoadOut = false;
                SaveData.currentLoadout[EnumManager.upgradeToLoadout[upgradeSelection]] = selectedForLoadOut = true;

                return;
            }
        }
    }
}
