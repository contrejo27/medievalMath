using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    // for activation/deactivation 
    public GameObject[] quivers;
    public Transform[] potionSlots;
    public List<Potion> potionsInInventory = new List<Potion>();

    bool[] unlockedSlots = {true, true, true, false, false };
    bool[] filledSlots = { false, false, false, false, false };
    //bool[] unlockedQuivers = { false, false, false, false };

    // some of these might belong in the PlayerController. We'll see.
    [Range(3,5)]
    public int inventorySize = 3;
    [HideInInspector]
    public int numPotions = 0;
    [HideInInspector]
    public bool isQuiverActive;

    void Awake()
    {
        // moving to Start() for now
        // GameStateManager.instance.inventory = this;

        // get progress from save data for quivers and inventeory size
        // read into bool arrays
    }

    void Start()
    {
        
        GameStateManager.instance.inventory = this;
        //SaveData.unlockedUpgrades[EnumManager.Upgrades.ShockArrows1] = true;
        //SaveData.SaveDataToJSon();
        if (SaveData.unlockedUpgrades[EnumManager.Upgrades.ShockArrows1])
        {
            quivers[0].SetActive(true);
        }
        if (SaveData.unlockedUpgrades[EnumManager.Upgrades.IceArrows1])
        {
            quivers[1].SetActive(true);
        }
        if (SaveData.unlockedUpgrades[EnumManager.Upgrades.FireArrows1])
        {
            quivers[2].SetActive(true);
        }
        if (SaveData.unlockedUpgrades[EnumManager.Upgrades.Inventory1])
        {
            inventorySize = 4;
            unlockedSlots[3] = true;
        }
        if (SaveData.unlockedUpgrades[EnumManager.Upgrades.Inventory1])
        {
            inventorySize = 5;
            unlockedSlots[4] = true;
        }

    }
    
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddPotion(GameObject g)
    {
        for(int i = 0; i< inventorySize; i++)
        {
            if (!filledSlots[i])
            {
                filledSlots[i] = true;
                g.transform.parent = potionSlots[i];
                g.transform.localPosition = Vector3.zero;
                g.transform.localRotation = Quaternion.identity;
                g.GetComponent<Potion>().inventoryPosition = i;
                potionsInInventory.Add(g.GetComponent<Potion>());
                numPotions++;
                break;
                
            }
        }
    }

    public void RemoveUsedPotion(Potion p)
    {
        filledSlots[p.inventoryPosition] = false;
        potionsInInventory.Remove(p);
        numPotions--;
    }

    public void DisablePotionUI(Potion potion)
    {
        foreach (Potion p in potionsInInventory)
        {
            if (p != potion)
            {
                p.UIEnabled = false;
            }
        }
    }

    public void EnablePotionUI()
    {
        foreach (Potion p in potionsInInventory)
        {
            p.UIEnabled = true;
        }
    }

    public bool IsInventoryFull()
    {
        //Debug.Log("Inventory full? " + (numPotions / inventorySize == 1) + " num Potions: " + numPotions + " Inventory size);
        return numPotions / inventorySize == 1;
    }

    
}
