using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterShotPotion : Potion {

    protected override void Init()
    {
        toolTip.text = "Scatter Shot Potion";
        base.Init();
    }

    public override void DoEffect()
    {
        GameStateManager.instance.player.AddModifier(ArrowModifier.Spread, 0);
        base.DoEffect();
    }
}
