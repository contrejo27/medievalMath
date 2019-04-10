using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrlLinker : MonoBehaviour
{
    // Start is called before the first frame update
    public void goToURL(string url)
    {
        Application.OpenURL(url);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
