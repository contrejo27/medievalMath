using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTimePotion : Potion {

    protected override void Init()
    {
        toolTip.text = "Slow Time Potion";
        base.Init();
    }

    public override void DoEffect()
    {
        GameStateManager.instance.SetTimeScale(.50f, 8f);
        base.DoEffect();
    }
}
