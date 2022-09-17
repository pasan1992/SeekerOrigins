using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectEventTrigger : GameEventTrigger
{
    public bool IsGlobal = false;
    public Interactable interactableObject;
    public string m_oninteractionStartEventName = "ON_START_INTERACTION";

    public string m_oninteractionEndEventName = "ON_STOP_INTERACTION";

    public void Start()
    {
        if(interactableObject ==null)
        {
            interactableObject = this.GetComponent<Interactable>();
        }

        interactableObject.OnInteractionStartCallback = onInteractionStart;
        interactableObject.OnInteractionStopCallback = onInteractionStop;
    }

    public void onInteractionStart()
    {
        if(IsGlobal)
        {
            PlayMakerFSM.BroadcastEvent(m_oninteractionStartEventName);
        }
        else
        {
            externalFSM.Fsm.Event(m_oninteractionStartEventName);
        }
        
    }

    public void onInteractionStop()
    {
        if(IsGlobal)
        {
            PlayMakerFSM.BroadcastEvent(m_oninteractionEndEventName);
        }
        else
        {
            externalFSM.Fsm.Event(m_oninteractionEndEventName);
        }
        
        
    }
}
