using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFirePotion : Potion {

    protected override void Init()
    {
        toolTip.text = "Rapidfire Potion";
        toolTipShadow.text = toolTip.text;
        base.Init();
    }

    public override void DoEffect()
    {
        GameStateManager.instance.player.SetRapidFire(10);
        base.DoEffect();
    }
}
