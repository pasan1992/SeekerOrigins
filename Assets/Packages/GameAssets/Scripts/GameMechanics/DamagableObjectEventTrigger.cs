using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableObjectEventTrigger : GameEventTrigger
{
    public DamagableObject Damagable_Object;
    public bool IsGlobal = false;
    public string OnDamagedEvent = "ON_DAMAGED_OBJECT";

    public string OnDestroyedEvent = "ON_DESTROYED_OBJECT";

    private void Start()
    {
        if (Damagable_Object == null)
        {
            Damagable_Object = this.GetComponent<DamagableObject>();
        }

        Damagable_Object.setOnDamaged(onDamagedCallback);
        Damagable_Object.setOnDestroyed(onDestroyedCallback);
    }

    public void onDamagedCallback()
    {
        if(IsGlobal)
        {
            PlayMakerFSM.BroadcastEvent(OnDamagedEvent);
        }
        else
        {
            externalFSM.Fsm.Event(OnDamagedEvent);
        }

    }

    public void onDestroyedCallback()
    {
        if(IsGlobal)
        {
            PlayMakerFSM.BroadcastEvent(OnDestroyedEvent);
        }
        else
        {
            externalFSM.Fsm.Event(OnDestroyedEvent);
        }

    }
}
