using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HutongGames.PlayMaker.Actions
{

public class SetObjectiveAction : FsmStateAction
{
    // Start is called before the first frame update
    public string ObjectiveMessage; 

    public override void OnEnter()
    {
        DialogManager.instance.SetObjectiveText(ObjectiveMessage);
        Finish();
    }


    #if UNITY_EDITOR

#endif
}

}

