using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseInteractableObject : MonoBehaviour {
    PlayerController playerController;
    EventTrigger eventTrigger;

    private MaterialPropertyBlock mpb;

    protected Material outlineMaterial;
    List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    List<Material[]> materials = new List<Material[]>();

    [HideInInspector]
    public bool isHighlighted = false;

    // Use this for initialization
    void Start () {
        Init();	
	}

    protected virtual void Init() {
        outlineMaterial = Resources.Load<Material>("Materials/Outline");
        mpb = new MaterialPropertyBlock();
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderers.Add(mr);
            materials.Add(mr.materials);

        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void OnPassOver()
    {
        GameStateManager.instance.player.SetLookingAtInterface(true);
        //Debug.Log("passing over " + name);
    }

    public virtual void OnEndPassOver()
    {
        GameStateManager.instance.player.SetLookingAtInterface(false);
        //Debug.Log("end pass over " + name);
    }

    public virtual void OnInteract()
    {
        //playerController = eventTrigger.
        //Debug.Log("Interact with " + name);
    }

    protected void SetHighlight(Color c)
    {
        foreach (MeshRenderer mr in meshRenderers)
        {
            Material[] mBackup = new Material[mr.materials.Length];
            Material[] ms = new Material[mr.materials.Length];
            for (int i = 0; i < mr.materials.Length; i++)
            {
                mBackup[i] = mr.materials[i];
                ms[i] = outlineMaterial;
            }
            mr.materials = ms;
            for (int i = 0; i < mr.materials.Length; i++)
            {
                //mr.GetPropertyBlock(mpb);
                if (mBackup[i].HasProperty("_Color"))
                    mr.materials[i].SetColor("_Color", mBackup[i].color);
                else
                    mr.materials[i].SetColor("_Color", Color.white);
                mr.materials[i].SetColor("_OutlineColor", c);
                //mpb.SetColor("_Color", mBackup[i].color);
                //mpb.SetColor("_OutlineColor", c);
                if (mBackup[i].mainTexture != null)
                {
                    //mpb.SetTexture("_MainTex", mBackup[i].mainTexture);
                    mr.materials[i].SetTexture("_MainTex", mBackup[i].mainTexture);
                    mr.materials[i].SetTextureOffset("_MainTex", mBackup[i].GetTextureOffset("_MainTex"));
                }
                //mr.SetPropertyBlock(mpb);
                /*
                Debug.Log("Set material to outline: " + mr.materials[i].name +", from: " + mBackup[i].name);
                Debug.Log("Set Color " + outlineMaterial.color.ToString() + " to " + mBackup[i].color.ToString() + "(" + mr.materials[i].color + ")");
                Debug.Log("Boop");
                */
            }
        }
        isHighlighted = true;
    }

    protected void RemoveHighlight()
    {
        int i = 0;
        foreach (MeshRenderer mr in meshRenderers)
        {
            mr.materials = materials[i];
            i++;
            Debug.Log("Removed outline");

        }
        isHighlighted = false;
    }


}
