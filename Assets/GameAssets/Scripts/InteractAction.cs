using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HutongGames.PlayMaker.Actions
{

public class InteractionAction : FsmStateAction
{

    public InteractObjectObjective.InteractionState FinishOnState;
    public Interactable m_interactable;
    public FsmEvent finishEvent;
    public FsmEvent onStartEvent;

    private bool finished = false;

    public override void OnEnter()
    {
        m_interactable.properties.PlayerRestricted = false;
        m_interactable.properties.ObjectiveInteratable = true;
        m_interactable.properties.interactionEnabled = true;
        m_interactable.OnInteractionStartCallback = OnInteractionStart;
        m_interactable.OnInteractionStopCallback = OnInteractionEnd;
    }


    public void OnInteractionStart()
    {
        if(FinishOnState == InteractObjectObjective.InteractionState.OnStart && !finished)
        {
            EndIntAction();   
        }
    }

    public void OnInteractionEnd()
    {
        if(FinishOnState == InteractObjectObjective.InteractionState.OnComplete && !finished)
        {
            EndIntAction();     
        }
    }

    private void EndIntAction()
    {
        if(finishEvent!=null)
            Fsm.Event(finishEvent);
        finished = true;
        Finish();
        finished = true;       
    }

    #if UNITY_EDITOR

#endif
}

}

