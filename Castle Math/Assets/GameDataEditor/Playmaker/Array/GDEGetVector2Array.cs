using UnityEngine;
using System;
using System.Collections.Generic;
using GameDataEditor;


#if GDE_PLAYMAKER_SUPPORT && PLAYMAKER_1_8

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Game Data Editor")]
	[Tooltip("Gets a Vector2 Array from a GDE Item")]
	public class GDEGetVector2Array : GDEActionBase
	{
		[UIHint(UIHint.Variable)]
		public FsmArray StoreResult;
		
		public override void Reset()
		{
			base.Reset();
			
			if (StoreResult != null)
				StoreResult.Reset();
		}
		
		public override void OnEnter()
		{
			try
			{
				Dictionary<string, object> data;
				List<Vector2> val = null;
				if (GDEDataManager.Get(ItemName.Value, out data))
					data.TryGetVector2List(FieldName.Value, out val);
				
				// Override from saved data if it exists
				val = GDEDataManager.GetVector2List(ItemName.Value, FieldName.Value, val);
				StoreResult.SetArrayContents(val);
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