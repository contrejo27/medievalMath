using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

public class MenuManager : MonoBehaviour {

	// Use this for initialization
	public void Start () {
		//StartCoroutine(ActivatorVR("Cardboard"));
	}
	
	public IEnumerator ActivatorVR(string vrToggle){
		VRSettings.LoadDeviceByName(vrToggle);
		yield return null;
		VRSettings.enabled = true;
		yield return new WaitForSeconds(.1f);
		SceneManager.LoadScene (1);
	}

	public void loadGame () {
		StartCoroutine(ActivatorVR("Cardboard"));
	}

	public void Quit()
	{
		Application.Quit ();
	}

}
