using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceQuiver : BaseQuiver {

    protected override void Init()
    {
        arrowModifier = ArrowModifier.Ice;
        base.Init();
    }
}
