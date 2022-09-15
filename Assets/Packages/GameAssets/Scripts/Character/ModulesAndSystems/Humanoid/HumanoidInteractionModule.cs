using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanoidInteractionModule
{

    public delegate void OnInteractionOver();
    private GameEvents.OnAmmoPickupEvent onAmmoPickupEvent;
    private GameEvents.OnInteractionEvent onWeaponPickupEvent;

    private OnInteractionOver m_onInteractionOver;

    private HumanoidAnimationModule m_animationModule;
    private HumanoidMovmentModule m_movementModule;
    private AgentData m_agentData;
    private HumanoidRangedWeaponsModule m_equipmentModule;

    private NavMeshAgent m_navMesAgent;
    private HumanoidMovingAgent m_agent;

    /**
     This bool variable make sure that when interaction cancled, 
     all the Coroutines started by the Humanoid Agent realated to interaction will not continue the interaction.
    */
    private bool m_interacting;
    private bool m_unCancleableInstruction;
    private Interactable m_currentInteractingObject;
    private Transform m_handTransfrom;

    private RangedWeapon.WEAPONTYPE previouse_hold_weponType = Weapon.WEAPONTYPE.Common;

    public HumanoidInteractionModule(HumanoidAnimationModule animationModule, 
    HumanoidMovmentModule movmenetModule,
    AgentData agentData,
    HumanoidRangedWeaponsModule equipmentModule,
    NavMeshAgent agent,
    HumanoidMovingAgent mov_agent,
    OnInteractionOver onInteractionOver,
    Transform handTransfrom)
    {
        m_animationModule = animationModule;
        m_movementModule = movmenetModule;
        m_agentData = agentData;
        m_equipmentModule = equipmentModule;

        // This is the callback of the HumanoidMovingAgent to return the character state into normal state
        m_onInteractionOver = onInteractionOver;
        m_unCancleableInstruction = false;
        m_interacting = false;
        m_navMesAgent = agent;
        m_agent = mov_agent;
        m_handTransfrom = handTransfrom;
    }

    public void setPreviousWeapon()
    {
            var current_weapon = m_agent.getCurrentWeapon();
            if(current_weapon !=null)
            {
                previouse_hold_weponType = current_weapon.getWeaponType();
            }
            else
            {
                previouse_hold_weponType = Weapon.WEAPONTYPE.Common;
            }
    }
    public IEnumerator interactWith(Interactable interactable,Interactable.InteractableProperties.InteractableType type)
    {
        if(!m_interacting)
        {
            // Set all required varialbes and conditions for the interaction
            m_interacting = true;
            m_navMesAgent.enabled = false;
            m_currentInteractingObject = interactable;
            m_currentInteractingObject.interact();



            switch (type)
            {
                case Interactable.InteractableProperties.InteractableType.PickupInteraction:

                    float distance = Vector3.Distance(m_movementModule.getCharacterTransfrom().position,interactable.transform.position);
                    
                    // Make sure the character turns at the object - if the character is very close no need to turn.
                    if(distance>1f)
                    {
                        Vector3 lookPoistion = new Vector3(interactable.transform.position.x,m_movementModule.getCharacterTransfrom().position.y,interactable.transform.position.z);
                        m_movementModule.LookAtObject(lookPoistion);
                    }

                    // Check if need to bend
                    int interactionID = -1;
                    if(Mathf.Abs(interactable.transform.position.y - m_movementModule.getCharacterTransfrom().position.y) > 0.5f)
                    {
                        interactionID = -2;
                    }

                    if(m_agent.isHidden())
                    {
                        m_agent.toggleHide();
                    }
                    //yield return onPickup(interactable,0);
                    //m_animationModule.triggerPickup(interactionID);

                    // Is the agent is crouched no animation is played when picking up items, Thus no wait time
                    if (m_animationModule.isEquiped())
                    {
                        yield return onPickup(interactable, 0);
                    }
                    else
                    {
                        m_animationModule.triggerPickup(interactionID);
                        yield return onPickup(interactable, 0.3f);
                    }

                    break;
                case Interactable.InteractableProperties.InteractableType.FastInteraction:
                    if (interactable is AmmoPack)
                    {
                        ConsumeAmmoPack((AmmoPack)interactable);
                    }
                    break;
                case Interactable.InteractableProperties.InteractableType.TimedInteraction:
                    if(m_agent.isHidden())
                    {
                        m_agent.toggleHide();
                    }
                    yield return onTimedInteraction(interactable);
                    
                    break;
                case Interactable.InteractableProperties.InteractableType.ContinousInteraction:
                case Interactable.InteractableProperties.InteractableType.FixedContinousInteraction:
                    if(m_agent.isHidden())
                    {
                        m_agent.toggleHide();
                    }
                    yield return onContinousInteraction(interactable);
                    
                    break;
                case Interactable.InteractableProperties.InteractableType.DialogInteraction:
                    if(m_agent.isHidden())
                    {
                        m_agent.toggleHide();
                    }
                    yield return DialogInteraction(interactable);
                    
                    break;
            }

            // Make sure the character state return to previous state.
            if(m_interacting)
            {
                // Only reach this with uncancalalbe instructions -on pick up automaticaly return to animation, no need to cancle
                yield return new WaitForSeconds(0.2f);
                m_interacting = false;
                m_onInteractionOver();
                m_navMesAgent.enabled = true;
                goBackToPreviousWeapon();
            }

        }
        yield return null;
    }

    private void goBackToPreviousWeapon()
    {
        switch(previouse_hold_weponType)
        {
            case Weapon.WEAPONTYPE.primary:
            m_agent.togglePrimaryWeapon();
            break;
            case Weapon.WEAPONTYPE.secondary:
            m_agent.togglepSecondaryWeapon();
            break;
        }
        previouse_hold_weponType = Weapon.WEAPONTYPE.Common;
    }

    public void cancleInteraction()
    {
        if(!m_unCancleableInstruction)
        {
            m_interacting = false;
            m_animationModule.setInteraction(false,0);
            m_onInteractionOver();
            
            
            if(m_currentInteractingObject != null)
            {
                m_currentInteractingObject.stopInteraction();
            }
            
            m_navMesAgent.enabled = true;
            goBackToPreviousWeapon();
        }
    }

    private IEnumerator onPickup(Interactable obj,float waitTime)
    {
        
        // fast interactions are uncancellable - once started need to wait until they are ended.
        m_unCancleableInstruction = true;

        m_animationModule.setUpperAnimationLayerWeight(0.2f);
        obj.OnInteractionStart();
        yield return new WaitForSeconds(waitTime);
        m_animationModule.setUpperAnimationLayerWeight(1);

        if(obj.properties.Type.Equals(Interactable.InteractableProperties.InteractableType.PickupInteraction))
        {
            if(obj is RangedWeapon)
            {
                if(obj is PrimaryWeapon)
                {
                    var wp  = obj.GetComponent<PrimaryWeapon>();
                    wp.accuracy_rating = 100;
                    m_agentData.primaryWeapon = wp;
                    m_equipmentModule.equipWeapon(m_agentData.primaryWeapon);
                    obj.OnEquipAction();
                    if(onWeaponPickupEvent !=null)
                    {
                        onWeaponPickupEvent(m_agent.GetPrimaryWeapon());
                    }
                    /*
                    if (!m_agentData.primaryWeapon)
                    {
                        m_agentData.primaryWeapon = obj.GetComponent<PrimaryWeapon>();
                        m_equipmentModule.equipWeapon(m_agentData.primaryWeapon);
                        obj.OnEquipAction();
                    }
                    else
                    {
                        
                        m_agentData.primaryWeapon = obj.GetComponent<PrimaryWeapon>();
                        m_equipmentModule.equipWeapon(m_agentData.primaryWeapon);
                        obj.OnEquipAction();
                    }
                    */
                }
                else if(obj is SecondaryWeapon)
                {
                    var wp = obj.GetComponent<SecondaryWeapon>();
                    wp.accuracy_rating = 100;
                    m_agentData.secondaryWeapon = wp;
                    m_equipmentModule.equipWeapon(m_agentData.secondaryWeapon);
                    obj.OnEquipAction();
                    if(onWeaponPickupEvent !=null)
                    {
                        onWeaponPickupEvent(m_agent.GetSecondaryWeapon());
                    }
                    /*
                    if (!m_agentData.secondaryWeapon)
                    {
                        m_agentData.secondaryWeapon = obj.GetComponent<SecondaryWeapon>();
                        m_equipmentModule.equipWeapon(m_agentData.secondaryWeapon);
                        obj.OnEquipAction();
                    }
                    else
                    {
                        m_agentData.secondaryWeapon = obj.GetComponent<SecondaryWeapon>();
                        m_equipmentModule.equipWeapon(m_agentData.secondaryWeapon);
                        obj.OnEquipAction();
                    }
                    */
                
                }

            }
            else if(obj is AmmoPack)
            {
                var ammo_pck = (AmmoPack)obj;
                ConsumeAmmoPack(ammo_pck);
            }
            else if(obj is Grenade)
            {
                var wp = obj.GetComponent<Grenade>();
                m_agentData.grenade = wp;
                m_equipmentModule.equipWeapon(m_agentData.grenade);
                obj.OnEquipAction();   
                if(onWeaponPickupEvent !=null)
                {
                    onWeaponPickupEvent(m_agentData.grenade);
                }                
            }
            else
            {
                ((Interactable)obj).stopInteraction();
            }
            
        }
         yield return new WaitForSeconds(waitTime);

         m_unCancleableInstruction = false;
    }

    public void ConsumeAmmoPack(AmmoPack ammo_pck)
    {
        
        foreach(AgentData.AmmoPack ammo in ammo_pck.AmmoPackData)
        {
            // int totalAmmo = 0;
            // m_agentData.weaponAmmoCount.TryGetValue(ammo_pck.AmmoType, out totalAmmo);
            // m_agentData.weaponAmmoCount[ammo_pck.AmmoType] = totalAmmo + ammo_pck.count;
            // ammo_pck.gameObject.SetActive(false);

            // int totalAmmo = 0;
            // m_agentData.weaponAmmoCount.TryGetValue(ammo.AmmoType, out totalAmmo);
            // m_agentData.weaponAmmoCount[ammo.AmmoType] = totalAmmo + ammo.AmmoCount;
            m_agentData.AddAmmo(ammo.AmmoType,ammo.AmmoCount);
        }
        //ammo_pck.AmmoPackData = new List<AgentData.AmmoPack>();

        m_agent.SetGrenateCount(ammo_pck.GrenadeCount);
        m_agent.GetAgentData().HealInjectionCount += ammo_pck.HealthInjectionCount;
        if (onAmmoPickupEvent != null)
        {
            onAmmoPickupEvent(ammo_pck);
        }
        ammo_pck.OnPickUpAction();
    }

    /**
     Continous interactions will continue unless they are cancled
    */
    private IEnumerator onContinousInteraction(Interactable interactableObj)
    {
        Vector3 intendedPosition = interactableObj.transform.position + interactableObj.properties.offset;
        Quaternion intentedRotation =  Quaternion.Euler(interactableObj.properties.rotation);
        
        Transform transform = m_movementModule.getCharacterTransfrom();

        // Place agent in the intaraction position.
        while((Vector3.Distance(transform.position,intendedPosition) > 0.01f ||
         Mathf.Abs(intentedRotation.eulerAngles.y - transform.rotation.eulerAngles.y) > 5f) &&
         m_interacting)
        {

            // if(Vector3.Distance(transform.position,intendedPosition) > 0.3f)
            // {
            //     Debug.Log("position mission");
            // }
            


            // if(Mathf.Abs(intentedRotation.eulerAngles.y - transform.rotation.eulerAngles.y) > 5f)
            // {
            //     Debug.Log("angle missing");
            // }

            transform.rotation = Quaternion.Lerp(transform.rotation,intentedRotation,0.2f);
            transform.position = Vector3.Lerp(transform.position,intendedPosition,0.1f);
            m_animationModule.setMovment(0,0);

            // To enable smooth transistion from staring positon and rotation to end position and rotation.
            yield return new WaitForSeconds(Time.deltaTime/2);
        }     

        /** Make sure that agent is still in interaction mode before continuing the interaction 
            If interaction is cancled in mid process this condition will fail
         */
        if(m_interacting)
        {
            m_animationModule.setInteraction(true,interactableObj.properties.interactionID);

            // Wait until intereaction is cancled.
            while(m_interacting)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }            
        }  
    }

    /**
     Continous interactions will continue unless they are cancled
    */
    private IEnumerator DialogInteraction(Interactable interactableObj)
    {
        Vector3 intendedPosition = interactableObj.transform.position + interactableObj.properties.offset;
        Quaternion intentedRotation =  Quaternion.Euler(interactableObj.properties.rotation);
        
        Transform transform = m_movementModule.getCharacterTransfrom();

        // Place agent in the intaraction position.
        while((Vector3.Distance(transform.position,intendedPosition) > 0.01f ||
         Mathf.Abs(intentedRotation.eulerAngles.y - transform.rotation.eulerAngles.y) > 5f) &&
         m_interacting)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,intentedRotation,0.2f);
            transform.position = Vector3.Lerp(transform.position,intendedPosition,0.1f);
            m_animationModule.setMovment(0,0);

            // To enable smooth transistion from staring positon and rotation to end position and rotation.
            yield return new WaitForSeconds(Time.deltaTime/2);
        }     

        /** Make sure that agent is still in interaction mode before continuing the interaction 
            If interaction is cancled in mid process this condition will fail
         */
        if(m_interacting)
        {
            m_animationModule.setInteraction(true,interactableObj.properties.interactionID);
            var dialog_interaction = interactableObj.GetComponent<DialogInteratable>();
            yield return dialog_interaction.PlayDialog();      

            if(m_interacting)
            {
                cancleInteraction();
            }         
        }  
    }


    /**
       Interaction will end after a certain time.
    */
    private IEnumerator onTimedInteraction(Interactable interactableObj)
    {

        Vector3 intendedPosition = interactableObj.transform.position + interactableObj.properties.offset;
        Quaternion intentedRotation =  Quaternion.Euler(interactableObj.properties.rotation);
        Transform transform = m_movementModule.getCharacterTransfrom();

        // Place agent in the interaction position
        while((Vector3.Distance(transform.position,intendedPosition) > 0.01f || 
        Mathf.Abs(intentedRotation.eulerAngles.y - transform.rotation.eulerAngles.y) > 5f) &&
        m_interacting && interactableObj.properties.enablePositionRequirment)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,intentedRotation,0.2f);
            transform.position = Vector3.Lerp(transform.position,intendedPosition,0.1f);
            m_animationModule.setMovment(0,0);

            // To enable smooth transistion from staring positon and rotation to end position and rotation.
            yield return new WaitForSeconds(Time.deltaTime/2);
        }

        /** Make sure that agent is still in interaction mode before continuing the interaction 
            If interaction is cancled in mid process this condition will fail
         */
        if(m_interacting)
        {
            m_animationModule.setInteraction(true,interactableObj.properties.interactionID);
            float waitTime = interactableObj.properties.interactionTime;
            float currentWaitedTime = 0;

            


            while(waitTime > currentWaitedTime && m_interacting)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                currentWaitedTime += Time.deltaTime;
            }

            // After every wait, need to check the if the interaction is still in progress before continuing.
            if(m_interacting)
            {
                cancleInteraction();
            }        
        }
    }

    // If Object need to be placed on the hand, handle that
    public void HandInPosition()
    {
        if(m_interacting)
        {
            if(m_currentInteractingObject.properties.placeObjectInHand)
            {
                m_currentInteractingObject.properties.actualObject.transform.parent = m_handTransfrom;
                m_currentInteractingObject.properties.actualObject.transform.localPosition = m_currentInteractingObject.visualProperties.holdingPositionOffset;
                m_currentInteractingObject.properties.actualObject.transform.localRotation = Quaternion.Euler(m_currentInteractingObject.visualProperties.holdingRotationOffset);
            }
        }
    }

    public void SkipDialog()
    {
        if(m_currentInteractingObject !=null 
        && m_currentInteractingObject.properties.Type == Interactable.InteractableProperties.InteractableType.DialogInteraction)
        {
            var dialog_interaction = m_currentInteractingObject.GetComponent<DialogInteratable>();
            dialog_interaction.SkipDialog();
        }
    }

    public Interactable GetCurrentInteractable()
    {
        return m_currentInteractingObject;
    }

    public void setOnAmmoPickupCallback(GameEvents.OnAmmoPickupEvent onAmmoPickup)
    {
        onAmmoPickupEvent += onAmmoPickup;
    }

    public void setOnWeaponPickupEvent(GameEvents.OnInteractionEvent weapon)
    {
        onWeaponPickupEvent += weapon;
    }
}
