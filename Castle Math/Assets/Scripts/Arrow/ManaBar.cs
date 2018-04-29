using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour {
	public Text perk;
	public float NumberOfQuestions;
	public GameObject manaBarEnd;
	public CanvasGroup mathCanvas;
	public hudManager hud;
	public doorHealth healthLeft;
	public doorHealth healthMid;
	public doorHealth healthRight;
    public GameStateManager gManager;

    private float CurrentNumber;
	private LaunchProjectile ProjectileLauncher;
	
	private int powerUpCount = 0;

	private AudioSource A_Source;
	public AudioClip PowerUpSound;

    // Use this for initialization
    void Start () {
		A_Source = GameObject.Find ("UIAudio").GetComponent<AudioSource> ();
		ProjectileLauncher = FindObjectOfType<LaunchProjectile> ();
        if (PlayerPrefs.GetInt("LoggedIn") == 0)
        {
            NumberOfQuestions = 10;
        }

    }

    public void ClearPowerUp(int PowerupIndex)
	{
		 //[PowerupIndex].SetActive (false);
	}

    public void UpgradeLevel(int levelsUp){
        gManager.currentSkillLevel+= levelsUp;
    }

	public void CorrectAnswer()
	{
		CurrentNumber += 1f;

		//perk text name should match icon sprite name
		if (CurrentNumber >= NumberOfQuestions) {
			CurrentNumber = 0f;
            ArrowModifier newMod;

            if (PlayerPrefs.GetInt("LoggedIn") == 0)
            {
                newMod = ArrowModifier.Spread;
                perk.text = "Spread";
                ProjectileLauncher.AddModifier(newMod, powerUpCount);

                mathCanvas.alpha = 0.0f;
                hud.AddPoweUpIcon(perk.text);

                //give player perk
                StartCoroutine(erasePerkText());
                A_Source.clip = PowerUpSound;
                A_Source.Play();
            }
            else
            {
                int RanMod = UnityEngine.Random.Range(0, gManager.currentSkillLevel);

                if (RanMod == 0){
                    newMod = ArrowModifier.Burst;
                    perk.text = "Burst";
                    ProjectileLauncher.AddModifier(newMod, powerUpCount);
                }
                else if (RanMod == 1){
                    newMod = ArrowModifier.Spread;
                    perk.text = "Spread";
                    ProjectileLauncher.AddModifier(newMod, powerUpCount);
                }
                else if (RanMod == 2){
                    newMod = ArrowModifier.Bomb;
                    perk.text = "Bomb";
                    ProjectileLauncher.AddModifier(newMod, powerUpCount);
                }
                else if (RanMod == 3){
                    perk.text = "Health";
                    healthMid.UpdateHealth(50);
                    healthLeft.UpdateHealth(50);
                    healthRight.UpdateHealth(50);
                }
                else{
                    healthMid.InvinciblePowerUp();
                    healthLeft.InvinciblePowerUp();
                    healthRight.InvinciblePowerUp();
                    perk.text = "Invincible";
                }
            }

            mathCanvas.alpha = 0.0f;
			hud.AddPoweUpIcon(perk.text);

			//give player perk
			StartCoroutine(erasePerkText());
			A_Source.clip = PowerUpSound;
			A_Source.Play ();
		}

		adjustManaBar();
	}

	public void IncorrectAnswer(){
		if(CurrentNumber > 0f){
			CurrentNumber -= 1f;
		}
		adjustManaBar();
	}

	void adjustManaBar(){
		float percent = CurrentNumber / NumberOfQuestions;
		transform.localScale = Vector3.Lerp (new Vector3 (.05f, .5f, 1f), new Vector3 (.8f, .5f, 1f), percent);
		//manaBarEnd.transform.position = ;

	}
	IEnumerator erasePerkText()
	{
		yield return new WaitForSeconds (3f);
		mathCanvas.alpha = 1.0f;
		perk.text = "";

	}
}
