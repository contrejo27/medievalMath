using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GemUIEffect : MonoBehaviour {
    public Sprite greenGem;
	public void unlockGem ()
    {
        Image gemImage = gameObject.GetComponent<Image>();
        gemImage.sprite = greenGem;
    }
}
