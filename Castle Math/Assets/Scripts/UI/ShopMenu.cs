using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour {

    string moneyFormat = "0.##";

    public Transform potionPoint;
    List<Potion> potionsInShopMenu;
    List<Vector3> originalPotionPositions;
    float potionPrice;

    public Text potionPriceText;
    public Text potionName;
    public Text purchaseFeedback;
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
        
        
    }

	void OnEnable()
    {
        //if (isAwake)
        //{
        // so this is kind of messy, but apparently you can't change dictionaries while you're iterating throguh them
        // (even if you're not adding/removing keys
        // So we're using the keys from another dictionary since they're the same.
        // Generally best practise would be to iterate through the enum, but that's more complex, and isn't suuuper

        foreach (EnumManager.GemType type in gemToArray.Keys)
        {
            dGemsAvailable[type] = GameStateManager.instance.levelManager.gemsOwned[type];
            gemsAvailable[gemToArray[type]].text = dGemsAvailable[type].ToString();
        }
        //}
        //else isAwake = true;
    }

    public void LoadPotions(List<Potion> potionsToBuy)
    {
        potionsInShopMenu = new List<Potion>(potionsToBuy);
        float potionSize = 1;
        float potionSpacing = .3f;
        Vector3 PotionStartPoint = new Vector3(potionPoint.position.x - (potionSpacing + potionSize) * (potionsInShopMenu.Count - 1) / 2, potionPoint.position.y, potionPoint.position.z);  
        for(int i = 0; i< potionsInShopMenu.Count; i++)
        {
            potionsInShopMenu[i].transform.position = PotionStartPoint + new Vector3(i*(potionSize+potionSpacing),0,0);
            potionsInShopMenu[i].OnToShopMenu();
            potionPrice += potionsInShopMenu[i].cost;
        }

        potionPriceText.text = "Cost: " + FormatMoneyString(potionPrice);
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
            DoIncompletePurchase();
    }

    public void DoCompletePurchase()
    {
        Debug.Log("Complete purchase");

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

        foreach(Potion p in potionsInShopMenu)
        {
            p.Purchase();
        }

        WipePotions(false);
        gameObject.SetActive(false);
        // Then create a spawner that shows the change returning


        //while(dGemsInShop[EnumManager.])
        /*
        foreach(KeyValuePair<EnumManager.GemType, int> pair in dGemsInShop)
        {
            //GameStateManager.instance.levelManager.
        }
        */
    }

    public void OnCancel()
    {
        WipePotions();
        gameObject.SetActive(false);
    }

    void WipePotions(bool returnToShop = true)
    {
        while (potionsInShopMenu.Count > 0)
        {
            if (returnToShop)
                potionsInShopMenu[0].transform.localPosition = new Vector3(0, 0, 0);

            potionsInShopMenu.RemoveAt(0);
        }
    }

    public void DoIncompletePurchase()
    {
        Debug.Log("Incomplete Purchase");
        // a poppup maybe?
    }

    public void OnOverSubmitButton()
    {
        PurchaseHoverFeedback();
    }

    public void OnExitSubmitButton()
    {
        purchaseFeedback.gameObject.SetActive(false);
    }

    public void PurchaseHoverFeedback()
    {
        purchaseFeedback.gameObject.SetActive(true);

        if (finalTotal == potionPrice)
            purchaseFeedback.text = "Exact change!";

        else if (finalTotal > potionPrice)
            purchaseFeedback.text = "Your change will be: " + (finalTotal - potionPrice).ToString(moneyFormat);

        else
            purchaseFeedback.text = "Insufficient funds offered";
    }
}
