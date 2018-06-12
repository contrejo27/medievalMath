using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeObject : BaseInteractableObject {

    public EnumManager.Upgrades upgradeType;
    public int starCost;
    bool selectable;
    bool purchaseable;
    bool currentlyOwned;

	// Use this for initialization
	protected override void Init () {
        SetSelectable();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnInteract()
    {
        if (selectable && purchaseable)
        {
            SaveData.unlockedUpgrades[upgradeType] = true;
            //SaveData.SaveDataToJSon();
            // This function also saves the game.
            GameStateManager.instance.SpendStars(starCost);

            base.OnInteract();
        }
    }

    public override void OnPassOver()
    {
        if (selectable)
        {
            if (purchaseable) SetHighlight(Color.green);
            else SetHighlight(Color.yellow);
        }
        else
        {
            if(!currentlyOwned) SetHighlight(Color.red);
        }
        base.OnPassOver();
    }

    public override void OnEndPassOver()
    {
        if (!currentlyOwned)
        {
            RemoveHighlight();
        }
        base.OnEndPassOver();
    }

    void SetSelectable()
    {
        if (SaveData.unlockedUpgrades[upgradeType])
        {
            selectable = false;
            currentlyOwned = true;
            return;
        }

        
        // TEMPORARY VALUE
        if (starCost < GameStateManager.instance.GetStars())
        {
            purchaseable = true;
        }
        

        switch (upgradeType)
        {
            case EnumManager.Upgrades.Barrricade1:
                selectable = true;
                break;
            case EnumManager.Upgrades.Barricade2:
                if (SaveData.unlockedUpgrades[EnumManager.Upgrades.Barrricade1])
                {
                    selectable = true;
                }
                break;
            case EnumManager.Upgrades.FireArrows2A:
                if (SaveData.unlockedUpgrades[EnumManager.Upgrades.FireArrows1] && !SaveData.unlockedUpgrades[EnumManager.Upgrades.FireArrows2B])
                {
                    selectable = true;
                }
                break;
            case EnumManager.Upgrades.FireArrows2B:
                if (SaveData.unlockedUpgrades[EnumManager.Upgrades.FireArrows1] && !SaveData.unlockedUpgrades[EnumManager.Upgrades.FireArrows2A])
                {
                    selectable = true;
                }
                break;
            case EnumManager.Upgrades.IceArrows2A:
                if (SaveData.unlockedUpgrades[EnumManager.Upgrades.IceArrows1] && !SaveData.unlockedUpgrades[EnumManager.Upgrades.IceArrows2B])
                {
                    selectable = true;
                }
                break;
            case EnumManager.Upgrades.IceArrows2B:
                if (SaveData.unlockedUpgrades[EnumManager.Upgrades.IceArrows1] && !SaveData.unlockedUpgrades[EnumManager.Upgrades.IceArrows2A])
                {
                    selectable = true;
                }
                break;
            case EnumManager.Upgrades.ShockArrows2A:
                if (SaveData.unlockedUpgrades[EnumManager.Upgrades.ShockArrows1] && !SaveData.unlockedUpgrades[EnumManager.Upgrades.ShockArrows2B])
                {
                    selectable = true;
                }
                break;
            case EnumManager.Upgrades.ShockArrows2B:
                if (SaveData.unlockedUpgrades[EnumManager.Upgrades.ShockArrows1] && !SaveData.unlockedUpgrades[EnumManager.Upgrades.ShockArrows2A])
                {
                    selectable = true;
                }
                break;
            case EnumManager.Upgrades.Inventory1:
                selectable = true;
                break;
            case EnumManager.Upgrades.Inventory2:
                if (SaveData.unlockedUpgrades[EnumManager.Upgrades.Inventory1])
                {
                    selectable = true;
                }
                break;
            default:
                break;
        }
    }
}
