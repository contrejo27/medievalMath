using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionShop : MonoBehaviour {
    // To be enabled every 5 waves in waveManager; 
    // This mostly just manages stocking the shop; payment is handled through the
    // potion/playercontroller scripts

    public Transform[] shopSlots;
    public GameObject[] availablePotions;
    [HideInInspector]
    public List<Potion> potionsInShop = new List<Potion>();

    void Awake()
    {
        // Moving to Start() for now 
        // GameStateManager.instance.potionShop = this;
        // fill availablePotions with unlocked potions. 
        // (for now we'll do this manually)
    }

    void Start()
    {
        GameStateManager.instance.potionShop = this;
        gameObject.SetActive(false);
    }

    // May casue issue in the future; see if this happens before Awake()/Start()
    void OnEnable()
    {
        LoadPotions();   
    }

    void LoadPotions()
    {
        // load potions into slots
        // random for now
        // (consider avoiding dupes in the future)
        foreach (Transform t in shopSlots)
        {
            GameObject g = Instantiate(availablePotions[Random.Range(0, availablePotions.Length)], Vector3.zero, Quaternion.identity, t) as GameObject;
            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
            potionsInShop.Add(g.GetComponent<Potion>());


        }
    }

    void OnDisable()
    {
        foreach(Potion p in potionsInShop)
        {
            Destroy(p.gameObject);
        }
        potionsInShop.Clear();
    }

    public void RemoveFromShop(Potion p)
    {
        potionsInShop.Remove(p);
    }
	
    public void DisablePotionUI(Potion potion)
    {
        foreach(Potion p in potionsInShop)
        {
            if (p != potion)
            {
                p.UIEnabled = false;
            }
        }
    }

    public void EnablePotionUI()
    {
        foreach(Potion p in potionsInShop)
        {
            p.UIEnabled = true;
        }
    }

    public void DeactivateShop()
    {
        GameStateManager.instance.waveManager.NextWave();
        GameStateManager.instance.player.SetLookingAtInterface(false);
        gameObject.SetActive(false);
    }

    public void OnPointerEnter()
    {
        GameStateManager.instance.player.SetLookingAtInterface(true);
    }

    public void OnPointerExit()
    {
        GameStateManager.instance.player.SetLookingAtInterface(false);
    }
}
