using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoverPointBasedCombatStage : BasicMovmentCombatStage
{
    private ICyberAgent m_target;
    private CoverPoint m_currentCoverPoint;
    private CoverPoint m_ownedCoverPoint;

    private GameEnums.Cover_based_combat_stages m_coverBasedStages;

    float fireRangeDistance = 18;
    int m_maxTimeLimitAtCover = 20;
    float m_currentTimeAtCover;
    int m_noOfIteractions =4;
    private int m_Max_fake_agen_shoot_count = 20;


    private Collider[] targetLocations;
    private Transform targetLocation;
    private Vector3 randomOffset;
    private float suppriceFactor = 0;

    private bool damageAlert = false;
    private bool target_not_sigted = false;

    private int m_fake_agent_shoot_count = 0;

    private RocketPack m_rocket_pack;
    private bool m_can_dodge = false;


    public RocketPack Rocket_pack
    {
        get => m_rocket_pack;
        set
        {
            m_rocket_pack = value;
        }
    }

    public bool CanDodge
    {
        get => m_can_dodge;
        set
        {
            m_can_dodge = value;
        }
    }

    public override Vector3 CenteredPosition 
    { get => centeredPosition; 
     set
     {
         centeredPosition = value;
         m_ownedCoverPoint.transform.position = value;
     }
    }

    public CoverPoint OwnedCoverPoint { get => m_ownedCoverPoint;
        set { m_ownedCoverPoint = value;
            m_ownedCoverPoint.setOccupent(m_selfAgent);
        } 
    }

    public ICyberAgent getTarget()
    {
        return m_target;
    }

    public override GameEnums.MovmentBehaviorType CurrentMovmentType 
    {
         get { return m_currentMovmentType;}
         set 
         {
             m_currentMovmentType = value;
             switch (m_currentMovmentType)
             {
                 case GameEnums.MovmentBehaviorType.FIXED_POSITION:
                    if(m_currentCoverPoint)
                    {
                        m_currentCoverPoint.setOccupent(null);
                        m_currentCoverPoint = null;
                    }

                    if(m_ownedCoverPoint == null)
                    {
                        Debug.LogError("Need a owned coverpoint for fixed postion movment type");
                    }
                 break;
                 case GameEnums.MovmentBehaviorType.FREE:
                    CenteredPosition = Vector3.zero;
                 break;
                 case GameEnums.MovmentBehaviorType.NEAR_POINT:
                 break;
             }
         }
    }



    public bool can_fire()
    {
       // return GamePlayCam.IsVisibleToCamera(m_selfAgent.getTransfrom());

        
        if (m_target.getFaction() == AgentBasicData.AgentFaction.Player)
        {
            return GamePlayCam.IsVisibleToCamera(m_selfAgent.getTransfrom());
        }
        return Vector3.Distance(m_selfAgent.getTransfrom().position, m_target.getTransfrom().position) < fireRangeDistance;
    }

    /*
    public static bool IsVisibleToCamera(Transform transform)
    {
        Vector3 visTest = Camera.main.WorldToViewportPoint(transform.position);
        return (visTest.x >= 0.05f && visTest.y >= 0.05f) && (visTest.x <= 0.95f && visTest.y <= 0.95f) && visTest.z >= 0;
    }
    */

    public CoverPointBasedCombatStage(ICyberAgent selfAgent, NavMeshAgent agent,GameEnums.MovmentBehaviorType behaviorType, CoverPoint coverPoint) : base(selfAgent, agent)
    {
        m_ownedCoverPoint = coverPoint;
        m_ownedCoverPoint.setOccupent(selfAgent);
        m_currentMovmentType = behaviorType;

    }

    public CoverPointBasedCombatStage(ICyberAgent selfAgent, NavMeshAgent agent,GameEnums.MovmentBehaviorType behaviorType) : base(selfAgent, agent)
    {
        m_currentMovmentType = behaviorType;
    }

    public override void endStage()
    {
        OnAgentDestroyed();
        base.endStage();     
    }

    public override void initalizeStage()
    {
        base.initalizeStage();
        ((HumanoidMovingAgent)m_selfAgent).togglePrimaryWeapon();
       m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT;
    }

    public void initalizeStage(AutoHumanoidAgentController.WeaponType type)
    {
        base.initalizeStage();
        switch (type)
        {
            case AutoHumanoidAgentController.WeaponType.PRIMARY:
                ((HumanoidMovingAgent)m_selfAgent).togglePrimaryWeapon();
                break;
            case AutoHumanoidAgentController.WeaponType.SECONDAY:
                ((HumanoidMovingAgent)m_selfAgent).togglepSecondaryWeapon();
                break;
        }    
        m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT;
    }

    public override void setTargets(ICyberAgent target)
    {
        if(target !=null &&  m_target != target && m_target != m_selfAgent)
        {
            m_target = target;
            targetLocations = m_target.getTransfrom().gameObject.GetComponentsInChildren<Collider>();
            suppriceFactor = 1.5f;

            if(m_ownedCoverPoint != null)
            {
                m_ownedCoverPoint.setTargetToCover(target);
            }
            
            // Check for fake target
            if (target.GetType() == typeof(FakeMovingAgent))
            {
                target_not_sigted = true;
            }
            else
            {
                target_not_sigted = false;
                m_fake_agent_shoot_count = 0;
            }

            // If new target is found, new cover position is needed
           // m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT;
        }
    }

    public override void stopStageBehavior()
    {
        base.stopStageBehavior();
    }
    protected override void stepUpdate()
    {
        base.stepUpdate();
    }

    protected override void updateFreePositionMovment()
    {
        // When centered positon is zero, center point is not concidered when calculating coverpoints
        // m_centeredPoint is set as zero from the super class setter for m_currentMovmentType variable.
        updateGeneralMovment();
        //updateCombatBehavior();
    }

    public override void updateStage()
    {
        base.updateStage();
        aimAtTarget();
    }

    public bool canFireAtAgent()
    {
        // If target is not take can shoot.
        if (!(m_target is FakeMovingAgent))
        {
            return true;
        }

        // If fake there is limited number of shots it takes
        if (m_fake_agent_shoot_count > m_Max_fake_agen_shoot_count)
        {
            return false;
        }
        m_fake_agent_shoot_count += 1;
        return true;
    }

    protected override void updateCombatBehavior()
    {
        
        findTargetLocationToFire();
        StateShower.Instance.setText(m_currentMovmentBehaviorStage.ToString() + "-"+ m_coverBasedStages.ToString() + ":"+ m_noOfIteractions,m_selfAgent);
        //Debug.Log(m_currentMovmentBehaviorStage);
       
        switch (m_currentMovmentBehaviorStage)
        {
            case GameEnums.MovmentBehaviorStage.AT_POINT:

                if(m_target != null)
                {
                    coverBasedCombat();                
                }
                else
                {
                    //Debug.LogError("No Target For" + m_selfAgent.getTransfrom().name);
                }
                
            break;
            case GameEnums.MovmentBehaviorStage.MOVING_TO_POINT:
                //if (m_target !=null && Vector3.Distance(m_selfAgent.getCurrentPosition(), m_target.getCurrentPosition()) < fireRangeDistance)
                if (m_target != null && can_fire())
                {
                    m_selfAgent.aimWeapon();
                    m_enableRun = false;
                    if(canFireAtAgent())
                    {
                        m_selfAgent.weaponFireForAI();
                        FireRocket();
                    }
                        
                    setStepIntervalSize(0.3f);
                }
                else
                {
                    m_selfAgent.stopAiming();
                    m_enableRun = true;
                    
                    getUpFromCover();
                }

                if(damageAlert && Random.value <  m_selfAgent.getSkill())
                {
                    if(m_can_dodge)
                    {
                        m_selfAgent.dodgeAttack(m_navMeshAgent.desiredVelocity);
                    }
                    CombatTextShower.Instance.yellMessage("Got Damaged!", m_selfAgent, 0.7f,0.5f);
                    damageAlert = false;
                }
            break;
            case GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT:
                getUpFromCover();
                CombatTextShower.Instance.yellMessage("Flanking!", m_selfAgent,0.7f,0.3f);
                break;
        }
        damageAlert = false;
        
    }

    private void coverBasedCombat()
    {
        switch (m_coverBasedStages)
        {
            case GameEnums.Cover_based_combat_stages.IN_COVER:

                 m_noOfIteractions --;

                 // If Not safe from current target get up and shoot
                 if( (m_currentCoverPoint != null && !m_currentCoverPoint.isSafeFromTarget()) || (m_ownedCoverPoint !=null && !m_ownedCoverPoint.isSafeFromTarget()) )
                 {
                    m_selfAgent.aimWeapon();
                    m_noOfIteractions = (int)Random.Range(4,10);
                    getUpFromCover();
                    m_coverBasedStages = GameEnums.Cover_based_combat_stages.SHOOT;
                    break;    
                 }

                // After staying at cover, peek and shoot
                if(m_noOfIteractions <= 0)
                 {
                    if(can_fire())
                    {
                        m_selfAgent.aimWeapon();
                        m_noOfIteractions = (int)Random.Range(4,10);
                        m_coverBasedStages = GameEnums.Cover_based_combat_stages.SHOOT;
                    }
                    else
                    {
                        //Debug.LogError("Fire Range Not enough for" + m_selfAgent.getTransfrom().name);

                        m_noOfIteractions = 0;
                    }

                    break;
                 }


                // When in cover, hide and do no aim
                if(m_selfAgent.isAimed())
                {
                    m_selfAgent.stopAiming();
                }

                if(!m_selfAgent.isHidden())
                {
                    m_selfAgent.toggleHide();
                }
                 
            break;
            case GameEnums.Cover_based_combat_stages.SHOOT:   

                // When firing is done, return to cover
                if(m_noOfIteractions <= 0)
                {
                    m_noOfIteractions = (int)Random.Range(2,5);
                    m_coverBasedStages = GameEnums.Cover_based_combat_stages.IN_COVER;
                    CombatTextShower.Instance.yellMessage("Taking Cover", m_selfAgent,0.7f,0.3f);
                    break;
                }

                if((m_currentCoverPoint != null && !m_currentCoverPoint.isSafeFromTarget()) || (m_ownedCoverPoint != null && !m_ownedCoverPoint.isSafeFromTarget()))
                {
                    getUpFromCover();
                }
                else
                {
                    getCover();
                }

                if(damageAlert)
                {
                    damageAlert = false;

                    if(m_noOfIteractions > 3)
                    {
                        m_noOfIteractions -=3;
                    }
                    else
                    {
                        m_noOfIteractions = (int)Random.Range(4,6);
                        m_coverBasedStages = GameEnums.Cover_based_combat_stages.IN_COVER;
                    }          
                }

                // Shoot 
                m_noOfIteractions --;
                if (!target_not_sigted)
                {
                    if (canFireAtAgent())
                    {
                        FireRocket();
                        m_selfAgent.weaponFireForAI();
                    }
                        
                    CombatTextShower.Instance.yellMessage("Fire!", m_selfAgent, 0.3f,0.5f);
                }
            break;
        }
    }

    private void updateGeneralMovment()
    {
        switch (m_currentMovmentBehaviorStage)
        {
        case GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT:
            
            CoverPoint tempCurrentCoverPoint =  CoverPointsManager.getNearCoverObject(m_selfAgent,m_target,fireRangeDistance,true,CenteredPosition,MaxDistnaceFromCenteredPoint);
            //CoverPoint tempCurrentCoverPoint = CoverPointsManager.getNextCoverPoint(m_currentCoverPoint,m_target,m_selfAgent.getCurrentPosition());

            //Stop the moving agent in case status change from moving - calculate next point
            m_navMeshAgent.velocity = Vector3.zero;
            m_navMeshAgent.isStopped = true;

            if(tempCurrentCoverPoint != null)
            {
                if(m_currentCoverPoint != tempCurrentCoverPoint)
                {
                    // Hande CoverPoint
                    if (m_currentCoverPoint)
                    {
                        m_currentCoverPoint.setOccupent(null);
                    }
                    m_currentCoverPoint = tempCurrentCoverPoint;
                    m_currentCoverPoint.setOccupent(m_selfAgent);

                    m_navMeshAgent.SetDestination(m_currentCoverPoint.getPosition());

                    // Get up and move
                    getUpFromCover();
                    m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.MOVING_TO_POINT;
                    m_navMeshAgent.isStopped = false;
                }
                else
                {
                    Debug.Log("Same Cover Point");
                }

            }
            // else
            // {
            //     Debug.LogError("Agent "+ m_selfAgent.getGameObject().name + " Cannot find cover points" );
            // }
        break;
        case GameEnums.MovmentBehaviorStage.MOVING_TO_POINT:

            if(CommonFunctions.checkDestniationReached(m_navMeshAgent) || m_navMeshAgent.remainingDistance < 0.3f)
            {
                m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.AT_POINT;
                m_currentTimeAtCover = 0;
                m_navMeshAgent.velocity = Vector3.zero;
                m_navMeshAgent.isStopped = true;
            }

        break;
        case GameEnums.MovmentBehaviorStage.AT_POINT:
            if(m_maxTimeLimitAtCover < m_currentTimeAtCover)
            {
               m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT;
            }
            m_currentTimeAtCover +=1;
        break;     
        }
    }

    protected override void updateNearPointPositonMovment()
    {
        updateGeneralMovment();
        //updateCombatBehavior();
    }

    //protected override void updateFixedPositionMovment()
    //{
    //    base.updateFixedPositionMovment();
    //    //updateCombatBehavior();
    //}
    protected override void updateFixedPositionMovment()
    {
        switch (m_currentMovmentBehaviorStage)
        {
            case GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT:

                CoverPoint cp = CoverPointsManager.getNearCoverObject(m_selfAgent, m_target, fireRangeDistance, true, CenteredPosition, MaxDistnaceFromCenteredPoint);
                if (cp == null)
                {
                    
                    m_navMeshAgent.destination = CenteredPosition;
                }
                else
                {
                    m_navMeshAgent.destination = cp.getPosition();
                }
                
                m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.MOVING_TO_POINT;
                m_navMeshAgent.isStopped = false;
                break;
            case GameEnums.MovmentBehaviorStage.MOVING_TO_POINT:

                if (CommonFunctions.checkDestniationReached(m_navMeshAgent) || m_navMeshAgent.remainingDistance < 0.3f)
                {
                    m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.AT_POINT;
                    m_navMeshAgent.velocity = Vector3.zero;
                    m_navMeshAgent.isStopped = true;
                }
                break;
            case GameEnums.MovmentBehaviorStage.AT_POINT:

                // If current distination is different from the new fixed position
                if (m_navMeshAgent.destination != null && Vector3.Distance(m_navMeshAgent.destination, centeredPosition) > 2)
                {
                    m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT;
                    m_navMeshAgent.isStopped = false;
                }

                break;
        }
    }
    private void getUpFromCover()
    {
        if(m_selfAgent.isHidden())
        {
            m_selfAgent.toggleHide();
        }
    }

    private void getCover()
    {
        if(!m_selfAgent.isHidden())
        {
            m_selfAgent.toggleHide();
        }
    }

    public override void alrtDamage()
    {
        damageAlert = true;
    }

    private void FireRocket()
    {
        if(m_rocket_pack && Random.value > 0.5  && targetLocation)
        {
            m_rocket_pack.FireMissleLocation(targetLocation.position);
        }
    }

    private void findTargetLocationToFire()
    {
        if(m_target !=null)
        {
            HumanoidMovingAgent humanoidOpponent = m_target as HumanoidMovingAgent;

            if(humanoidOpponent != null && humanoidOpponent.isCrouched() && humanoidOpponent.isAimed())
            {
                targetLocation = humanoidOpponent.getHeadTransfrom();
            }
            else if (targetLocations !=null && targetLocations.Length !=0)
            {
                int randomIndex = Random.Range(0, targetLocations.Length - 1);
                targetLocation = targetLocations[randomIndex].transform;
            }
            else
            {
                targetLocation = m_target.getTransfrom();
            }

            if(Random.value + suppriceFactor > m_selfAgent.getSkill())
            {
                randomOffset = Random.insideUnitSphere * 1f;
                //randomOffset = Vector3.zero;
            }
            else
            {
                randomOffset = Vector3.zero;
            }

            randomOffset.y = 0;

            if (suppriceFactor > 0)
            {
                suppriceFactor -=0.1f;
            }
        }
    }

    public override void OnAgentDestroyed()
    {
        if(m_currentCoverPoint)
        {
            m_currentCoverPoint.setOccupent(null);
        }
        
    }
    
    private void aimAtTarget()
    {
        if(m_target !=null)
        {
            // When self is in cover
            if(m_coverBasedStages.Equals(GameEnums.Cover_based_combat_stages.IN_COVER))
            {
                // If target is hidden aim for the head
                if(m_target.isHidden())
                {
                    // When target is peeking from cover
                    if(m_target.isAimed())
                    {
                        m_selfAgent.setTargetPoint(m_target.getTransfrom().position + new Vector3(0, 1.1f, 0) + randomOffset + m_target.getCurrentVelocity()*0);
                    }
                    // When target is hidden in cover - supress fire
                    else
                    {
                        m_selfAgent.setTargetPoint(m_target.getTransfrom().position + new Vector3(0, 0.4f, 0) + randomOffset + m_target.getCurrentVelocity()*0);
                    }
                }
                // Target is open 
                else
                {
                    m_selfAgent.setTargetPoint(m_target.getTransfrom().position + new Vector3(0, 1.25f, 0) + randomOffset + m_target.getCurrentVelocity()*0);
                }
            }
            else
            {
                m_selfAgent.setTargetPoint(targetLocation.position + randomOffset + m_target.getCurrentVelocity()*0);
            }
        }
    }
}
