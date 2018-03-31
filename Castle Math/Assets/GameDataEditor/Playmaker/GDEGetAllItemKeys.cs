using UnityEngine;
using System;
using System.Collections.Generic;
using GameDataEditor;


#if GDE_PLAYMAKER_SUPPORT && PLAYMAKER_1_8

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(GDMConstants.ActionCategory)]
    [Tooltip(GDMConstants.GetAllItemKeysActionTooltip)]
    public class GDEGetAllItemKeys : FsmStateAction
    {
	[RequiredField]
	[UIHint(UIHint.FsmString)]
	[Tooltip(GDMConstants.SchemaTooltip)]
	public FsmString Schema;

	[UIHint(UIHint.Variable)]
	public FsmArray StoreResult;
		
	public override void Reset()
	{
	    base.Reset();
	    Schema = null;
	    StoreResult = null;
	}
		
	public override void OnEnter()
	{
	    try
	    {   
		List<string> result = null;
		GDEDataManager.GetAllDataKeysBySchema(Schema.Value, out result);
		
		if (result != null)
		    StoreResult.SetArrayContents(result);
		else
		    StoreResult.SetArrayContents(new List<string>());
	    }
	    catch(UnityException ex)
	    {
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
