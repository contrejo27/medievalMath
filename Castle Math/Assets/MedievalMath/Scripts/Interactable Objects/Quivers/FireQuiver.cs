using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireQuiver : BaseQuiver {

    protected override void Init()
    {
        arrowModifier = ArrowModifier.Fire;
        base.Init();
        
    }

}
