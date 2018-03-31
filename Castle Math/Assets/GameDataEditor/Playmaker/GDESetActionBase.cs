using UnityEngine;
using System.Collections.Generic;
using GameDataEditor;

#if GDE_PLAYMAKER_SUPPORT

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(GDMConstants.ActionCategory)]
    public abstract class GDESetActionBase : GDEActionBase
    {
        [UIHint(UIHint.FsmString)]
        [Tooltip(GDMConstants.SchemaTooltip)]
        public FsmString SchemaName;

        public override void Reset()
        {
            SchemaName = null;
        }
        
        public override void OnEnter()
        {
            if (!string.IsNullOrEmpty(SchemaName.Value) && !string.IsNullOrEmpty(ItemName.Value))
                GDEDataManager.RegisterItem(SchemaName.Value, ItemName.Value);
        }
    }
}

#endif
