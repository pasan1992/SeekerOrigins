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
            //Fsm.Event(onStartEvent);
            finished = true;
            Finish();
            finished = true;
        }
    }

    public void OnInteractionEnd()
    {
        if(FinishOnState == InteractObjectObjective.InteractionState.OnComplete && !finished)
        {
            //Fsm.Event(finishEvent);
            finished = true;
            m_interactable.SetInteratable(false);
            Finish();
            
        }
    }

    #if UNITY_EDITOR

#endif
}

}

