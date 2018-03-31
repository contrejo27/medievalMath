using UnityEngine;
using System;
using System.Collections.Generic;
using GameDataEditor;


#if GDE_PLAYMAKER_SUPPORT && PLAYMAKER_1_8

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(GDMConstants.ActionCategory)]
    [Tooltip(GDMConstants.GetRandomItemKeyTooltip)]
    public class GDEGetRandomItemKey : FsmStateAction
    {
	[RequiredField]
	[UIHint(UIHint.FsmString)]
	[Tooltip(GDMConstants.SchemaTooltip)]
	public FsmString Schema;

	[UIHint(UIHint.FsmString)]
	public FsmString StoreResult;
		
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
		List<string> schemaKeys;
		StoreResult.Value = "";

		if (GDEDataManager.GetAllDataKeysBySchema(Schema.Value, out schemaKeys))
		    StoreResult.Value = schemaKeys.Random();	     
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
