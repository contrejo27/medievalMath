using UnityEngine;
using System.Collections.Generic;
using GameDataEditor;

#if GDE_PLAYMAKER_SUPPORT

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(GDMConstants.ActionCategory)]
	[Tooltip(GDMConstants.SaveAllTooltip)]
	public class GDESaveAll : FsmStateAction
	{   
		public override void OnEnter()
		{
			try
			{
				GDEDataManager.Save();
			}
			catch(UnityException ex)
			{
				LogError(GDMConstants.ErrorSavingData);
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
