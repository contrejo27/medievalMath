using UnityEngine;
using System.Collections.Generic;
using GameDataEditor;

#if GDE_PLAYMAKER_SUPPORT

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(GDMConstants.ActionCategory)]
	[Tooltip(GDMConstants.ResetAllTooltip)]
	public class GDEResetAll : FsmStateAction
	{   
		public override void OnEnter()
		{
			try
			{
				GDEDataManager.ClearSaved();
			}
			catch(UnityException ex)
			{
				LogError(GDMConstants.ErrorResettingAll);
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
