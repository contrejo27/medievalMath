using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpEffects : MonoBehaviour {
	public Animator Anim; 
	void Start(){
		Anim = GetComponent<Animator> ();
				print("Anim");

	}
    void OnEnable() {
		Anim.Play("ui_anim");
    }
}
