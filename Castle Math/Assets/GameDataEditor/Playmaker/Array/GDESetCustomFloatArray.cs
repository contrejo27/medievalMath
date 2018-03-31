using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using GameDataEditor;

#if GDE_PLAYMAKER_SUPPORT && PLAYMAKER_1_8

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(GDMConstants.ActionCategory)]
    [Tooltip(GDMConstants.SetCustomFloatArrayActionTooltip)]
    public class GDESetCustomFloatArray : GDESetActionBase
    {   
	[UIHint(UIHint.FsmString)]
	[Tooltip(GDMConstants.FloatListCustomFieldTooltip)]
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

		List<float> vals = null;
		
		if (Variable.Values != null)
		    vals = Variable.Values.ToList().ConvertAll(obj => Convert.ToSingle(obj));

		GDEDataManager.SetFloatList(customKey, CustomField.Value, vals);
	    }
	    catch(UnityException ex)
	    {
		LogError(string.Format(GDMConstants.ErrorSettingCustomValue, GDMConstants.FloatType, ItemName.Value, FieldName.Value, CustomField.Value));
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
