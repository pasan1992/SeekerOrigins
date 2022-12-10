using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(HumanoidMovingAgent))]
public class HumanoidMovingAgent : MonoBehaviour, ICyberAgent
{
    #region parameters

    // Callback
    private GameEvents.BasicNotifactionEvent m_onDestoryCallback;
    private GameEvents.BasicNotifactionEvent m_onDisableCallback;
    private GameEvents.BasicNotifactionEvent m_onEnableCallback;

    private GameEvents.BasicNotifactionEvent m_onHeal;

    // Main Modules
    protected HumanoidRangedWeaponsModule m_equipmentModule;
    protected HumanoidAnimationModule m_animationModule;
    protected HumanoidMovmentModule m_movmentModule;
    protected HumanoidDamageModule m_damageModule;
    protected HumanoidInteractionModule m_interactionModule;

    // Attributes
    public enum CharacterMainStates { Aimed, Armed_not_Aimed, Dodge, Idle,Interaction,Stunned,MeeleAttack,Shock,Point }
    private HashSet<CharacterMainStates> effectStates = new HashSet<CharacterMainStates>{CharacterMainStates.Stunned,CharacterMainStates.MeeleAttack,CharacterMainStates.Shock,
    CharacterMainStates.Point};
    public CharacterMainStates m_characterState = CharacterMainStates.Idle;
    private CharacterMainStates m_previousTempState = CharacterMainStates.Idle;
    protected GameObject m_target;
    private bool m_characterEnabled = true;
    private Vector3 m_movmentVector;
    private bool m_isDisabled = false;
    private GameEvents.BasicNotifactionEvent m_onDamagedCallback;
    // Public
    // Main Agent data component
    public AgentData AgentData;


    // These components required for the function of the agent
    public AgentFunctionalComponents AgentComponents;


    //Event Callbacks
    private GameEvents.BasicNotifactionEvent m_onReloadEndCallback;
    private GameEvents.BasicNotifactionEvent m_onInteractionEndCallback;

    private GameEvents.BasicNotifactionEvent m_onWeaponEquipCallback;
    private GameEvents.BasicNotifactionEvent m_onWeaponUnEquipCallback;
    private GameEvents.BasicNotifactionEvent m_onThrowCallback;
    

    private Renderer m_renderer;
    private IEnumerator m_previousCorutine;


    #endregion

    #region Initalize
    public virtual void Awake()
    {
        var scene = SceneManager.GetActiveScene();

        m_target = new GameObject();
        m_movmentVector = new Vector3(0, 0, 0);
        AgentData.InitalizeAmmo();

        // Create Animation system.
        AimIK aimIK = this.GetComponent<AimIK>();
        aimIK.solver.target = m_target.transform;
        m_animationModule = new HumanoidAnimationModule(this.GetComponent<Animator>(), this.GetComponent<AimIK>(),AgentComponents, 10);

        // Create equipment system.
        RangedWeapon[] currentWeapons = this.GetComponentsInChildren<RangedWeapon>();
        WeaponProp[] currentWeaponProps = this.GetComponentsInChildren<WeaponProp>();
        
        m_equipmentModule = new HumanoidRangedWeaponsModule(currentWeaponProps, m_characterState, m_target, GetComponent<Recoil>(), m_animationModule,AgentData,AgentComponents);

        // Create movment system.
        NavMeshAgent navMeshAgent = this.GetComponent<NavMeshAgent>();

        m_movmentModule = new HumanoidMovmentModule(this.transform, m_characterState, m_target, m_animationModule,navMeshAgent);

        // Create Damage module
        m_damageModule = new HumanoidDamageModule(AgentData, 
        this.GetComponent<RagdollUtility>(), 
        this.GetComponentInChildren<HitReaction>(),
        m_animationModule, findHeadTransfrom(), 
        findChestTransfrom(), 
        destroyCharacter, 
        this.GetComponentInChildren<Outline>());

        // Create intearction system.
        m_interactionModule = new HumanoidInteractionModule(m_animationModule,
        m_movmentModule,
        AgentData,
        m_equipmentModule,
        navMeshAgent,
        this,
        // This is the callback for interaction done.
        OnInteractionDone,
        m_equipmentModule.getWeaponHoldTransfrom()
        );
        m_renderer = this.GetComponentInChildren<Renderer>();
        
    }
    #endregion

    #region Updates
    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_characterEnabled)
        {
                // Update Systems.
            m_animationModule.UpdateAnimationState(m_characterState);
            m_movmentModule.UpdateMovment((int)m_characterState, m_movmentVector);
            m_equipmentModule.UpdateSystem(m_characterState);
            m_damageModule.update();
        }
    }
    #endregion

    #region Commands

    private bool isEffectState()
    {
        return effectStates.Contains(m_characterState);
    }

    private bool isEffectState(CharacterMainStates state)
    {
        return effectStates.Contains(state);
    }

    /**
     Interact with given interatable object
     - Interaction type can be different from the interaction type of the interatable object given.
    */
    public bool interactWith(Interactable obj,Interactable.InteractableProperties.InteractableType type)
    {
        bool interactCondition =( (m_characterState.Equals(CharacterMainStates.Idle) || 
        m_characterState.Equals(CharacterMainStates.Armed_not_Aimed)) && !m_characterState.Equals(CharacterMainStates.Interaction) && !isEffectState());
        // Check if character is ready to interact - Do not interact if the character is already interacting.
        if(interactCondition)
        {
            // Check if the object in interaction is already in interacting state by other agent.
            if(obj != null && !obj.isInteracting())
            {
                if(isArmed() && (type == Interactable.InteractableProperties.InteractableType.ContinousInteraction 
                    || type == Interactable.InteractableProperties.InteractableType.TimedInteraction || 
                    type == Interactable.InteractableProperties.InteractableType.DialogInteraction
                    || type == Interactable.InteractableProperties.InteractableType.FixedContinousInteraction) )
                {
                    m_interactionModule.setPreviousWeapon();
                    hosterWeapon();
                }
                //togglepSecondaryWeapon();

                m_movmentVector = Vector3.zero;
                m_previousTempState = m_characterState;
                m_characterState = CharacterMainStates.Interaction;
                StartCoroutine(m_interactionModule.interactWith(obj,type));
            }
        }     
        return interactCondition; 
    }

    public void consume_ammo_pack(AmmoPack ammo_pack)
    {
        m_interactionModule.ConsumeAmmoPack(ammo_pack);
    }

    /**
     Cancle the current interaction
    */
    public void cancleInteraction()
    {
        m_interactionModule.cancleInteraction();
    }

    public void skipInteraction()
    {
        m_interactionModule.SkipDialog();
    }
    public void Interact(bool isPlayer)
    {
        if(isEffectState())
            return;
        Interactable obj = AgentItemFinder.findNearItem(getCurrentPosition(),isPlayer);
        if(obj)
        {
            interactWith(obj,obj.properties.Type);
        }
    }

    public bool InteractWith(Interactable obj) {
         if(obj)
        {
            return interactWith(obj,obj.properties.Type);
        }    
        return false;   
    }

    public void damageAgent(CommonFunctions.Damage amount)
    {
        m_damageModule.DamageByAmount(amount);

        if(m_onDamagedCallback != null)
        {
            m_onDamagedCallback();
        }
        
    }

    // This fire wepon is called by the AI Agent controlers to fire weapon.
    public virtual void weaponFireForAI()
    {
        if(isEffectState())
            return;
        if(m_equipmentModule.isReloading())
        {
            return;
        }

        if(m_equipmentModule.getCurrentWeaponLoadedAmmoCount() > 0)
        {
            StartCoroutine(fireWeapon());
            return;
        }
        var reload_done = m_equipmentModule.reloadCurretnWeapon();

        if(!reload_done)
        {
            m_equipmentModule.toggleSecondary();
        }
    }

    public void reloadCurretnWeapon()
    {
        if(isEffectState())
            return;
        m_equipmentModule.reloadCurretnWeapon();
    }

    // Aim Current Weapon 
    public virtual void aimWeapon()
    {
        if(isEffectState())
            return;
        if (m_characterState.Equals(CharacterMainStates.Armed_not_Aimed) && !isEquipingWeapon() && !isInteracting())
        {
            
            m_characterState = CharacterMainStates.Aimed;
            m_equipmentModule.getCurrentWeapon().setAimed(true);
        }
    }

    // Stop Aiming current Weapon.
    public virtual void stopAiming()
    {
        if (m_characterState.Equals(CharacterMainStates.Aimed) && !isEffectState())
        {
            m_characterState = CharacterMainStates.Armed_not_Aimed;
            m_equipmentModule.getCurrentWeapon().setAimed(false);
        }
    }

    // Move character
    public virtual void moveCharacter(Vector3 movmentDirection)
    {
        if(isEffectState() )
        {
            return;
        }
            

        m_movmentVector = movmentDirection;

        if (movmentDirection == Vector3.zero)
        {
            m_equipmentModule.set_is_owner_moving(false);
            MouseCurserSystem.getInstance().is_moving_player(false);
            return;
        }
        m_equipmentModule.set_is_owner_moving(true);
        MouseCurserSystem.getInstance().is_moving_player(true);
    }

    // Destory Character
    private void destroyCharacter()
    {
        this.GetComponent<FullBodyBipedIK>().enabled = false;
        //m_equipmentModule.DropCurrentWeapon();
        m_equipmentModule.DropAllWeapons();
        m_characterEnabled = false;
        m_animationModule.disableAnimationSystem();
        //Invoke("postDestoryEffect", 1);

        if (m_onDestoryCallback != null)
        {
           // Debug.Log("Destroy Get called");
            m_onDestoryCallback();
        }
    }

    private void damageAgent()
    {
        switch(AgentData.AgentNature)
        {
            case AgentData.AGENT_NATURE.DROID:
                m_damageModule.emitSmoke();
            break;            
        }            
    }

    public void toggleHide()
    {
        m_animationModule.toggleCrouched();
    }

    public void togglePrimaryWeapon()
    {
        if(isEffectState())
            return;
        // To make sure that weapon toggle won't happen mid interaction.
        if(!m_characterState.Equals(CharacterMainStates.Interaction))
        {
             // !improtant. Returns the character state after the toggle.
            m_characterState = m_equipmentModule.togglePrimary();
        }
        
    }

    public void togglepSecondaryWeapon()
    {
        if(isEffectState())
            return;
        // To make sure that weapon toggle won't happen mid interaction.
        if(!m_characterState.Equals(CharacterMainStates.Interaction))
        {
             // !important Returns the character state after the toggle
            m_characterState = m_equipmentModule.toggleSecondary();
        }
       
    }

    public void toggleGrenede()
    {
        if(isEffectState())
            return;
        // To make sure that weapon toggle won't happen mid interaction.
        if (!m_characterState.Equals(CharacterMainStates.Interaction))
        {
            // !important Returns the character state after the toggle
            m_characterState = m_equipmentModule.toggleGrenede();
        }     
    }

    public void reactOnHit(Collider collider, Vector3 force, Vector3 point)
    {
        m_damageModule.reactOnHit(collider, force, point);
    }

    public void dodgeAttack(Vector3 dodgeDirection)
    {
        if(isEffectState())
            return;
        if (!m_characterState.Equals(CharacterMainStates.Dodge))
        {
            m_characterState = CharacterMainStates.Dodge;
            m_animationModule.triggerDodge();
            m_movmentModule.dodge(dodgeDirection);
            m_equipmentModule.releaseTrigger();
        }
    }

    public void releaseTrigger()
    {
        m_equipmentModule.releaseTrigger();
    }

    public void pullTrigger()
    {
        if(isEffectState())
            return;
        m_equipmentModule.pullTrigger();
    }

    public void lookAtTarget()
    {
        m_movmentModule.lookAtTarget();
    }

    #endregion

    #region Getters and Setters

    public void setOnAmmoPickupCallback(GameEvents.OnAmmoPickupEvent onAmmoPickup)
    {
        m_interactionModule.setOnAmmoPickupCallback(onAmmoPickup);
    }

    public void setOnWeaponPickupEvent(GameEvents.OnInteractionEvent wp_event)
    {
        m_interactionModule.setOnWeaponPickupEvent(wp_event);
    }
    public bool isVisibleOnScreen()
    {
        return m_renderer.isVisible;
    }

    public void setOnDamagedCallback(GameEvents.BasicNotifactionEvent callback)
    {
        m_onDamagedCallback += callback;
    }

    public Transform getChestTransfrom()
    {
        return m_damageModule.getChestTransfrom();
    }

    public Vector3 getMovmentDirection()
    {
        return m_movmentVector;
    }

    public void resetAgent()
    {
        m_damageModule.resetCharacter();
        m_animationModule.enableAnimationSystem();
        m_characterEnabled = true;
        m_equipmentModule.resetWeapon();
        this.GetComponent<FullBodyBipedIK>().enabled = true;
    }

    public void setOnDestoryCallback(GameEvents.BasicNotifactionEvent onDestoryCallback)
    {
        m_onDestoryCallback += onDestoryCallback;
    }

    public void setOnDisableCallback(GameEvents.BasicNotifactionEvent callback)
    {
        m_onDisableCallback += callback;
    }

    public void setOnEnableCallback(GameEvents.BasicNotifactionEvent callback)
    {
        m_onEnableCallback += callback;
    }

    public void setOnHealCallback(GameEvents.BasicNotifactionEvent callback)
    {
        m_onHeal +=callback;
        m_damageModule.setOnHealCallback(callback);
    }

    public bool IsFunctional()
    {
        return m_damageModule.HealthAvailable();
    }

    public bool isReadyToAim()
    {
        return m_animationModule.isEquiped() && (m_characterState.Equals(CharacterMainStates.Armed_not_Aimed) || m_characterState.Equals(CharacterMainStates.Aimed)) && m_equipmentModule.is_ready_to_aim();
    }

    // public bool hasWeaponInHand()
    // {
    //     return m_animationModule.isEquiped();
    // }

    public bool isArmed()
    {
        return m_animationModule.isEquiped();
    }

    public void setTargetPoint(Vector3 position)
    {
        if(Vector3.Distance(position,this.transform.position)>1.65f)
        {
            m_target.transform.position = position;
        }
        
    }

    public void SET_TARGET(Vector3 position)
    {
         if(Vector3.Distance(position,this.transform.position)>1.65f)
        {
            m_target.transform.position = position;
        }       
    }

    public bool isEquipingWeapon()
    {
        return m_equipmentModule.isInEquipingAction();
    }

    // public bool isEquiped()
    // {
    //     return (m_animationModule.isEquiped() || m_equipmentModule.isInEquipingAction());
    // }

    public void enableTranslateMovment(bool enable)
    {
        m_movmentModule.setPhysicalLocationChange(enable);
    }

    public Vector3 getCurrentPosition()
    {
        if(this.transform !=null)
        {
            return this.transform.position;
        }
        return Vector3.zero;
    }

    public Vector3 getTopPosition()
    {
        return m_damageModule.getHeadTransfrom().position;
    }

    public Transform getHeadTransfrom()
    {
        return m_damageModule.getHeadTransfrom();
    }

    public virtual void setWeponFireCapability(bool enadled)
    {
        if (m_equipmentModule.getCurrentWeapon() != null)
        {
            m_equipmentModule.getCurrentWeapon().setWeaponSafty(!enadled);
        }
    }

    public bool isCrouched()
    {
        return m_movmentModule.isCrouched();
    }

    public AgentBasicData.AgentFaction getFaction()
    {
        return AgentData.m_agentFaction;
    }

    public void setFaction(AgentBasicData.AgentFaction group)
    {
        AgentData.m_agentFaction = group;

        if (m_equipmentModule != null)
        {
            m_equipmentModule.setOwnerFaction(group);
        }
    }

    public void SwitchAmmoType(AmmoTypeEnums.WeaponTypes weapon, string ammoType)
    {
        m_equipmentModule.SetWeaponAmmoType(weapon,ammoType);
    }

    public CharacterMainStates getCharacterMainStates()
    {
        return m_characterState;
    }

    public Transform getTransfrom()
    {
        return this.transform;
    }

    public void setSkill(float skill)
    {
        AgentData.Skill = skill;
    }

    public float getSkill()
    {
        return AgentData.Skill;
    }

    public bool isAimed()
    {
        return m_equipmentModule.isProperlyAimed();
    }

    public bool isDisabled()
    {
        return m_isDisabled || isEffectState();
    }

    public int getPrimaryWeaponAmmoCount()
    {
        return m_equipmentModule.getPrimaryWeaponAmmoCount();
    }

    public int getSecondaryWeaponAmmoCount()
    {
        return m_equipmentModule.getSecondaryWeaponAmmoCount();
    }

    public int getCurrentWeaponLoadedAmmoCount()
    {
        return m_equipmentModule.getCurrentWeaponLoadedAmmoCount();
    }

    public int getCurrentWeaponMagazineSize()
    {
        return m_equipmentModule.getCurrentWeaponMagazineSize();
    }

    public void setPrimayWeaponAmmoCount(int count)
    {
        m_equipmentModule.setPrimayWeaponAmmoCount(count);
    }

    public void setSecondaryWeaponAmmoCount(int count)
    {
        m_equipmentModule.setSecondaryWeaponAmmoCount(count);
    }

    public Weapon getCurrentWeapon()
    {
       return m_equipmentModule.getCurrentWeapon();
    }

    public RangedWeapon GetPrimaryWeapon()
    {
        return m_equipmentModule.GetPrimaryWeapon();
    }

    public RangedWeapon GetSecondaryWeapon()
    {
        return m_equipmentModule.GetSecondaryWeapon();
    }


    public RangedWeapon.WEAPONTYPE getCurrentWeaponType()
    {
        return m_equipmentModule.getCurrentWeapon().getWeaponType();
    }

    public bool is_currentWeaponEmpty()
    {
        return m_equipmentModule.is_weapon_empty();
    }

    // Return true if weapon is already hosted or actualy wepon is husted from this command
    public bool hosterWeapon()
    {
        if(isEffectState())
            return false;
        if((m_characterState.Equals(CharacterMainStates.Armed_not_Aimed) || m_characterState.Equals(CharacterMainStates.Aimed)) && !m_equipmentModule.isInEquipingAction() && !m_equipmentModule.isReloading())
        {
            switch (m_equipmentModule.getCurrentWeapon().getWeaponType())
            {
                case RangedWeapon.WEAPONTYPE.grenede:
                    toggleGrenede();
                break;
                case Weapon.WEAPONTYPE.primary:
                    togglePrimaryWeapon();
                break;
                case Weapon.WEAPONTYPE.secondary:
                    togglepSecondaryWeapon();
                break;
            }
            return true;
        }
        else if(m_characterState.Equals(CharacterMainStates.Idle))
        {
            return true;
        }
        return false;
    }

    public IEnumerator waitTillUnarmed()
    {
        while(!this.hosterWeapon())
        {
            yield return null;
        }
    }

    #endregion

    #region Events Handlers

    public void HandInPosition()
    {
        m_interactionModule.HandInPosition();
    }

    public void OnInteractionDone()
    {
        if(m_characterState == CharacterMainStates.Interaction)
        {
            m_characterState = m_previousTempState;
            OnInteractionDone();
        }      
    }

    public void OnThrow()
    {
        if(isEffectState())
            return;
        //m_equipmentModule.OnThrow();
        m_equipmentModule.GrenadeQuickThrow();
    }

    public void Throw()
    {
        if(isEffectState())
            return;
        if(m_equipmentModule.getGrenadeCount() > 0)
        {
            m_animationModule.triggerThrow();
        }     
    }

    public int GetGrenadeCount()
    {
        if (m_equipmentModule == null)
        {
            return 0;
        }
        return m_equipmentModule.getGrenadeCount();
    }


    public void ReloadEnd()
    {
        m_equipmentModule.ReloadEnd();

        if(m_onReloadEndCallback != null)
        {
            m_onReloadEndCallback();
        }
        
    }

    public void EquipAnimationEvent()
    {
        m_equipmentModule.EquipAnimationEvent();

        if(m_onWeaponEquipCallback != null)
        {
            m_onWeaponEquipCallback();
        }
        
    }

    public void UnEquipAnimationEvent()
    {
        m_equipmentModule.UnEquipAnimationEvent();

        if(m_onWeaponUnEquipCallback != null)
        {
            m_onWeaponUnEquipCallback();
        }
    }

    public bool isCharacterEnabled()
    {
        return m_characterEnabled;
    }


    public bool isHidden()
    {
        return m_movmentModule.isCrouched();
    }

    public void DodgeEnd()
    {
        if(isDisabled())
            return;
            
        if (m_animationModule.isEquiped())
        {
            m_characterState = CharacterMainStates.Armed_not_Aimed;
            return;
        }
        m_characterState = CharacterMainStates.Idle;
    }
    #endregion

    #region Helper Functions

    private IEnumerator fireWeapon()
    {
        yield return new WaitForSeconds(Random.Range(0.4f,0.7f));
        m_equipmentModule.pullTrigger();
        yield return new WaitForSeconds(Random.Range(0.4f,0.7f));
        m_equipmentModule.releaseTrigger();
    }

    public void Heal()
    {
        if(AgentData.Health > 0 & AgentData.HealInjectionCount > 0)
        {
            AgentData.Health += AgentData.HealPerInjection;
            if(AgentData.Health > AgentData.MaxHealth)
            {
                AgentData.Health = AgentData.MaxHealth;
            }
            AgentData.HealInjectionCount -=1;

            if(m_onHeal !=null)
            {
                m_onHeal();
            }
        }
    }

    public int GetHeadCount()
    {
        return AgentData.HealInjectionCount;
    }

    private Transform findHeadTransfrom()
    {
        Transform headTransfrom = null;
        foreach (Rigidbody rb in this.GetComponentsInChildren<Rigidbody>())
        {
            if (rb.tag == "Head")
            {
                headTransfrom = rb.transform;
            }
        }
        return headTransfrom;
    }

    private Transform findChestTransfrom()
    {
        Transform headTransfrom = null;
        foreach (Rigidbody rb in this.GetComponentsInChildren<Rigidbody>())
        {
            if (rb.tag == "Chest")
            {
                headTransfrom = rb.transform;
            }
        }

        return headTransfrom;
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public GameObject getGameObject()
    {
        return this.transform.gameObject;
    }

    public Vector3 getTargetPosition()
    {
        return m_target.transform.position;
    }

    public Transform getTargetTransfrom()
    {
        return m_target.transform;
    }

    public bool isInteracting()
    {
        return m_characterState == CharacterMainStates.Interaction;
    }

    public Interactable.InteractableProperties.InteractableType getCurrentInteractionType()
    {
        if(isInteracting())
        {
            return m_interactionModule.GetCurrentInteractable().properties.Type;
        }
        else
        {
            return Interactable.InteractableProperties.InteractableType.NullInteraction;
        }
    }    

    public void setAnimationSpeed(float speed)
    {
        m_animationModule.setAnimationSpeed(speed);
    }

    public Vector3 getCurrentVelocity()
    {
        return m_movmentModule.getCurrentVelocity();
    }

    public AgentData GetAgentData()
    {
        return AgentData;
    }
    public void setOnEquipCallback(GameEvents.BasicNotifactionEvent callback)
    {
        m_onWeaponEquipCallback += callback;
    }

    public void setOnUnEquipCallback(GameEvents.BasicNotifactionEvent callback)
    {
        m_onWeaponUnEquipCallback += callback;
    }

    public void StopMovment()
    {
        m_animationModule.Stop();
    }

    private void setEffectState(CharacterMainStates state, float duration, Vector3 direction)
    {
        if(!isEffectState(state))
        {
            return;
        }

        if(isDisabled() || m_characterState == CharacterMainStates.Dodge)
            return;
        
        if(!isEffectState())
        {
            m_previousTempState = m_characterState;
        }

        m_characterState = state;

        m_animationModule.setEffectState(true,state);
        if(GetAgentData().AgentNature == AgentBasicData.AGENT_NATURE.DROID)
        {
            m_damageModule.emitSmoke();
        }

        if(m_previousCorutine !=null)
        {
            StopCoroutine(m_previousCorutine);
        }
        
        m_previousCorutine = waitAndRemoveStun(duration,state);
        StartCoroutine(m_previousCorutine);
        m_movmentModule.setMovment(false);
        m_movmentVector = Vector3.zero;
        m_movmentModule.LookAtObject(this.transform.position - direction);
    }

    public void setStunned(float duration, Vector3 direction)
    {
        setEffectState(CharacterMainStates.Stunned,duration,direction);
    }

    public void setShocked(float duration, Vector3 direction)
    {
        setEffectState(CharacterMainStates.Shock,duration,direction);
    }

    public void setPoint(float duration, Vector3 direction)
    {
        setEffectState(CharacterMainStates.Point,duration,direction);
    }

    IEnumerator waitAndRemoveStun(float waitDuration,CharacterMainStates state)
    {
        yield return new WaitForSeconds(waitDuration);
        m_animationModule.setEffectState(false,state);
        yield return new WaitForSeconds(0.5f);
        removeStun();
    }
    public void removeStun()
    {
        
        m_characterState = m_previousTempState;  
        m_movmentModule.setMovment(true);  
    }

    public void MeleteAttack(float duration,Vector3 direction)
    {
        setEffectState(CharacterMainStates.MeeleAttack,duration,direction);
    }


    public void setBasicCallbacks(
        GameEvents.BasicNotifactionEvent onEquip,
        GameEvents.BasicNotifactionEvent onUnequip,
        GameEvents.BasicNotifactionEvent onReloadEnd,
        GameEvents.BasicNotifactionEvent onDamaged,
        GameEvents.BasicNotifactionEvent onDestoryCallback,
        GameEvents.BasicNotifactionEvent onInteractionDone,
        GameEvents.BasicNotifactionEvent onThrowItem
    )
    {
        m_onWeaponEquipCallback +=onEquip;
        m_onWeaponUnEquipCallback += onUnequip;
        m_onDestoryCallback +=onDestoryCallback;
        m_onDamagedCallback +=onDamaged;
        this.m_onInteractionEndCallback += onInteractionDone;
        this.m_onThrowCallback += onThrowItem;
        // if(onDestoryCallback !=null)
        // {
        //     m_onDestoryCallback +=onDestoryCallback;
        // }

        // if(onEquip != null)
        // {
            
        // }

        // if(onUnequip != null)
        // {

        // }

        // if(onReloadEnd != null)
        // {

        // }

        // if(onDamaged != null)
        // {

        // }        
        // if(onDestoryCallback != null)
        // {

        // }
    }
    #endregion

    #region Commented Code
    #endregion
}

