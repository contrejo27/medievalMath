using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockQuiver : BaseQuiver {

    protected override void Init()
    {
        arrowModifier = ArrowModifier.Shock;
        base.Init();
    }

}
