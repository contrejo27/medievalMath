using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPotion : Potion {

    public bool doesExplode;
    
	protected override void Init()
    {
        toolTip.text = doesExplode ? "Double Agent Potion" : "Scarecrow Potion";
        toolTipShadow.text = toolTip.text;
        base.Init();
    }
	
	public override void DoEffect()
    {
        GameStateManager.instance.levelManager.SetDummyEnemies(duration, doesExplode);
        base.DoEffect();
    }
}
