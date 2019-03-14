using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderRecovery : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        foreach (Renderer r in GameObject.FindObjectsOfType<Renderer>())
        {
            Material mat = new Material(Shader.Find(r.material.shader.name));

            mat.CopyPropertiesFromMaterial(r.material);
            if(r.name != "GvrReticlePointer")
                r.material = mat;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
