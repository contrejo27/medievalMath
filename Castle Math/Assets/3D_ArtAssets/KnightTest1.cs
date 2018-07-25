using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightTest1 : MonoBehaviour {

	public EnemyBehavior [] knight;

	void Start () 
	{
		foreach(EnemyBehavior k in knight)
		k.SetTarget (transform);
	}
	
	void Update () 
	{
		
	}
}
