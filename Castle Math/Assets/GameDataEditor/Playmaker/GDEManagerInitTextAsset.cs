using UnityEngine;
using GameDataEditor;

#if GDE_PLAYMAKER_SUPPORT

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(GDMConstants.ActionCategory)]
    [Tooltip(GDMConstants.InitActionTooltip)]
    public class GDEManagerInitTextAsset : FsmStateAction
    {
        [RequiredField]
        public FsmObject GDEData;

	[UIHint(UIHint.FsmBool)]
	[Tooltip(GDMConstants.EncryptedCheckboxTooltip)]
	public FsmBool Encrypted;
            
        public override void OnEnter()
        {
            try
            {
		TextAsset textAsset = GDEData.Value as TextAsset;
		if (!GDEDataManager.Init(textAsset, Encrypted.Value))
                    LogError(GDMConstants.ErrorNotInitialized);
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
