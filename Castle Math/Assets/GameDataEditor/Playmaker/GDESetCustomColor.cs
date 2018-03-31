using UnityEngine;
using System.Collections.Generic;
using GameDataEditor;

#if GDE_PLAYMAKER_SUPPORT

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(GDMConstants.ActionCategory)]
	[Tooltip(GDMConstants.SetCustomColorActionTooltip)]
	public class GDESetCustomColor : GDESetActionBase
	{   
		[UIHint(UIHint.FsmString)]
		[Tooltip(GDMConstants.ColorCustomFieldTooltip)]
		public FsmString CustomField;
		
		[UIHint(UIHint.FsmColor)]
		public FsmColor ColorValue;
		
		public override void Reset()
		{
			base.Reset();
			ColorValue = null;
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
				
				GDEDataManager.SetColor(customKey, CustomField.Value, ColorValue.Value);
			}
			catch(UnityException ex)
			{
				LogError(string.Format(GDMConstants.ErrorSettingCustomValue, GDMConstants.ColorType, ItemName.Value, FieldName.Value, CustomField.Value));
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

