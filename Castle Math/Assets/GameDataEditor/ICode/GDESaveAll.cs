using UnityEngine;
using System.Collections.Generic;
using GameDataEditor;

#if GDE_ICODE_SUPPORT

namespace ICode.Actions
{
	[Category(GDMConstants.ActionCategory)]
	[Tooltip(GDMConstants.SaveAllTooltip)]
	public class GDESaveAll : StateAction
	{   
		public override void OnEnter()
		{
			try
			{
				GDEDataManager.Save();
			}
			catch(UnityException ex)
			{
				Debug.LogError(GDMConstants.ErrorSavingData);
				Debug.LogError(ex.ToString());
			}
			finally
			{
				Finish();
			}
		}
	}
}

#endif

