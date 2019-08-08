using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class debugText : MonoBehaviour
{
    public Text dText;
    public bool enableDebugText;

    // Singleton
    public static debugText instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddDebugText(string text)
    {
        if (enableDebugText) dText.text += text;
    }
}