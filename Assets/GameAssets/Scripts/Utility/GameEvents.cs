﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents 
{
    public delegate void BasicNotifactionEvent();
    public delegate void BasicAgentCallback(ICyberAgent agent);
    public delegate void BasicPositionEvent(Vector3 position);
    
    public delegate void BasicSoundEvent(Vector3 position,AgentBasicData.AgentFaction faction,float maxDistance);

    public delegate void BasicEnableDisableEvent(bool enabled);

    public delegate void OnAmmoPickupEvent(AmmoPack ammoPack);
    public delegate void OnInteractionEvent(Interactable obj);
}
