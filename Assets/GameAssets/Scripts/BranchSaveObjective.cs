using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace HutongGames.PlayMaker.Actions
{

public class BranchSaveObjective : FsmStateAction
{
    public string Objective;
    public string Value;

    public FsmEvent isEvent;
    public FsmEvent isNotEvent;

    public override void OnEnter()
    {
        var obj_value = OptionalObjective.Instance.getObjectiveValue(Objective);
        if(obj_value == Value)
        {
            Debug.Log("Objective " + Objective + " Found");
            if(isEvent!=null)
                Fsm.Event(isEvent);
        }
        else
        {
            Debug.Log("Objective " + Objective + " Not Found");
            if(isNotEvent!=null)
                Fsm.Event(isNotEvent);
        }
        Finish();
    }   

}

}

