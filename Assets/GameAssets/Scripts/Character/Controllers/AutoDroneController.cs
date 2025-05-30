﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(FlyingAgent))]
public class AutoDroneController :  AgentController
{
    // Start is called before the first frame update
    public string enemyTag;

    private FlyingAgent m_selfAgent;
    private NavMeshAgent m_navMeshAgent;
    private ICharacterBehaviorState m_currentBehaviorState;
    private ICharacterBehaviorState m_combatState;
    private ICharacterBehaviorState m_itearationState;
    private HumanoidAgentBasicVisualSensor m_visualSensor;
    private bool inStateTransaction = false;
    private HealthBar m_healthbar;

    #region initalize
    void Awake()
    {
        m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        m_navMeshAgent.updateRotation = false;    
        m_selfAgent = this.GetComponent<FlyingAgent>();

        m_itearationState = new IteractionStage(m_selfAgent,m_navMeshAgent,m_selfAgent.getGameObject().GetComponent<WaypointRutine>().m_wayPoints.ToArray());
        m_combatState = new DroneCombatStage(m_selfAgent,m_navMeshAgent,FindObjectOfType<PlayerController>().GetComponent<HumanoidMovingAgent>());
        m_currentBehaviorState  = m_itearationState;

        m_visualSensor = new HumanoidAgentBasicVisualSensor(m_selfAgent);
        m_selfAgent.stopAiming();
    }

    private void Start()
    {
        intializeAgentCallbacks(m_selfAgent);
        m_selfAgent.setOnDamagedCallback(onDamaged);

        m_visualSensor.setOnEnemyDetectionEvent(onEnemyDetection);
        m_visualSensor.setOnAllClear(onAllClear);
        
        EnvironmentSound.Instance.listenToSound(onSoundAlert);  
        m_healthbar = this.GetComponentInChildren<HealthBar>();
    }

    #endregion

    #region update

    // Update is called once per frame
    void Update()
    {
        if(m_selfAgent.IsFunctional() && !m_selfAgent.isDisabled() && isInUse())
        {
            m_currentBehaviorState.updateStage();

            if(!m_selfAgent.isInteracting())
            {
                m_visualSensor.UpdateSensor();
            }

            if (m_healthbar)
            {
                m_healthbar.setHealthPercentage(m_selfAgent.GetAgentData());
            }
            
        }
    }

    private void droneUpdate()
    {
    }

    #endregion

    #region commands
    #endregion

    #region getters and setters

    public override void setMovableAgent(ICyberAgent agent)
    {
        m_selfAgent = (FlyingAgent)agent;
    }

    public override float getSkill()
    {
        return m_selfAgent.GetAgentData().Skill;
    }

    public override ICyberAgent getICyberAgent()
    {
        return m_selfAgent;
    }

    private void switchToCombatStage()
    {
        if(m_currentBehaviorState !=m_combatState && !inStateTransaction && m_restrictions !=AGENT_AI_RESTRICTIONS.NO_COMBAT)
        {
            m_selfAgent.cancleInteraction();
            inStateTransaction = true;
            StartCoroutine(waitTillInteractionStopAndSwitchToCombat());
            m_selfAgent.aimWeapon();
        }  
    }

    #endregion

    #region events

    public void onDamaged()
    {
        switchToCombatStage();
    }
    public void onSoundAlert(Vector3 position, AgentBasicData.AgentFaction faction, float maxDistance)
    {
        var dis = Vector3.Distance(this.transform.position, position);

        // if max distance of the sound is less than the distance from the sound to unit. return
        if (dis > maxDistance)
        {
            return;
        }

        if ( m_currentBehaviorState != m_combatState)
        {
            m_visualSensor.forceUpdateSneosr();
        }     

        // If sound is comming from a enemy agent, force guess the location of the agent
        if(!faction.Equals(m_selfAgent.GetAgentData().m_agentFaction))
        {
            m_visualSensor.forceGussedTargetLocation(position);
        }
     }

    public void onEnemyDetection(ICyberAgent opponent)
    {
        if (!CommonFunctions.isAllies(opponent,m_selfAgent))
        {
         m_combatState.setTargets(opponent);
         switchToCombatStage();
        }

    }

    IEnumerator waitTillInteractionStopAndSwitchToCombat()
    {
        yield return m_selfAgent.waitTilInteractionOver();
        m_currentBehaviorState = m_combatState;
        inStateTransaction = false;
        m_navMeshAgent.enabled =true;
    }

    public void onAllClear()
    {
        if(m_currentBehaviorState != m_itearationState)
        {
            m_currentBehaviorState =m_itearationState;
        }
    }
    public override void OnAgentDestroy()
    {
        //TEst
        EnvironmentSound.Instance.removeListen(onSoundAlert);
        base.OnAgentDestroy();
        if(m_navMeshAgent.isOnNavMesh && m_navMeshAgent.isStopped)
        {
            m_navMeshAgent.isStopped = true;
        }

        m_navMeshAgent.enabled = false;

        //this.transform.position = new Vector3(3.18f, 3.27f, 52.93f);
        setInUse(false);     
    }


    public override void onAgentDisable()
    {
        m_navMeshAgent.isStopped = true;
        m_navMeshAgent.velocity = Vector3.zero;
    }

    public override void onAgentEnable()
    {
        //m_navMeshAgent.enabled = true;
        this.transform.position = m_selfAgent.transform.position;
        m_selfAgent.transform.parent = this.transform;

        if(m_navMeshAgent.isOnNavMesh && m_navMeshAgent.isStopped)
        {
            m_navMeshAgent.isStopped = false;
        }

    }

    public override void resetCharacher()
    {
        m_selfAgent.resetAgent();

        if(m_navMeshAgent.isOnNavMesh && m_navMeshAgent.isStopped)
        {
            m_navMeshAgent.isStopped = false;
        }

        m_navMeshAgent.enabled = true;
        this.gameObject.SetActive(true);
    }

    public override void setPosition(Vector3 postion)
    {
        m_navMeshAgent.Warp(postion);
    }

    public override void BeAlert()
    {
        m_visualSensor.forceUpdateSneosr();
    }

    public override void SwitchToCombat()
    {
        switchToCombatStage();
    }

    public override void MoveToWaypoint(BasicWaypoint[] waypoints, bool enableIterate,GameEvents.BasicNotifactionEvent onEnd)
    {
        base.MoveToWaypoint(waypoints,enableIterate,onEnd);
        if(m_currentBehaviorState != m_itearationState)
        {
            Debug.LogError("Current movment stage is not iteration stage, make the agent restriction as no combat from start. Or give a wait in the playmaker before calling this");
        }
        ((IteractionStage)m_itearationState).setMovmentEndEvent(onEnd);
        ((IteractionStage)m_itearationState).moveToWayPoint(waypoints,enableIterate);
        m_selfAgent.cancleInteraction();
    }
    public override void ForceCombatMode(Transform position)
    {
        switchToCombatStage();
        m_visualSensor.forceCombatMode(position.position);
    }

    #endregion

}
