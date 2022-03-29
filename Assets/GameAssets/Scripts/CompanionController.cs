using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionController : AgentController
{
    private ICyberAgent m_companionAgent;
    private Vector3 m_currentMovmentVector = Vector3.zero;
    private NavMeshAgent m_navMeshAgent;
    protected BasicMovmentCombatStage m_combatStage;
    protected BasicMovmentCombatStage m_followStage;
    protected HumanoidAgentBasicVisualSensor m_visualSensor;
    private RaycastHit m_raycastHit;
    public HumanoidMovingAgent m_followingTarget;
    private GameObject m_selfCoverPoint;
    private BasicMovmentCombatStage m_currentState;

    private bool m_onForceAlert = false;
    private HealthBar m_healthBar;
    public bool EnableFollow = true;

    public Vector3 FollowOffset = Vector3.left;

    public float max_Sound_hearing_distance = 30;
    public bool startWithCombat = false;

    public bool in_place_mode = false;

    // Start is called before the first frame update
    void Awake()
    {
        GameObject coverpointprefab = Resources.Load<GameObject>("Prefab/SelfCoverPoint");
        m_selfCoverPoint = GameObject.Instantiate(coverpointprefab);
        m_companionAgent = GetComponent<ICyberAgent>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_healthBar = this.GetComponentInChildren<HealthBar>();
    }
    void Start()
    {
        // Initialize navmesg agent
        m_navMeshAgent.updateRotation = false;

        // Intalize Stages
        initializeCombatStage();
        m_followStage = new BasicMovmentCombatStage(m_companionAgent,m_navMeshAgent);
        
        m_followStage.CurrentMovmentType = GameEnums.MovmentBehaviorType.FIXED_POSITION;
        m_currentState = m_followStage;
        m_combatStage.CenteredPosition = m_followingTarget.getCurrentPosition();

        m_visualSensor = new HumanoidAgentBasicVisualSensor(m_companionAgent);

        // Register Events
        m_visualSensor.setOnEnemyDetectionEvent(onEnemyDetection);
        m_visualSensor.setOnAllClear(onAllClear);
        m_companionAgent.setOnDamagedCallback(onDamaged);
        m_companionAgent.setOnDestoryCallback(OnAgentDestroy);
        m_followingTarget.setOnDamagedCallback(onDamaged);
        EnvironmentSound.Instance.listenToSound(onSoundAlert);

        m_followingTarget.setOnDestoryCallback(OnFollowerDestroy);

        if(startWithCombat)
        {
            switchToCombatStage();
        }

    }

    private void initializeCombatStage()
    {
        if (m_companionAgent is FlyingAgent)
        {
            m_combatStage = new DroneCombatStage(((FlyingAgent)m_companionAgent),m_navMeshAgent,null);
        }
        else if (m_companionAgent is HumanoidMovingAgent)
        {
            m_combatStage = new CoverPointBasedCombatStage(m_companionAgent,m_navMeshAgent,GameEnums.MovmentBehaviorType.FIXED_POSITION, m_selfCoverPoint.GetComponent<CoverPoint>());
        }
        m_combatStage.MaxDistnaceFromCenteredPoint = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if(!EnableFollow)
        {
            return;
        }

        //var agent = ((CoverPointBasedCombatStage)m_combatStage).getTarget();
        //if (agent !=null)
        //{
        //    Debug.Log(agent);
        //}


       if(m_companionAgent.IsFunctional())
       {
            updatePlayerInput();

            m_visualSensor.UpdateSensor();

            

            if(!in_place_mode)
            {
                m_followStage.CenteredPosition = m_followingTarget.getCurrentPosition() + FollowOffset * 2;
                m_combatStage.CenteredPosition = m_followingTarget.getCurrentPosition() + FollowOffset * 2;
            }
            
            

            if (m_currentState !=null)
            {
                m_currentState.updateStage();    
            }

            if (m_healthBar)
            {
                m_healthBar.setHealthPercentage(m_companionAgent.GetAgentData());
            }

        }
    }

    private void updatePlayerInput()
    {
        if(Input.GetMouseButtonUp(2))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out m_raycastHit, 100,LayerMask.GetMask("Floor"))) 
            {
                //switchToCombatStage();
                // m_onForceAlert = true;
                // m_currentState.CenteredPosition = m_raycastHit.point;
                //m_currentState.CurrentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT;
                //m_visualSensor.forceUpdateSneosr();
                //m_visualSensor.forceGussedTargetLocation(m_raycastHit.point);
                toggleInplaceMode(m_raycastHit.point);
                switchToCombatStage();
            }
        }
    }

    private void toggleInplaceMode(Vector3 position)
    {
        // disable inplace mode
        m_combatStage.MaxDistnaceFromCenteredPoint = 1;
        m_combatStage.CenteredPosition = position;
        in_place_mode = true;
    }

    public override ICyberAgent getICyberAgent()
    {
        return m_companionAgent;
    }

    public override float getSkill()
    {
        return m_companionAgent.getSkill();
    }

    public override void onAgentDisable()
    {
        throw new System.NotImplementedException();
    }

    public override void onAgentEnable()
    {
        throw new System.NotImplementedException();
    }

    public override void resetCharacher()
    {
        if (m_navMeshAgent != null)
        {
            m_navMeshAgent.enabled = true;
        }
    }

    public override void setMovableAgent(ICyberAgent agent)
    {
        m_companionAgent = agent;
    }

    public override void setPosition(Vector3 postion)
    {
        m_navMeshAgent.Warp(postion);
    }

    public void onEnemyDetection(ICyberAgent opponent)
    {
        m_combatStage.setTargets(opponent);

        if(m_currentState != m_combatStage)
        {
            switchToCombatStage();
        }
    }

    private void switchToCombatStage()
    {
        if(m_restrictions == AGENT_AI_RESTRICTIONS.NO_COMBAT)
        {
            return;
        }

        if(m_currentState != m_combatStage)
        {
            m_companionAgent.cancleInteraction();
            m_combatStage.initalizeStage();
            m_currentState = m_combatStage;
        }
    }

    public void onDamaged()
    {
        m_combatStage.alrtDamage();
        m_visualSensor.forceUpdateSneosr();
    }

    public void onAllClear()
    {
        Debug.LogError("On CLEARN DISABLED");
        if(m_onForceAlert)
        {
            m_combatStage.setTargets(null);
            return;
        }

        if(m_currentState!= null && !m_currentState.Equals(m_followStage) )
        {
            StartCoroutine(switchFromCombatStageToFollowStage());
        }
    }

    private IEnumerator endCombatStage()
    {
        if(m_currentState.Equals(m_combatStage))
        {
            
            m_currentState.endStage();
            m_currentState = null;

            if(m_companionAgent.isHidden())
            {
                m_companionAgent.toggleHide();
            }

            if(m_companionAgent.isAimed())
            {
                m_companionAgent.stopAiming();
            }

           // This corutine will run until weapon is hosted
            yield return StartCoroutine(m_companionAgent.waitTillUnarmed());
        }
    }

    private IEnumerator switchFromCombatStageToFollowStage()
    {
        yield return StartCoroutine(endCombatStage());
        swithToFollowStage();
    }

    private void swithToFollowStage()
    {
        m_currentState = m_followStage;
        m_followStage.initalizeStage();
    }

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
            (dis < max_Sound_hearing_distance && GamePlayCam.IsVisibleToCamera(m_companionAgent.getTransfrom()) && !faction.Equals(m_companionAgent.GetAgentData().m_agentFaction))

            // Sound is comming from a close friend
            || (dis < max_Sound_hearing_distance / 2 && faction.Equals(m_companionAgent.GetAgentData().m_agentFaction)))
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
    public override void BeAlert()
    {
        m_visualSensor.forceUpdateSneosr();
    }

    public override void SwitchToCombat()
    {
        switchToCombatStage();
    }

    public void OnFollowerDestroy()
    {
        switchToCombatStage();
        //if (OtherController !=null)
        //{
        //    OtherController.enabled = true;
        //    this.enabled = false;
        //}else
        //{
        //    Debug.LogError("No other controller for follower " + this.name);
        //}
    }

    public override void OnAgentDestroy()
    {
        base.OnAgentDestroy();
        m_navMeshAgent.enabled = false;

        if (m_currentState == m_combatStage)
        {
            m_combatStage.endStage();
        }

        if (m_healthBar)
        {
            m_healthBar.enabled = false;
        }

        // reset character
        Invoke("reUseCharacter", m_timeToReset);
    }
    public override void ForceCombatMode(Transform position)
    {
        switchToCombatStage();
        m_visualSensor.forceCombatMode(position.position);
    }
}
