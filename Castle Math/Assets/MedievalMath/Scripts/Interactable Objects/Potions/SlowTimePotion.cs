﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTimePotion : Potion {

	private GameObject EffectImage;

    protected override void Init()
    {
        toolTip.text = "Slow Time Potion";
        toolTipShadow.text = toolTip.text;
        base.Init();
    }

    public override void DoEffect()
    {
        GameStateManager.instance.SetTimeScale(.50f, 8f);
		StartCoroutine (SlowTimeCD ());
        base.DoEffect();
    }

	IEnumerator SlowTimeCD() {
		yield return new WaitForSeconds (8f);

        EffectImage = GameObject.Find ("EffectImage");

        EffectImage.GetComponent<SpriteRenderer> ().sprite = null;
	}
}