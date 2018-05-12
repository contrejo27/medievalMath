using System.Collections;
using UnityEngine;
using UnityEngine.VR;

public class VRToggle : MonoBehaviour
{

    void Update() {

        if (Input.GetMouseButtonDown (0)) {
            ToggleVR ();
        }
    }

    void ToggleVR() {

       /* if (VRSettings.loadedDeviceName == "cardboard") {
            StartCoroutine(LoadDevice("None"));
        } else {
            StartCoroutine(LoadDevice("cardboard"));
        }*/
    }

    IEnumerator LoadDevice(string newDevice)
    {
        UnityEngine.VR.VRSettings.LoadDeviceByName(newDevice);
        yield return null;
        UnityEngine.VR.VRSettings.enabled = true;
    }
}