using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgeBlock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CheckAgeBlock();
    }

    public void CheckAgeBlock()
    {
        if (PlayerPrefs.GetInt("ageRestricted") == 0)
        {
            gameObject.SetActive(false);
        }
    }

}
