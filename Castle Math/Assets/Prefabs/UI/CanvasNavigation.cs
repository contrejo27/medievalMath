using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
                 
public abstract class CanvasNavigation : MonoBehaviour
{
#pragma warning disable
    [Header("Navigation")]
    [SerializeField] private Button backButton;
    [SerializeField] GameObject previousCanvas;
    [SerializeField] GameObject nextCanvas;
#pragma warning restore

    protected virtual void Awake()
    {
        if (backButton) backButton.onClick.AddListener(BackPressed);
    }

    protected virtual void BackPressed()
    {
        if (previousCanvas) Instantiate(previousCanvas);
        Destroy(this.gameObject);
    }

    protected virtual void GoToNextCanvas(GameObject nextCanvasOverride = null)
    {
        if (nextCanvasOverride) Instantiate(nextCanvasOverride);
        else if (nextCanvas) Instantiate(nextCanvas);
        Destroy(this.gameObject);
    }
}
