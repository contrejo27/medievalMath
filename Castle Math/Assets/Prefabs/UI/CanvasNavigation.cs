using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
                 
public abstract class CanvasNavigation : MonoBehaviour
{
#pragma warning disable
    [Header("Navigation")]
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject nextCanvas;
	[SerializeField] private GameObject previousCanvas = null;

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

	protected virtual void GoToNextCanvas(GameObject nextCanvasOverride = null, bool destroyPrevious = true)
    {
		Debug.Log ("going to next canvs");
		GameObject canvasToSpawn = null;

		if (nextCanvasOverride)
			canvasToSpawn = nextCanvasOverride;
		else if (nextCanvas) 
			canvasToSpawn = nextCanvas;

		if (canvasToSpawn) 
			canvasToSpawn = Instantiate (canvasToSpawn);

		if(destroyPrevious)
        	Destroy(this.gameObject);
    }
}
