using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    public void Retry ()
    {
        GameStateManager.instance.Retry();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
