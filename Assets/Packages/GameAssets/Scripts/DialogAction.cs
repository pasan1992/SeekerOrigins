using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HutongGames.PlayMaker.Actions
{

public class DialogAction : FsmStateAction
{
    // Start is called before the first frame update
    public DialogManager.DialogStatment[] dialogStatments;
    private int totalDialogs;

    public FsmEvent finishEvent;
    private bool dialogEnded = false;

    public bool WaitTillDialog = true;

    public override void OnEnter()
    {
        totalDialogs = dialogStatments.Length;
        DialogManager.instance.StartDialog(dialogStatments,onDialogEnd);
        dialogEnded =  false;
        
        if (!WaitTillDialog)
        {
            Finish();
        }
    }
    public override void OnUpdate()
    {
        if(dialogEnded)
        {
            Fsm.Event(finishEvent);
            Finish();
        }
    }

    public void onDialogEnd()
    {
        dialogEnded = true;
    }

    #if UNITY_EDITOR

    public override float GetProgress()
    {
        return (totalDialogs - (float)DialogManager.instance.getRemaningLines())/totalDialogs;
    }

#endif
}

}

