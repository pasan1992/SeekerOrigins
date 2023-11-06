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
        if(m_interactable !=null)
        {
            m_interactable.properties.PlayerRestricted = false;
            m_interactable.properties.ObjectiveInteratable = true;
            m_interactable.properties.interactionEnabled = true;
            m_interactable.OnInteractionStartCallback = OnInteractionStart;
            m_interactable.OnInteractionStopCallback = OnInteractionEnd;
        }


        finished = false;
    }


    public void OnInteractionStart()
    {
        if(FinishOnState == InteractObjectObjective.InteractionState.OnStart && !finished)
        {
            EndIntAction(onStartEvent);   
        }
    }

    public void OnInteractionEnd()
    {
        Debug.Log("Interaction Complete");
        if(FinishOnState == InteractObjectObjective.InteractionState.OnComplete && !finished)
        {
            EndIntAction(finishEvent);     
        }
    }

    private void EndIntAction(FsmEvent end_event)
    {
        if(end_event!=null)
            Fsm.Event(end_event);
        finished = true;
        m_interactable.properties.interactionEnabled = false;
        Finish();
        finished = true;       
    }

    #if UNITY_EDITOR

#endif
}

}

