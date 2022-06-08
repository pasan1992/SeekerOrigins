using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HumanoidMovingAgent))]
public class AutoHumanoidAgentController :  AgentController
{
    public enum WeaponType { PRIMARY,SECONDAY};
    public HumanoidMovingAgent target;
    public WeaponType preferedWeapon = WeaponType.PRIMARY;

    // public HumanoidMovingAgent followingTarget;    
    protected HumanoidMovingAgent m_movingAgent;
    protected ICharacterBehaviorState m_currentState;
    protected ICharacterBehaviorState m_combatStage;
    protected ICharacterBehaviorState m_idleStage;
    protected HumanoidAgentBasicVisualSensor m_visualSensor;
    protected NavMeshAgent m_navMeshAgent;


    // private GameObject selfCoverPoint;
    //public float health;
    public WaypointRutine rutine;
    private HealthBar m_healthBar;

    private bool m_forced_attack = false;
    private Transform m_force_transfrom = null;
    #region initaialize

    private void Awake()
    {
        m_movingAgent = this.GetComponent<HumanoidMovingAgent>();
        m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        m_healthBar = this.GetComponentInChildren<HealthBar>();
    }

    void Start()
    {
        inializeGuard();

        // Initalize Agent and sensors
        m_visualSensor = new HumanoidAgentBasicVisualSensor(m_movingAgent);
        m_visualSensor.setOnEnemyDetectionEvent(onEnemyDetection);
        m_visualSensor.setOnAllClear(onAllClear);
        m_movingAgent.setWeponFireCapability(true);
        intializeAgentCallbacks(m_movingAgent);
        m_movingAgent.enableTranslateMovment(false);
        m_movingAgent.setOnDamagedCallback(onDamaged);
        m_currentState = m_idleStage;
        EnvironmentSound.Instance.listenToSound(onSoundAlert);
    }

    public void forceStart()
    {
        Start();
    }

    private void inializeGuard()
    {
        m_combatStage = new CoverPointBasedCombatStage(m_movingAgent,m_navMeshAgent,GameEnums.MovmentBehaviorType.FREE);
        m_idleStage = new IteractionStage(m_movingAgent,m_navMeshAgent,rutine.m_wayPoints.ToArray());
    }
    #endregion

    #region update
    public void Update()
    {
        //timeFromLastSwitch += Time.deltaTime;
        
        if(m_currentState != null && m_movingAgent.IsFunctional() && !m_movingAgent.isDisabled() & isInUse())
        {
            //m_currentState.updateStage();
            m_visualSensor.UpdateSensor();
        }

        if (m_healthBar)
        {
            m_healthBar.setHealthPercentage(m_movingAgent.AgentData);
        }

        // if(m_forced_attack)
        // {
        //     ((CoverPointBasedCombatStage)m_combatStage).CenteredPosition = m_force_transfrom.position;
        // }


    }

    void FixedUpdate()
    {
        if(m_restrictions == AGENT_AI_RESTRICTIONS.DISABLED)
        {
            return;
        }

        if(m_currentState != null && m_movingAgent.IsFunctional() && !m_movingAgent.isDisabled() & isInUse())
        {
            m_currentState.updateStage();

            if(m_movingAgent.isAimed() && m_movingAgent.isCrouched())
            {
               m_healthBar.set_half_cover();
            }
            else if(m_movingAgent.isCrouched())
            {
                m_healthBar.set_full_cover();
            }
            else
            {
                m_healthBar.set_no_cover();
            }
        }
    }
    #endregion

    #region Events

    public void onDamaged()
    {
        if(m_currentState != m_combatStage)
        {
            switchToCombatStage();
        }
        else
        {
            ((CoverPointBasedCombatStage)m_combatStage).alrtDamage();
            
        }
        m_visualSensor.forceUpdateSneosr();
    }

    public override void OnAgentDestroy()
    {
        base.OnAgentDestroy();
        EnvironmentSound.Instance.removeListen(onSoundAlert);
        m_healthBar.set_no_cover();
        m_navMeshAgent.enabled = false;

        if(m_currentState == m_combatStage)
        {
            m_combatStage.endStage();
        }

        if(m_healthBar)
        {
            m_healthBar.enabled = false;
        }
        
        // reset character
        Invoke("reUseCharacter", m_timeToReset);
        
    }

    private void reUseCharacter()
    {
        setInUse(false);
    }

    public override void resetCharacher()
    {
        if (m_navMeshAgent != null)
        {
            m_navMeshAgent.enabled = true;
        }

        if (m_movingAgent != null)
        {
        }
    }

    public override void onAgentDisable()
    {
        m_navMeshAgent.enabled = false;
        EnvironmentSound.Instance.removeListen(onSoundAlert);
        m_healthBar.set_no_cover();
    }

    public override void onAgentEnable()
    {
        m_navMeshAgent.enabled = true;
        if (m_healthBar)
        {
            m_healthBar.enabled = true;
        }
    }

    public void forceSetEnemyTarget(GameObject transform)
    {
        onEnemyDetection(transform.GetComponent<ICyberAgent>());
    }

    public void onEnemyDetection(ICyberAgent opponent)
    {
        m_combatStage.setTargets(opponent);
        if (m_currentState != m_combatStage)
        {
            switchToCombatStage();
        }
    }

    public void onAllClear()
    {
        if (!m_currentState.Equals(m_idleStage))
        {
            StartCoroutine(switchFromCombatStageToIteractionStage());
        }
    }

    // TODO - This sound component must be a another sensor. need to convert it
    public void onSoundAlert(Vector3 position, AgentBasicData.AgentFaction faction, float maxDistance)
    {
        var dis = Vector3.Distance(this.transform.position, position);

        // if max distance of the sound is less than the distance from the sound to unit. return
        if (dis > maxDistance)
        {
            return;
        }

        // Check for sound distance before acting on it.
        if ( 
            // Sound is comming from a enemy
            (dis < m_movingAgent.GetAgentData().max_Sound_hearing_distance && GamePlayCam.IsVisibleToCamera(m_movingAgent.getTransfrom()) && !faction.Equals(m_movingAgent.AgentData.m_agentFaction))
            
            // Sound is comming from a close friend
            || (dis < m_movingAgent.GetAgentData().max_Sound_hearing_distance / 2 && faction.Equals(m_movingAgent.AgentData.m_agentFaction)) )
        {

            if (m_currentState != m_combatStage)
            {
                //switchToCombatStage();
                m_visualSensor.forceUpdateSneosr();

                // If sound is comming from a enemy agent, force guess the location of the agent
                m_visualSensor.forceGussedTargetLocation(position);
            }


        }
    }


    
    public void OnPostRender()
    {
        //m_visualSensor.OnPostRender();
    }

    #endregion

    #region getters and setters

    public override float getSkill()
    {
        return m_movingAgent.AgentData.Skill;
    }

    public override void setMovableAgent(ICyberAgent agent)
    {
        m_movingAgent = (HumanoidMovingAgent)agent;
    }

    public override ICyberAgent getICyberAgent()
    {
        return m_movingAgent;
    }

    public override void setPosition(Vector3 postion)
    {
        m_navMeshAgent.Warp(postion);
    }
    #endregion

    #region Commands

    private void switchToCombatStage()
    {
        if(m_restrictions != AGENT_AI_RESTRICTIONS.NO_RESTRICTIONS)
        {
            return;
        }
        CombatTextShower.Instance.yellMessage("Initiating Attack", m_movingAgent,1,0.7f);
        if (m_currentState != m_combatStage)
        {
            m_movingAgent.cancleInteraction();
            ((CoverPointBasedCombatStage)m_combatStage).initalizeStage(preferedWeapon);
            m_currentState = m_combatStage;
        }
        
    }

    private IEnumerator switchFromCombatStageToIteractionStage()
    {
        yield return StartCoroutine(endCombatStage());
        swithtoIteractionStage();
    }

    private IEnumerator endCombatStage()
    {
        if (m_currentState.Equals(m_combatStage))
        {

            m_currentState.endStage();
            m_currentState = null;

            if (m_movingAgent.isHidden())
            {
                m_movingAgent.toggleHide();
            }

            if (m_movingAgent.isAimed())
            {
                m_movingAgent.stopAiming();
            }
            // This corutine will run until weapon is hosted
            yield return StartCoroutine(m_movingAgent.waitTillUnarmed());
        }
    }

    private void swithtoIteractionStage()
    {
        m_currentState = m_idleStage;
        m_idleStage.initalizeStage();
    }

    public void StopIteratingWaypoints()
    {
        ((IteractionStage)m_idleStage).setIterationEnabled(false);
    }

    public void StartIteratingWaypoints()
    {
        ((IteractionStage)m_idleStage).setIterationEnabled(true);
    }

    public void ResetWaypointIteration()
    {
        ((IteractionStage)m_idleStage).ResetIteration();
    }

    public void SetNewWaypoints(BasicWaypoint[] waypoints)
    {
        ((IteractionStage)m_idleStage).SetNewWaypoints(waypoints);
    }

    

    public override void ForceCombatMode(Transform position)
    {
        m_restrictions =  AGENT_AI_RESTRICTIONS.NO_RESTRICTIONS;
        m_force_transfrom = position;
        CoverPointBasedCombatStage combat_stage = ((CoverPointBasedCombatStage)m_combatStage);
        // set self cover point
        //GameObject coverpointprefab = Resources.Load<GameObject>("Prefab/SelfCoverPoint");
        //var selfCoverPoint = GameObject.Instantiate(coverpointprefab);
        //combat_stage.OwnedCoverPoint = selfCoverPoint.GetComponent<CoverPoint>();
        //combat_stage.MaxDistnaceFromCenteredPoint = 20;
        //combat_stage.CurrentMovmentType = GameEnums.MovmentBehaviorType.FREE;

        switchToCombatStage();
        Debug.Log(m_visualSensor);
        Debug.Log(position);
        m_visualSensor.forceCombatMode(position.position);
        m_forced_attack = true;
        m_visualSensor.disableLook();

    }

    public override void BeAlert()
    {
        if(m_visualSensor !=null)
        m_visualSensor.forceUpdateSneosr();
    }

    public override void SwitchToCombat()
    {
        switchToCombatStage();
    }

    #endregion


}
