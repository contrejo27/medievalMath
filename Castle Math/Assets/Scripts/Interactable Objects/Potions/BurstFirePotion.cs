using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstFirePotion : Potion
{
    protected override void Init()
    {
        toolTip.text = "Burst Fire Potion";
        toolTipShadow.text = toolTip.text;
        base.Init();
    }
    public override void DoEffect()
    {
        GameStateManager.instance.player.AddModifier(ArrowModifier.Burst, 0);
        base.DoEffect();
    }
}
