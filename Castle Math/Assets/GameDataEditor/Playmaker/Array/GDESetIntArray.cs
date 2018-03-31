using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using GameDataEditor;

#if GDE_PLAYMAKER_SUPPORT && PLAYMAKER_1_8

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(GDMConstants.ActionCategory)]
    [Tooltip(GDMConstants.SetIntArrayActionTooltip)]
    public class GDESetIntArray : GDESetActionBase
    {   
	[UIHint(UIHint.Variable)]
	public FsmArray Variable;
		
	public override void Reset()
	{
	    base.Reset();
			
	    if (Variable != null)
		Variable.Reset();
	}
		
	public override void OnEnter()
	{
            base.OnEnter();
            
	    try
	    {
		List<int> vals = null;
		
		if (Variable.Values != null)
		    vals = Variable.Values.ToList().ConvertAll(obj => Convert.ToInt32(obj));

		GDEDataManager.SetIntList(ItemName.Value, FieldName.Value, vals);
	    }
	    catch(UnityException ex)
	    {
		LogError(string.Format(GDMConstants.ErrorSettingValue, GDMConstants.IntType, ItemName.Value, FieldName.Value));
		LogError(ex.ToString());
	    }
	    finally
	    {
		Finish();
	    }
	}
    }
}

#endif

