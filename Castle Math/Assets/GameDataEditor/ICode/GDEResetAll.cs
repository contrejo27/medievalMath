using UnityEngine;
using System.Collections.Generic;
using GameDataEditor;

#if GDE_ICODE_SUPPORT

namespace ICode.Actions
{
	[Category(GDMConstants.ActionCategory)]
	[Tooltip(GDMConstants.ResetAllTooltip)]
	public class GDEResetAll : StateAction
	{   
		public override void OnEnter()
		{
			try
			{
				GDEDataManager.ClearSaved();
			}
			catch(UnityException ex)
			{
				Debug.LogError(GDMConstants.ErrorResettingAll);
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

