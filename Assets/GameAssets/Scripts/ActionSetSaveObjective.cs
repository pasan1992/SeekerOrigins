using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace HutongGames.PlayMaker.Actions
{

public class ActionSetSaveObjective : FsmStateAction
{
    public string Objective;
    public string value;

    public override void OnEnter()
    {
        OptionalObjective.Instance.setObjective(Objective,value);
        Finish();
    }   

}

}

