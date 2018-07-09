using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickShotPotion : Potion {

	protected override void Init () {
        toolTip.text = "Quickshot Potion";
        base.Init();
	}

    public override void DoEffect()
    {
        GameStateManager.instance.player.SetQuickShot(12);
        base.DoEffect();
    }
}
