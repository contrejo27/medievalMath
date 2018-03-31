using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameDataEditor;

#if GDE_PLAYMAKER_SUPPORT && PLAYMAKER_1_8

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(GDMConstants.ActionCategory)]
    [Tooltip(GDMConstants.SetColorArrayActionTooltip)]
    public class GDESetColorArray : GDESetActionBase
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
                List<Color> vals = Variable.Values != null ? Variable.Values.Cast<Color>().ToList() : null;
		GDEDataManager.SetColorList(ItemName.Value, FieldName.Value, vals);
	    }
	    catch(UnityException ex)
	    {
		LogError(string.Format(GDMConstants.ErrorSettingValue, GDMConstants.ColorType, ItemName.Value, FieldName.Value));
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


