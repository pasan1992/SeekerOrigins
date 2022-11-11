using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
public class HumanoidRangedWeaponsModule
{
    #region protectedParameters

    // Weapon Related properties
    protected Weapon m_currentWeapon;
    protected RangedWeapon m_rifle;
    protected RangedWeapon m_pistol;
    protected Grenade m_grenede;

    protected GameObject primaryHosterLocation;
    protected GameObject secondaryHosterLocation;
    protected GameObject weaponHoldLocation;
    protected GameObject grenadeHoldLocation;

    protected AgentData m_agentData;

    protected AgentFunctionalComponents m_agentComponents;

    protected GameObject m_target;
    protected Recoil m_recoil;
    protected HumanoidMovingAgent.CharacterMainStates m_currentState;
    protected HumanoidAnimationModule m_animationSystem;

    // Functional related properties
    private bool m_inEquipingAction = false;
    private bool m_inWeaponAction = false;

    private enum WeaponSystemSubStages {
        Armed 
        ,Equiping 
        ,UnEquiping
        ,Reloading
        ,WeaponAction
        ,UnArmed
        }
    private WeaponSystemSubStages m_currentWeaponSubStage;
    
    #endregion

    public HumanoidRangedWeaponsModule( 
    WeaponProp[] props, 
    HumanoidMovingAgent.CharacterMainStates state, 
    GameObject target, 
    Recoil recoil, 
    HumanoidAnimationModule animSystem,
    AgentData agentData,
    AgentFunctionalComponents agentComponets)
    {
        m_currentState = state;
        m_target = target;
        m_recoil = recoil;
        m_animationSystem = animSystem;
        m_agentData = agentData;
        m_agentComponents = agentComponets;
        getAllWeapons(props);
    }

    private void UpdateWeaponSubStage()
    {
        if(m_animationSystem.checkCurrentAnimationTag("Armed"))
        {
            m_currentWeaponSubStage = WeaponSystemSubStages.Armed;
        }
        else if(m_animationSystem.checkCurrentAnimationTag("Equip"))
        {
            m_currentWeaponSubStage = WeaponSystemSubStages.Equiping;
        }
        else if(m_animationSystem.checkCurrentAnimationTag("UnEquip"))
        {
            m_currentWeaponSubStage = WeaponSystemSubStages.UnEquiping;
        }
        else if(m_animationSystem.checkCurrentAnimationTag("Reload"))
        {
            m_currentWeaponSubStage = WeaponSystemSubStages.Reloading;
        }
        else if(m_animationSystem.checkCurrentAnimationTag("WeaponAction"))
        {
            m_currentWeaponSubStage = WeaponSystemSubStages.WeaponAction;
        }
        else
        {
           m_currentWeaponSubStage = WeaponSystemSubStages.UnArmed;
        }

        switch (m_currentWeaponSubStage)
        {
            case WeaponSystemSubStages.Armed:
                m_inWeaponAction = false;
                m_inEquipingAction = false;
                break;
            case WeaponSystemSubStages.Equiping:
            break;
            case WeaponSystemSubStages.Reloading:
            break;
            case WeaponSystemSubStages.UnEquiping:
            break;
            case WeaponSystemSubStages.WeaponAction:
                m_inWeaponAction = true;
            break;
        }
    }

    public bool is_ready_to_aim()
    {
        return (m_currentWeaponSubStage != WeaponSystemSubStages.Reloading && !isInEquipingAction());
    }

    #region updates
    public void UpdateSystem(HumanoidMovingAgent.CharacterMainStates state)
    {
        UpdateWeaponSubStage();

        if (m_currentWeapon != null)
        {
            m_currentWeapon.updateWeapon();
        }

        // On Character state change.
        switch (state)
        {
            case HumanoidMovingAgent.CharacterMainStates.Aimed:

                // Set Gun Aimed;
                if (!m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Aimed))
                {
                    aimCurrentEquipment(true);
                }

                break;
            case HumanoidMovingAgent.CharacterMainStates.Armed_not_Aimed:

                // Set Gun Aimed;
                if (!m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Armed_not_Aimed))
                {

                    aimCurrentEquipment(false);
                }
                break;
            case HumanoidMovingAgent.CharacterMainStates.Idle:

                // Set one time only
                if (!m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Idle))
                {
                    aimCurrentEquipment(false);
                }
                break;
            default:
                if (m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Armed_not_Aimed) || m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Aimed))
                {
                    aimCurrentEquipment(false);
                }
                break;
        }


        m_currentState = state;
    }
    #endregion

    #region Event handlers

    public void OnThrow()
    {
        //m_inWeaponAction = false;
        var grenade = m_agentData.useAmmoCount(m_grenede.getCurrentGrenateType(),1);
        if (grenade ==0)
        {
            Debug.LogError("This is a bug");
        }
        m_grenede.ThrowGrenadeQuick(grenadeHoldLocation.transform);
        // if(m_grenede.count == 0)
        // {
        //     m_inWeaponAction = false;
        //     toggleSecondary();
        // }

        if(getGrenadeCount() < 0)
        {
            Debug.LogError("G COUNT LESS THANT ZERO");
        }
    }
    public void ReloadEnd()
    {
        RangedWeapon currentRangedWeapon = (RangedWeapon)m_currentWeapon;

        if(m_currentWeapon !=null && currentRangedWeapon)
        {
            currentRangedWeapon.setReloading(false);
            int totalAmmo = m_agentData.checkUnloadAvaialbleAmmo(currentRangedWeapon.m_weaponAmmunitionName);
            // m_agentData.weaponAmmoCount.TryGetValue(currentRangedWeapon.m_weaponAmmunitionName, out totalAmmo);
            var ammo_needed = currentRangedWeapon.m_magazineSize - currentRangedWeapon.getLoadedAmmoCount();

            // // Enought Ammo available
            // if(totalAmmo > ammo_needed)
            // {
            //     totalAmmo -= ammo_needed;
            //     //  m_agentParameters.weaponAmmoCount.(m_currentWeapon.name,totalAmmo);
            //     m_agentData.weaponAmmoCount[currentRangedWeapon.m_weaponAmmunitionName] = totalAmmo;
            //     currentRangedWeapon.setAmmoCount(currentRangedWeapon.m_magazineSize);
            //     return;
            // }

            // // Not enough ammo
            // currentRangedWeapon.setAmmoCount(totalAmmo + currentRangedWeapon.getAmmoCount());
            // m_agentData.weaponAmmoCount[currentRangedWeapon.m_weaponAmmunitionName] = 0;
            var ammo_taken = m_agentData.useAmmoCount(currentRangedWeapon.m_weaponAmmunitionName,ammo_needed);
            currentRangedWeapon.setAmmoCount( currentRangedWeapon.getLoadedAmmoCount() + ammo_taken);
            return;
        }
    }

    // Equip Animation event.
    public void EquipAnimationEvent()
    {
        RangedWeapon.WEAPONTYPE type = m_currentWeapon.getWeaponType();
        //m_inEquipingAction = false;
        placeWeaponInHand(m_currentWeapon);

        // Set Current Weapon Properties.
        m_currentWeapon.gameObject.SetActive(true);

        switch(m_currentWeapon.getWeaponType())
        {
            case Weapon.WEAPONTYPE.primary:
            case Weapon.WEAPONTYPE.secondary:
                ((RangedWeapon)m_currentWeapon).setGunTarget(m_target);
            break;

            case Weapon.WEAPONTYPE.grenede:
                m_currentWeapon.setGunTarget(m_target);
            break;
        }

        // if(m_currentWeapon.GetType().Equals(typeof( RangedWeapon)))
        // {
        //     ((RangedWeapon)m_currentWeapon).setGunTarget(m_target);
        // }      
    }

    // UnEquip Animation event.
    public void UnEquipAnimationEvent()
    {
        RangedWeapon.WEAPONTYPE type = m_currentWeapon.getWeaponType();
        //m_currentWeapon.gameObject.SetActive(false);
        m_currentWeapon = null;
        //m_inEquipingAction = false;

        switch (type)
        {
            case RangedWeapon.WEAPONTYPE.primary:
                
                //m_rifleProp.setVisible(true);
                //placePrimaryWeaponInHosterLocation();
                // Debug.Log("Primary Unequip Finished");
                placeWeaponinHosterLocation(m_rifle);
                break;

            case RangedWeapon.WEAPONTYPE.secondary:
                //m_pistolProp.setVisible(true);
                placeWeaponinHosterLocation(m_pistol);
                //placeSecondaryWeaponInHosterLocation();
                //Debug.Log("Secondary Unequip Finished");
                break;
        }
    }

    public void OnWeaponFire(float weight)
    {
        m_recoil.Fire(weight);
    }
    #endregion

    #region commands

    public void pullTrigger()
    {
        if (isProperlyAimed() && m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Aimed) && m_currentWeapon)
        {
            switch(m_currentWeapon.getWeaponType())
            {
                case Weapon.WEAPONTYPE.primary:
                case Weapon.WEAPONTYPE.secondary:
                    ((RangedWeapon)m_currentWeapon).pullTrigger();
                break;

                case Weapon.WEAPONTYPE.grenede:
                    // m_inWeaponAction = true;
                    // m_grenede.pullGrenedePin();
                break;
            }

            // if(m_currentWeapon.GetType().Equals(typeof( RangedWeapon)))
            // {
            //     ((RangedWeapon)m_currentWeapon).pullTrigger();
            // }  
            // //m_currentWeapon.pullTrigger();

            // if(m_currentWeapon.GetType().Equals(typeof(Grenade)))
            // {
            //     m_inWeaponAction = true;
            // }
        }
    }

    public void releaseTrigger()
    {
        if (m_currentWeapon)
        {
            //m_currentWeapon.releaseTrigger();

            // if(m_currentWeapon.GetType().Equals(typeof(RangedWeapon)))
            // {
            //     ((RangedWeapon)m_currentWeapon).releaseTrigger();
            // } 

            // if(m_currentWeapon.GetType().Equals(typeof(Grenade)))
            // {
            //     m_animationSystem.triggerThrow();
            //     m_inWeaponAction = true;
            // }

            switch(m_currentWeapon.getWeaponType())
            {
                case Weapon.WEAPONTYPE.primary:
                case Weapon.WEAPONTYPE.secondary:
                    ((RangedWeapon)m_currentWeapon).releaseTrigger();
                break;

                case Weapon.WEAPONTYPE.grenede:
                    if(m_grenede.isPinPulled())
                    {
                        m_animationSystem.triggerThrow();
                        m_inWeaponAction = true;
                    }
                break;
            }
        }
    }

    public void DropCurrentWeapon()
    {
        if (m_currentWeapon)
        {
            Debug.Log("Droping current weapon");
            m_currentWeapon.dropWeapon();
        }
    }

    public void DropAllWeapons()
    {
        if(!m_agentData.drop_reward)
        {
            if (m_pistol)
                m_pistol.gameObject.SetActive(false);

            if(m_rifle)
                m_rifle.gameObject.SetActive(false);           
            return;
        }
        var rand_value = Random.value;

        if(rand_value < 0.5)
        {
            if (m_pistol)
            {
                m_pistol.dropWeapon();
                // var ammo_pack = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.PistolAmmo);
                // var pck = ammo_pack.GetComponent<AmmoPack>();
                // (ammo_pack.GetComponent<InteractableTrigger>()).trig_enabled = true;
                // ammo_pack.SetActive(true);
                // CommonFunctions.place_on_ground(m_pistol.transform.position, ammo_pack.transform);
            }

            if (m_rifle)
                m_rifle.gameObject.SetActive(false);
        }
        else if(rand_value < 0.9)
        {
            if (m_rifle)
            {
                m_rifle.dropWeapon();
                // var ammo_pack = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.PistolAmmo);
                // (ammo_pack.GetComponent<InteractableTrigger>()).trig_enabled = true;
                // ammo_pack.SetActive(true);
                // var pck = ammo_pack.GetComponent<AmmoPack>();
                // CommonFunctions.place_on_ground(m_rifle.transform.position, ammo_pack.transform);
            }

            if (m_pistol)
                m_pistol.gameObject.SetActive(false);
        }
        else
        {
                if(m_rifle)
                m_rifle.gameObject.SetActive(false);

            if (m_pistol)
                m_pistol.gameObject.SetActive(false);
        }





        /*
        if(m_grenede)
        {
            m_grenede.dropWeapon();
        }
        */
    }

    private void aimCurrentEquipment(bool aimed)
    {
        m_animationSystem.aimEquipment(aimed);

        if (getCurrentWeapon())
        {
            getCurrentWeapon().setAimed(aimed);
        }

    }

    public bool reloadCurretnWeapon()
    {
        if(!canReload())
        {
            return false;
        }

        if(m_currentWeapon !=null && !isReloading() && !m_inWeaponAction && !m_currentWeapon.GetType().Equals(typeof(Grenade)))
        {
            switch(m_currentWeapon.getWeaponType())
            {
                case Weapon.WEAPONTYPE.primary:
                case Weapon.WEAPONTYPE.secondary:
                    ((RangedWeapon)m_currentWeapon).reloadWeapon();
                    m_animationSystem.triggerReload();
                    return true;
                case Weapon.WEAPONTYPE.grenede:
                    return false;
            }
            // m_currentWeapon.reloadWeapon();
            // m_animationSystem.triggerReload();
        }
        return false;
    }

    private bool canReload()
    {
        
        

        if (m_currentWeapon != null)
        {
            // If ammo count is full No need to reload
            var r_weapon = (RangedWeapon)m_currentWeapon;
            if (r_weapon.getLoadedAmmoCount() == r_weapon.m_magazineSize)
            {
                return false;
            }

            // int totalAmmo = 0;
            // m_agentData.weaponAmmoCount.TryGetValue(m_currentWeapon.m_weaponAmmunitionName, out totalAmmo);
            int totalAmmo = m_agentData.checkUnloadAvaialbleAmmo(m_currentWeapon.m_weaponAmmunitionName);
            if (totalAmmo > 0)
            {
                return true;
            }
        }

        return false;
    }

    public HumanoidMovingAgent.CharacterMainStates togglePrimary()
    {
        if (!isInEquipingAction() && !isReloading() && m_rifle && !m_inWeaponAction)
        {
            m_animationSystem.setCurretnWeapon(1);

            if (m_currentWeapon != null)
            {
                // UnEquip Weapon With Animation
                if (m_currentWeapon.getWeaponType().Equals(RangedWeapon.WEAPONTYPE.primary))
                {
                    m_inEquipingAction = true;
                    return m_animationSystem.unEquipEquipment();
                }
                // Fast toggle to weapon
                else
                {
                    // Fast toggle
                    placeWeaponinHosterLocation(m_currentWeapon);
                    m_currentWeapon = m_rifle;
                    placeWeaponInHand(m_currentWeapon);
                         
                    m_animationSystem.fastEquipCurrentEquipment();

                    if (m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Aimed))
                    {
                        m_currentWeapon.setAimed(true);
                    }

                    return m_currentState;
                }
            }
            // Equip Weapon with animation
            else
            {
                m_inEquipingAction = true;
                m_currentWeapon = m_rifle;
                return m_animationSystem.equipCurrentEquipment();
            }
        }
        // Not possible to equip weapon - in mid action or weapon not available
        else
        {
            // Weapon not avialable
            if(m_currentWeapon == null && m_rifle == null && !m_inWeaponAction)
            {
                m_animationSystem.triggerShrug();
            }
            logWeaponIssue();
            return m_currentState;
        }

    }
    public HumanoidMovingAgent.CharacterMainStates toggleSecondary()
    {

        if (!isInEquipingAction() && !isReloading() && m_pistol && !m_inWeaponAction)
        {
            m_animationSystem.setCurretnWeapon(0);
            // Current weapon equiped
            if (m_currentWeapon != null)
            {
                // Unequip weapon with animation
                if (m_currentWeapon.getWeaponType().Equals(RangedWeapon.WEAPONTYPE.secondary))
                {
                    m_inEquipingAction = true;
                    return m_animationSystem.unEquipEquipment();
                }
                // Fast toggle weapon
                else
                {
                    // Fast toggle
                    placeWeaponinHosterLocation(m_currentWeapon);
                    m_currentWeapon = m_pistol;
                    placeWeaponInHand(m_currentWeapon);
                    


                    m_animationSystem.fastEquipCurrentEquipment();

                    if (m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Aimed))
                    {
                        m_currentWeapon.setAimed(true);
                    }

                    return m_currentState;
                }
            }
            // Equip weapon with animation
            else
            {
                m_inEquipingAction = true;
                m_currentWeapon = m_pistol;
                return m_animationSystem.equipCurrentEquipment();
            }
        }
        // Unable to equip weapon - no weapon or mid action
        else
        {
            // No weapon avaialble
            if(m_currentWeapon == null && m_pistol == null && !m_inWeaponAction)
            {
                m_animationSystem.triggerShrug();
            }
            logWeaponIssue();
            return m_currentState;
        }
    }

    private void logWeaponIssue()
    {
        if(isInEquipingAction())
        {
            if(m_inEquipingAction)
            {
            }

            if(m_currentWeaponSubStage.Equals(WeaponSystemSubStages.Equiping))
            {
            }

            if(m_currentWeaponSubStage.Equals(WeaponSystemSubStages.UnEquiping))
            {
            }
        }

        if(m_inWeaponAction)
        {
            Debug.LogError("m_inWeaponAction flag is on");
        }

        if(isReloading())
        {
            Debug.LogError("Was reloading");
        }
    }

    public HumanoidMovingAgent.CharacterMainStates toggleGrenede()
    {
        Debug.LogError("Grenade switcing logic working. This is a bug");
       if(!isInEquipingAction() && !isReloading() && m_grenede && !m_inWeaponAction)
       {
           // Current Weapon avaialble
           if(m_currentWeapon != null)
           {
               // Unequip weapon with animation
                if (m_currentWeapon.getWeaponType().Equals(RangedWeapon.WEAPONTYPE.grenede))
                {
                    // Since there are not equiping animation, no need to have m_inEquipingAction enable
                    //m_inEquipingAction = true;
                    placeWeaponinHosterLocation(m_currentWeapon);
                    m_currentWeapon = null;


                    // If grenedes count is zero, remove the grenate from user wepons
                    // if(m_grenede.count ==0)
                    // {
                    //     GameObject.Destroy(m_grenede);
                    //     m_grenede = null;
                    // }
                    Debug.LogError("Grenade switcing logic working. This is a bug");

                    return m_animationSystem.unEquipEquipment();
                }
                // Weapon fast toggle
                else
                {
                    // if(m_grenede !=null && m_grenede.count >0 )
                    // {
                    //     placeWeaponinHosterLocation(m_currentWeapon);
                    //     m_currentWeapon = m_grenede;
                    //     placeWeaponInHand(m_currentWeapon);
                    //     m_animationSystem.setCurretnWeapon(2);
                    //     m_animationSystem.fastEquipCurrentEquipment();
                    // }
                    Debug.LogError("Grenade switcing logic working. This is a bug");
                    return m_currentState;
                }
           }
           // Equip with animation
        //    else if(m_grenede != null && m_grenede.count > 0)
        //    {
        //         Debug.LogError("Grenade switcing logic working. This is a bug");
        //         m_animationSystem.setCurretnWeapon(2);
        //         m_currentWeapon = m_grenede;
        //         placeWeaponInHand(m_currentWeapon);
        //         return m_animationSystem.equipCurrentEquipment();
        //    }
        //    else
        //    {
        //         Debug.LogError("Grenade switcing logic working. This is a bug");
        //         // Unable to equip or unequip no weapon
        //         if (m_grenede == null && m_currentWeapon == null && !m_inWeaponAction)
        //         {
        //             m_animationSystem.triggerShrug();
        //         }
        //     }
       }
       return m_currentState;
    } 
    #endregion

    #region Getters And Setters definition

    public bool is_weapon_empty()
    {
        if (m_currentWeapon == null)
            return false;

        if (m_currentWeapon.GetType().IsSubclassOf(typeof (RangedWeapon)))
        {
            return ((RangedWeapon)m_currentWeapon).isWeaponEmpty();
        }
        return false;
    }

    public bool isProperlyAimed()
    {
        return m_animationSystem.isProperlyAimed();
    }

    public void setCurrentWeapon(RangedWeapon currentWeapon)
    {
        this.m_currentWeapon = currentWeapon;
        m_currentWeapon.setGunTarget(m_target);
        m_currentWeapon.setOwnerFaction(m_agentData.m_agentFaction);
    }

    public void setCurretnWeaponProp(WeaponProp weaponProp)
    {
        //this.m_pistolProp = weaponProp;
    }

    public Weapon getCurrentWeapon()
    {
        return m_currentWeapon;
    }

    public void SetWeaponAmmoType(AmmoTypeEnums.WeaponTypes  weaponTypes,string ammo_name)
    {
        switch (weaponTypes)
        {
            case AmmoTypeEnums.WeaponTypes.Grenade:
            break;
            case AmmoTypeEnums.WeaponTypes.Rifle:
                if(m_rifle)
                {
                    var current_ammo = m_rifle.getLoadedAmmoCount();

                    // switch ammo name and make sure the ammo count is previous as before, any leftover ammo from current type is put back to the agent data and updated with new count
                   
                    var ammo_count = m_rifle.getLoadedAmmoCount();
                    m_agentData.AddAmmo(ammo_name,ammo_count);

                    m_rifle.SwitchAmmoType(ammo_name);
                    ammo_count = m_rifle.getLoadedAmmoCount();
                    m_agentData.AddAmmo(ammo_name,ammo_count);
                    m_rifle.setAmmoCount(m_agentData.useAmmoCount(ammo_name,current_ammo));

                    if(m_currentWeapon !=null && m_rifle != m_currentWeapon )
                    {
                        togglePrimary();
                    }
                }
            break;
            case AmmoTypeEnums.WeaponTypes.Pistol:
                if(m_pistol)
                {
                    var current_ammo = m_pistol.getLoadedAmmoCount();

                    // switch ammo name and make sure the ammo count is previous as before, any leftover ammo from current type is put back to the agent data and updated with new count
                       // put ammo already in type back                 
                    var ammo_count = m_pistol.getLoadedAmmoCount();
                    m_agentData.AddAmmo(ammo_name,ammo_count);

                    m_pistol.SwitchAmmoType(ammo_name);
                    ammo_count = m_pistol.getLoadedAmmoCount();
                    m_agentData.AddAmmo(ammo_name,ammo_count);
                    m_pistol.setAmmoCount(m_agentData.useAmmoCount(ammo_name,current_ammo));

                    if(m_currentWeapon !=null && m_pistol != m_currentWeapon )
                    {
                        toggleSecondary();
                    }
                }
            break;
        }
    }



    public int getCurrentWeaponLoadedAmmoCount()
    {
        int count = 0;

        if(m_currentWeapon)
        {
            //m_agentParameters.weaponAmmoCount.TryGetValue(m_currentWeapon.name,out count);
            switch(m_currentWeapon.getWeaponType())
            {
                case Weapon.WEAPONTYPE.primary:
                case Weapon.WEAPONTYPE.secondary:
                    count =  ((RangedWeapon)m_currentWeapon).getLoadedAmmoCount();
                break;

                case Weapon.WEAPONTYPE.grenede:
                    //count = ((Grenade)m_currentWeapon).count;
                    Debug.LogError("This is a bug");
                break;
            }

        }

        return count;
    }

    public int getCurrentWeaponMagazineSize()
    {
        int count = 0;

        if (m_currentWeapon)
        {
            //m_agentParameters.weaponAmmoCount.TryGetValue(m_currentWeapon.name,out count);
            switch (m_currentWeapon.getWeaponType())
            {
                case Weapon.WEAPONTYPE.primary:
                case Weapon.WEAPONTYPE.secondary:
                    count = ((RangedWeapon)m_currentWeapon).m_magazineSize;
                    break;

                case Weapon.WEAPONTYPE.grenede:
                    break;
            }

        }

        return count;
    }

    public void setWeaponTarget(GameObject target)
    {
        m_currentWeapon.setGunTarget(target);
    }

    public GameObject getTarget()
    {
        return m_target;
    }

    public bool isEquiped()
    {
        return m_animationSystem.isEquiped();
    }

    private void getAllWeapons(WeaponProp[] props)
    {       
        foreach (WeaponProp prop in props)
        {
            WeaponProp.WeaponLocation type = prop.getPropType();

            switch (type)
            {
                case WeaponProp.WeaponLocation.HOSTER_PRIMARY:
                    primaryHosterLocation = prop.gameObject;
                    break;

                case WeaponProp.WeaponLocation.HOSTER_SECONDAY:
                    secondaryHosterLocation = prop.gameObject;
                    break;
                case WeaponProp.WeaponLocation.RIGHT_HAND:
                    weaponHoldLocation = prop.gameObject;
                    break;
                case WeaponProp.WeaponLocation.LEFT_HAND:
                    grenadeHoldLocation = prop.gameObject;
                    break;
            }
        }

        if(m_agentData.primaryWeapon)
        {
            /*
            m_rifle = m_agentData.primaryWeapon;
            m_rifle.setAimed(false);   
            m_rifle.setOwnerFaction(m_agentData.m_agentFaction);
            m_rifle.setGunTarget(m_target);
            m_rifle.addOnWeaponFireEvent(OnWeaponFire);
            m_rifle.targetPointTransfrom = m_agentComponents.weaponAimTransform;
            placeWeaponinHosterLocation(m_rifle);
            */
            equipWeapon(m_agentData.primaryWeapon);
            m_agentData.primaryWeapon.OnEquipAction(); 
        }

        if(m_agentData.secondaryWeapon)
        {
            
            //m_pistol = m_agentData.secondaryWeapon;
            //m_pistol.setAimed(false);
            //m_pistol.setOwnerFaction(m_agentData.m_agentFaction);
            //m_pistol.setGunTarget(m_target);
            //m_pistol.addOnWeaponFireEvent(OnWeaponFire);
           // m_pistol.targetPointTransfrom = m_agentComponents.weaponAimTransform;
            //placeWeaponinHosterLocation(m_pistol);
            
            equipWeapon(m_agentData.secondaryWeapon);
            m_agentData.secondaryWeapon.OnEquipAction();
        }

        if(m_agentData.grenade)
        {
            m_grenede = m_agentData.grenade;
            m_grenede.setGunTarget(m_target);
            m_grenede.OnEquipAction();
        }
    }

    public bool isInEquipingAction()
    {
        return m_inEquipingAction || m_currentWeaponSubStage.Equals(WeaponSystemSubStages.Equiping) || m_currentWeaponSubStage.Equals(WeaponSystemSubStages.UnEquiping);
    }

    public void setOwnerFaction(AgentBasicData.AgentFaction agentGroup)
    {
        m_agentData.m_agentFaction = agentGroup;

        if(m_rifle)
        {
            m_rifle.setOwnerFaction(agentGroup);
        }

        if(m_pistol)
        {
            m_pistol.setOwnerFaction(agentGroup);           
        }
    }

    public void resetWeapon()
    {
        m_currentWeapon.resetWeapon();
    }

    public int getPrimaryWeaponAmmoCount()
    {
        return m_rifle.getLoadedAmmoCount();
    }

    public int getSecondaryWeaponAmmoCount()
    {
        return m_pistol.getLoadedAmmoCount();
    }

    public int getGrenadeCount()
    {
        
        if(m_grenede == null)
        {
            return 0;
        }
        return m_agentData.checkUnloadAvaialbleAmmo(m_grenede.getCurrentGrenateType());
    }

    public void setPrimayWeaponAmmoCount(int count)
    {
        m_rifle.setAmmoCount(count);
    }

    public void setSecondaryWeaponAmmoCount(int count)
    {
        m_pistol.setAmmoCount(count);
    }

    public void set_is_owner_moving(bool moving)
    {
        if (m_currentWeapon)
            m_currentWeapon.set_is_owner_moving(moving);
    }

    public bool isReloading()
    {
        if(m_currentWeapon)
        {
            switch(m_currentWeapon.getWeaponType())
            {
                case Weapon.WEAPONTYPE.primary:
                case Weapon.WEAPONTYPE.secondary:
                    return ((RangedWeapon)m_currentWeapon).isReloading();
                case Weapon.WEAPONTYPE.grenede:
                    return false;
                default:
                    return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void placeWeaponinHosterLocation(Weapon weapon)
    {
            Transform hosteringLocation = null;
            System.Type weaponType = weapon.GetType();

            if(weaponType == typeof(PrimaryWeapon))
            {
                hosteringLocation = primaryHosterLocation.transform;
            }
            else if (weaponType == typeof(SecondaryWeapon))
            {
                hosteringLocation = secondaryHosterLocation.transform;
            }
            else if(weaponType == typeof(Grenade))
            {
                hosteringLocation = grenadeHoldLocation.transform;
            }
            
            weapon.transform.parent = hosteringLocation;
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            weapon.OnPlaceOnHoster();
    }

    public void GrenadeQuickThrow()
    {
        if(m_grenede != null && getGrenadeCount() > 0)
        {
            placeWeaponinHosterLocation(m_grenede);
            OnThrow();
            //m_grenede.transform.position = new Vector3(0,-2,0);
        }
    }
    public void placeWeaponInHand(Weapon weapon)
    {
        weapon.transform.parent = weaponHoldLocation.transform;
        weapon.transform.localPosition = Vector3.zero + weapon.nonFunctionalProperties.handPlacementOffset;
        weapon.transform.localRotation = Quaternion.identity;
        m_recoil.handRotationOffset = weapon.nonFunctionalProperties.weaponRecoilOffset;  
        weapon.transform.localScale = Vector3.one;  
        weapon.OnPlaceOnHand();   
    }

    public Transform getWeaponHoldTransfrom()
    {
        return weaponHoldLocation.transform;
    }

    public RangedWeapon GetPrimaryWeapon()
    {
        return m_rifle;
    }

    public RangedWeapon GetSecondaryWeapon()
    {
        return m_pistol;
    }

    // public void SetGrenadeCount(int value)
    // {
    //     m_grenede.count += value;
    // }

    public void equipWeapon(Weapon weapon)
    {
        bool need_to_place_in_hand = false;
        switch (weapon.getWeaponType())
        {
            case Weapon.WEAPONTYPE.primary:
                

                if (m_rifle !=null && m_currentWeapon !=null && m_currentWeapon == m_rifle && m_currentWeaponSubStage == WeaponSystemSubStages.Armed)
                {
                    need_to_place_in_hand = true;
                }

                if (m_rifle)
                {
                    m_rifle.dropWeapon();
                    m_rifle = null;
                }

                m_rifle = (PrimaryWeapon)weapon;
                

                if(need_to_place_in_hand)
                {
                    m_currentWeapon = weapon;
                    placeWeaponInHand(m_currentWeapon);
                    if (m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Aimed))
                    {
                        m_currentWeapon.setAimed(true);
                    }
                }
                else
                {
                    placeWeaponinHosterLocation(weapon);
                }
                
                m_rifle.onWeaponEquip();
                ((RangedWeapon)weapon).addOnWeaponFireEvent(OnWeaponFire);
                m_rifle.targetPointTransfrom = m_agentComponents.weaponAimTransform;
            break;
            case Weapon.WEAPONTYPE.secondary:

                if (m_pistol !=null && m_currentWeapon !=null && m_currentWeapon == m_pistol && m_currentWeaponSubStage == WeaponSystemSubStages.Armed)
                {
                    need_to_place_in_hand = true;
                }

                if (m_pistol)
                {
                    m_pistol.dropWeapon();
                    m_pistol = null;
                }

                m_pistol = (SecondaryWeapon)weapon;

                if (need_to_place_in_hand)
                {
                    m_currentWeapon = weapon;
                    placeWeaponInHand(m_currentWeapon);
                    if (m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Aimed))
                    {
                        m_currentWeapon.setAimed(true);
                    }
                }
                else
                {
                    placeWeaponinHosterLocation(weapon);
                }


                m_pistol.onWeaponEquip();
                ((RangedWeapon)weapon).addOnWeaponFireEvent(OnWeaponFire);
                m_pistol.targetPointTransfrom = m_agentComponents.weaponAimTransform;
            break;
            case Weapon.WEAPONTYPE.grenede:
                Debug.LogError("This is a bug");
                // if(m_grenede)
                // {
                //     m_grenede.dropWeapon();
                //     ((Grenade)weapon).count +=m_grenede.count;
                //     GameObject.Destroy(m_grenede);
                //     m_grenede = null;
                // }
                // m_grenede = (Grenade)weapon;
                // m_grenede.onWeaponEquip();
                // placeWeaponinHosterLocation(weapon);
                //m_grenede.targetPointTransfrom = m_agentComponents.lookAimTransform;
            break;
        }

        weapon.setAimed(false);
        weapon.setOwnerFaction(m_agentData.m_agentFaction);
        weapon.setGunTarget(m_target);
    }

    #endregion
}