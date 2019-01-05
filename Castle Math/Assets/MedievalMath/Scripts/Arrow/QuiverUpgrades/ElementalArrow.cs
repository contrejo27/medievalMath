using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalArrow : ArrowClass {
    public enum ElementalArrowModifier { UpgradeOne, UpgradeTwoA, UpgradeTwoB }
    protected ElementalArrowModifier currentUpgradeLevel;

    public BaseQuiver quiver;

    // Use this for initialization
    void Start () {
		
	}

    public override void ArrowLaunched()
    {
        quiver.StartCooldown();
        base.ArrowLaunched();
    }

    void OnDestroy()
    {

    }

    // Update is called once per frame
    void Update () {
		
	}
}
