using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObjectObjective : BaseObjective
{
    public Interactable InteractableObject;
    public enum InteractionState {OnStart,OnComplete}
    public InteractionState EventFireAt = InteractionState.OnComplete;


    public override void StartObjective()
    {
        base.StartObjective();
        if(EventFireAt == InteractionState.OnComplete)
        {
            InteractableObject.OnInteractionStopCallback = OnInteractionStop;
        }

        if(EventFireAt == InteractionState.OnStart)
        {
            InteractableObject.OnInteractionStartCallback = OnInteractionStart;
        }    
    }

    public void OnInteractionStart()
    {
        onObjectiveComplete();
    }

    public void OnInteractionStop()
    {
        onObjectiveComplete();
    }


}
