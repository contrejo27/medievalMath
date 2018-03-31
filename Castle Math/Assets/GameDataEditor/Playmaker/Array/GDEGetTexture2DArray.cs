using UnityEngine;
using System;
using System.Collections.Generic;
using GameDataEditor;


#if GDE_PLAYMAKER_SUPPORT && PLAYMAKER_1_8

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(GDMConstants.ActionCategory)]
	[Tooltip(GDMConstants.GetTexture2DListActionTooltip)]
	public class GDEGetTexture2DArray : GDEActionBase
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
				List<Texture2D> val = null;
				if (GDEDataManager.Get(ItemName.Value, out data))
					data.TryGetTexture2DList(FieldName.Value, out val);
				
				// Override from saved data if it exists
				val = GDEDataManager.GetUnityObjectList<Texture2D>(ItemName.Value, FieldName.Value, val);
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