using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTimePotion : Potion {

    protected override void Init()
    {
        toolTip.text = "Snail Time Potion";
        toolTipShadow.text = toolTip.text;
        base.Init();
    }

    public override void DoEffect()
    {
        GameStateManager.instance.levelManager.SlowAllEnemeies(.93f,duration*3);
        base.DoEffect();
    }
}
