using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameDataEditor;

#if GDE_PLAYMAKER_SUPPORT && PLAYMAKER_1_8

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(GDMConstants.ActionCategory)]
    [Tooltip(GDMConstants.SetCustomVec2ArrayActionTooltip)]
    public class GDESetCustomVector2Array : GDESetActionBase
    {   
	[UIHint(UIHint.FsmString)]
	[Tooltip(GDMConstants.Vec2ListCustomFieldTooltip)]
	public FsmString CustomField;
		
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
		Dictionary<string, object> data;
		string customKey;
				
		if (GDEDataManager.DataDictionary.ContainsKey(ItemName.Value))
		{
		    GDEDataManager.Get(ItemName.Value, out data);
		    data.TryGetString(FieldName.Value, out customKey);
		    customKey = GDEDataManager.GetString(ItemName.Value, FieldName.Value, customKey);
		}
		else
		{
		    // New Item Case
		    customKey = GDEDataManager.GetString(ItemName.Value, FieldName.Value, string.Empty);
		}

		List<Vector2> vals = Variable.Values != null ? Variable.Values.Cast<Vector2>().ToList() : null;
		GDEDataManager.SetVector2List(customKey, CustomField.Value, vals);
	    }
	    catch(UnityException ex)
	    {
		LogError(string.Format(GDMConstants.ErrorSettingCustomValue, GDMConstants.Vec2Type, ItemName.Value, FieldName.Value, CustomField.Value));
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
