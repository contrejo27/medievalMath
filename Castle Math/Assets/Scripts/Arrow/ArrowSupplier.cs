using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSupplier : MonoBehaviour {

	public int defaultNumberToSpawn;

	public AudioClip spawnSound;

	public int NumberOfArrows;

	public GameObject[] ArrowToSpawn;

	public GameObject[] Arrows;
	public List<int> ArrowIndex;
	public AudioSource uiSound;

	private int AmountToSpawn;

	// Use this for initialization
	void Awake () {
		Arrows = new GameObject[0];
		CreateArrow ();
    }


    public void CreateArrow()
	{
		//NumberOfArrows += NumberToSpawn;
		uiSound.clip = spawnSound;
		uiSound.Play ();
		StartCoroutine (DelaySpawn (0, defaultNumberToSpawn));
	}

	public void CreateArrowIntermath(int numArrowsToSpawn)
	{
		//NumberOfArrows += NumberToSpawn;
		uiSound.clip = spawnSound;
		uiSound.Play ();
		StartCoroutine (DelaySpawnIntermath (0, numArrowsToSpawn));
	}

	IEnumerator DelaySpawnIntermath(int Index, int numArrowsToSpawn)
	{
		GroupResize (NumberOfArrows+numArrowsToSpawn*2,ref Arrows);


		for (int i = 0; i < numArrowsToSpawn; i++) {
			GameObject newArrow = Instantiate (ArrowToSpawn[Index], transform.position, transform.rotation);


			Arrows [i + (Arrows.Length-numArrowsToSpawn)] = newArrow;
			ArrowIndex.Add (Index);
			NumberOfArrows++;

			yield return new WaitForSeconds (.1f);
			if(i%4 == 0){
				uiSound.clip = spawnSound;
				uiSound.Play ();
			}
			Destroy(newArrow,.8f);
		}
	}

	IEnumerator DelaySpawn(int Index, int numArrowsToSpawn)
	{
		GroupResize (NumberOfArrows+numArrowsToSpawn,ref Arrows);


		for (int i = 0; i < numArrowsToSpawn; i++) {
			GameObject newArrow = Instantiate (ArrowToSpawn[Index], transform.position, transform.rotation);


			Arrows [i + (Arrows.Length-numArrowsToSpawn)] = newArrow;
			ArrowIndex.Add (Index);
			NumberOfArrows++;

			yield return new WaitForSeconds (.1f);
			Destroy(newArrow,.8f);
		}
	}

	public void UseArrow()
	{
		if (NumberOfArrows > 0 && GameStateManager.instance.currentState ==EnumManager.GameState.Wave) {
			NumberOfArrows -= 1;
/*
			GameObject destroyObject = Arrows [NumberOfArrows].gameObject;
			ArrowIndex.RemoveAt (NumberOfArrows);


			Destroy (destroyObject);*/
		}
	}


	public void GroupResize (int Size, ref GameObject[] Group)
	{

		GameObject[] temp = new GameObject[Size];
		for (int c = 0; c < Mathf.Min(Size, Group.Length); c++ ) {
			temp [c] = Group [c];
		}
		Group = temp;
	}


}
