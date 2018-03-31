using UnityEngine;
using System.Collections.Generic;
using GameDataEditor;


#if GDE_PLAYMAKER_SUPPORT && PLAYMAKER_1_8

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(GDMConstants.ActionCategory)]
	[Tooltip(GDMConstants.GetTexture2DListCustomActionTooltip)]
	public class GDEGetCustomTexture2DArray : GDEActionBase
	{   
		[UIHint(UIHint.FsmString)]
		[Tooltip(GDMConstants.Texture2DListCustomFieldTooltip)]
		public FsmString CustomField;
		
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
				string customKey;
				List<Texture2D> val = null;
				
				if (GDEDataManager.DataDictionary.ContainsKey(ItemName.Value))
				{
					GDEDataManager.Get(ItemName.Value, out data);
					data.TryGetString(FieldName.Value, out customKey);
					customKey = GDEDataManager.GetString(ItemName.Value, FieldName.Value, customKey);
					
					Dictionary<string, object> customData;
					GDEDataManager.Get(customKey, out customData);
					
					customData.TryGetTexture2DList(CustomField.Value, out val);
				}
				else
				{
					// New item case
					customKey = GDEDataManager.GetString(ItemName.Value, FieldName.Value, string.Empty);
					
					if (GDEDataManager.Get(customKey, out data))
						data.TryGetTexture2DList(CustomField.Value, out val);
				}
				
				// Override from saved data if it exists
				val = GDEDataManager.GetUnityObjectList<Texture2D>(customKey, CustomField.Value, val);
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


