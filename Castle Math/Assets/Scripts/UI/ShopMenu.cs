using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour {

    string moneyFormat = "0.##";

    public Transform potionPoint;
    float potionPrice;

    public Text potionPriceText;
    public Text potionName;
    public Text[] gemValues = new Text[5];
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
            {EnumManager.GemType.Red, 0 },
            {EnumManager.GemType.Yellow, 1 },
            {EnumManager.GemType.Purple, 2 },
            {EnumManager.GemType.Cyan, 3 },
            {EnumManager.GemType.Green, 4 }
        };

    Dictionary<EnumManager.GemType, float> dGemTotalsInShop 
        = new Dictionary<EnumManager.GemType, float>()
        {
            {EnumManager.GemType.Red, 0 },
            {EnumManager.GemType.Yellow, 0 },
            {EnumManager.GemType.Purple, 0 },
            {EnumManager.GemType.Cyan, 0 },
            {EnumManager.GemType.Green, 0 }
        };

    Dictionary<EnumManager.GemType, int> dGemsInShop
        = new Dictionary<EnumManager.GemType, int>()
        {
            {EnumManager.GemType.Red, 0 },
            {EnumManager.GemType.Yellow, 0 },
            {EnumManager.GemType.Purple, 0 },
            {EnumManager.GemType.Cyan, 0 },
            {EnumManager.GemType.Green, 0 }
        };

    Dictionary<EnumManager.GemType, int> dGemsAvailable
        = new Dictionary<EnumManager.GemType, int>()
        {
            {EnumManager.GemType.Red, 0 },
            {EnumManager.GemType.Yellow, 0 },
            {EnumManager.GemType.Purple, 0 },
            {EnumManager.GemType.Cyan, 0 },
            {EnumManager.GemType.Green, 0 }
        };
    #endregion

    bool isAwake;
   
    void Awake()
    {
        gemValues[0].text = FormatMoneyString(EnumManager.gemValues[EnumManager.GemType.Red]);
        gemValues[1].text = FormatMoneyString(EnumManager.gemValues[EnumManager.GemType.Yellow]);
        gemValues[2].text = FormatMoneyString(EnumManager.gemValues[EnumManager.GemType.Purple]);
        gemValues[3].text = FormatMoneyString(EnumManager.gemValues[EnumManager.GemType.Cyan]);
        gemValues[4].text = FormatMoneyString(EnumManager.gemValues[EnumManager.GemType.Green]);
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

    void LoadPotion(Potion potionToBuy)
    {

    }

    void AddGemToShop(EnumManager.GemType type)
    {
        --dGemsAvailable[type];
        ++dGemsInShop[type];
        
        dGemTotalsInShop[type] += EnumManager.gemValues[type];
        finalTotal += EnumManager.gemValues[type];

        UpdateShopNumericalText(type);
    }

    void RemoveGemFromShop(EnumManager.GemType type)
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
}
