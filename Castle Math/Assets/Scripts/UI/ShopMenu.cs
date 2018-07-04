﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour {

    string moneyFormat = "0.##";

    public Transform potionPoint;
    List<Potion> potionsInShop;
    float potionPrice;

    public Text potionPriceText;
    public Text potionName;
    //public Text[] gemValues = new Text[5];
    public Text[] gemTotalsInShop = new Text[5];
    public Text[] gemsInShop = new Text[5];
    public Text[] gemsAvailable = new Text[5];

    float finalTotal;
    public Text finalTotalText;

    GameObject potionDisplay;

    #region dictionaries
    Dictionary<EnumManager.GemType, int> gemToArray
        = new Dictionary<EnumManager.GemType, int>()
        {
            {EnumManager.GemType.Penny, 0 },
            {EnumManager.GemType.Nickel, 1 },
            {EnumManager.GemType.Dime, 2 },
            {EnumManager.GemType.Quarter, 3 },
            {EnumManager.GemType.Dollar, 4 }
        };

    Dictionary<int, EnumManager.GemType> arrayToGem
        = new Dictionary<int, EnumManager.GemType>()
        {
            {0,EnumManager.GemType.Penny },
            {1,EnumManager.GemType.Nickel },
            {2,EnumManager.GemType.Dime },
            {3,EnumManager.GemType.Quarter },
            {4,EnumManager.GemType.Dollar }
        };

    Dictionary<EnumManager.GemType, float> dGemTotalsInShop 
        = new Dictionary<EnumManager.GemType, float>()
        {
            {EnumManager.GemType.Penny, 0 },
            {EnumManager.GemType.Nickel, 0 },
            {EnumManager.GemType.Dime, 0 },
            {EnumManager.GemType.Quarter, 0 },
            {EnumManager.GemType.Dollar, 0 }
        };

    public Dictionary<EnumManager.GemType, int> dGemsInShop
        = new Dictionary<EnumManager.GemType, int>()
        {
            {EnumManager.GemType.Penny, 0 },
            {EnumManager.GemType.Nickel, 0 },
            {EnumManager.GemType.Dime, 0 },
            {EnumManager.GemType.Quarter, 0 },
            {EnumManager.GemType.Dollar, 0 }
        };

    public Dictionary<EnumManager.GemType, int> dGemsAvailable
        = new Dictionary<EnumManager.GemType, int>()
        {
            {EnumManager.GemType.Penny, 0 },
            {EnumManager.GemType.Nickel, 0 },
            {EnumManager.GemType.Dime, 0 },
            {EnumManager.GemType.Quarter, 0 },
            {EnumManager.GemType.Dollar, 0 }
        };
    #endregion

    bool isAwake;
   
    void Awake()
    {
        /*
        gemValues[0].text = FormatMoneyString(EnumManager.gemValues[EnumManager.GemType.Penny]);
        gemValues[1].text = FormatMoneyString(EnumManager.gemValues[EnumManager.GemType.Nickel]);
        gemValues[2].text = FormatMoneyString(EnumManager.gemValues[EnumManager.GemType.Dime]);
        gemValues[3].text = FormatMoneyString(EnumManager.gemValues[EnumManager.GemType.Quarter]);
        gemValues[4].text = FormatMoneyString(EnumManager.gemValues[EnumManager.GemType.Dollar]);
        */
    }

	void OnEnable()
    {
        if (isAwake)
        {
            foreach(EnumManager.GemType type in dGemsAvailable.Keys)
                dGemsAvailable[type] = GameStateManager.instance.levelManager.gemsOwned[type];
        }
        else isAwake = true;
    }

    public void LoadPotions(List<Potion> potionsToBuy)
    {
        potionsInShop = new List<Potion>(potionsToBuy);
    }

    public void AddGemToShop(EnumManager.GemType type)
    {
        --dGemsAvailable[type];
        ++dGemsInShop[type];
        
        dGemTotalsInShop[type] += EnumManager.gemValues[type];
        finalTotal += EnumManager.gemValues[type];

        UpdateShopNumericalText(type);
    }

    public void RemoveGemFromShop(EnumManager.GemType type)
    {
        ++dGemsAvailable[type];
        --dGemsInShop[type];

        dGemTotalsInShop[type] -= EnumManager.gemValues[type];
        finalTotal -= EnumManager.gemValues[type];

        UpdateShopNumericalText(type);
    }

    void UpdateShopNumericalText(EnumManager.GemType type)
    {
        gemsAvailable[gemToArray[type]].text = dGemsAvailable[type].ToString();
        gemsInShop[gemToArray[type]].text = "x" + dGemsInShop[type].ToString();


        gemTotalsInShop[gemToArray[type]].text = FormatMoneyString(dGemTotalsInShop[type]);
        finalTotalText.text = FormatMoneyString(finalTotal);
    }

    string FormatMoneyString(float cost)
    {
        return cost.ToString(moneyFormat);
    }

    public void CheckPurchase()
    {
        if (finalTotal >= potionPrice)
            DoCompletePurchase();
        else
            DoIncompletePutchase();
    }

    public void DoCompletePurchase()
    {
        float priceTemp = potionPrice;
        float remainder = 0;

        for(int i = 4; i>=0; i--)
        {
            while(dGemsInShop[arrayToGem[i]] > 0 && priceTemp > 0)
            {
                dGemsInShop[arrayToGem[i]]--;
                if(priceTemp - EnumManager.gemValues[arrayToGem[i]] < 0)
                {
                    priceTemp = 0;
                    remainder = priceTemp - EnumManager.gemValues[arrayToGem[i]];

                }
                else
                {
                    priceTemp -= EnumManager.gemValues[arrayToGem[i]];
                }
                GameStateManager.instance.levelManager.RemoveGems(1, arrayToGem[i]);
            }
            if(priceTemp <= 0)
            {
                for(int j = i; j >=0; j--)
                {
                    while(remainder - EnumManager.gemValues[arrayToGem[j]] >= 0)
                    {
                        remainder -= EnumManager.gemValues[arrayToGem[j]];
                        GameStateManager.instance.levelManager.RecieveGems(1, arrayToGem[j]);
                    }
                    if(remainder <= 0)
                    {
                        break;
                    }
                }
                break;
            }
        }
        // Then create a spawner that shows the change returning


        //while(dGemsInShop[EnumManager.])
        /*
        foreach(KeyValuePair<EnumManager.GemType, int> pair in dGemsInShop)
        {
            //GameStateManager.instance.levelManager.
        }
        */
    }

    public void DoIncompletePutchase()
    {
        // a poppup maybe?
    }
}