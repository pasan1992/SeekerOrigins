using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExplodingRunner : BasicMovmentCombatStage
{
    public ICyberAgent m_target;
    private float m_explosionRange = 2;
    private AttackerExplosion m_explosion;
    private Vector3 previousPosition;

    private float timeFromLastExplosion=0;

    public ExplodingRunner(ICyberAgent selfAgent,NavMeshAgent agent, AttackerExplosion explosion) : base(selfAgent, agent)
    {
        m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT;
        m_explosion = explosion; 
        m_stepIntervalInSeconds = 0.2f;


        resetAgent();
        m_selfAgent.GetAgentData().unbalanced = true;
    }

    private void resetAgent()
    {
        var currentWeapon =  ((HumanoidMovingAgent)m_selfAgent).getCurrentWeapon();
        if(currentWeapon && ((HumanoidMovingAgent)m_selfAgent).isArmed())
        {
            switch(((HumanoidMovingAgent)m_selfAgent).getCurrentWeaponType())
            {
                case Weapon.WEAPONTYPE.primary:
                ((HumanoidMovingAgent)m_selfAgent).togglePrimaryWeapon();
                break;
                case Weapon.WEAPONTYPE.secondary:
                ((HumanoidMovingAgent)m_selfAgent).togglepSecondaryWeapon();
                break;
            }
        }

        if(m_selfAgent.isHidden())
        {
            m_selfAgent.toggleHide();
        }

        if(m_explosion.enableTimerFromStart)
        {
            Explode();
        }
    }

    public override void setTargets(ICyberAgent target)
    {
        m_target = target;
    }

    protected override void updateFreePositionMovment()
    {
        // When centered positon is zero, center point is not concidered when calculating coverpoints
        // m_centeredPoint is set as zero from the super class setter for m_currentMovmentType variable.
        updateGeneralMovment();
        timeFromLastExplosion +=0.3f;
    }

    public override void alrtDamage()
    {
        m_selfAgent.dodgeAttack(m_navMeshAgent.desiredVelocity);
    }

    private void Explode()
    {
        if(timeFromLastExplosion>2f)
        {
            timeFromLastExplosion = 0;
            m_explosion.performAttack(1.5f,(m_selfAgent.getTransfrom().position - m_target.getTransfrom().position));
        }
        
    }

    private void updateGeneralMovment()
    {

        resetAgent();

        m_navMeshAgent.isStopped = false;

        float distance_to_target = 0;
        if(m_target !=null)
        {
            m_navMeshAgent.SetDestination(m_target.getCurrentPosition() + Random.insideUnitSphere);
        }
        else
        {
            return;
        }
        
        m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.MOVING_TO_POINT;

        distance_to_target = Vector3.Distance(m_target.getCurrentPosition(),m_selfAgent.getCurrentPosition());
        if(distance_to_target < m_explosion.Range/2)
        {
            Explode();
        }
    }
}
