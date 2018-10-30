using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTimePotion : Potion {

    protected override void Init()
    {
        toolTip.text = "Freeze Time Potion";
        toolTipShadow.text = toolTip.text;
        base.Init();
    }

    public override void DoEffect()
    {
        GameStateManager.instance.levelManager.SlowAllEnemeies(0,duration);
        base.DoEffect();
    }
}
