using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExplodingRunner : BasicMovmentCombatStage
{
    public ICyberAgent m_target;
    private float m_explosionRange = 2;
    private BasicExplodingObject m_explosion;
    private Vector3 previousPosition;
    public ExplodingRunner(ICyberAgent selfAgent,NavMeshAgent agent, BasicExplodingObject explosion) : base(selfAgent, agent)
    {
        m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT;
        m_explosion = explosion; 
        m_stepIntervalInSeconds = 0.1f;
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
    }

    public override void alrtDamage()
    {
        m_selfAgent.dodgeAttack(m_navMeshAgent.desiredVelocity);
    }

    private void updateGeneralMovment()
    {
        float distance_to_target = 0;
        switch (m_currentMovmentBehaviorStage)
        {
        case GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT:
            m_navMeshAgent.SetDestination(m_target.getCurrentPosition() + Random.insideUnitSphere);
            m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.MOVING_TO_POINT;

                distance_to_target = Vector3.Distance(m_target.getCurrentPosition(),m_selfAgent.getCurrentPosition());
                if(distance_to_target < m_explosion.Range/2)
                {
                    m_explosion.explode();
                }

        break;
        case GameEnums.MovmentBehaviorStage.MOVING_TO_POINT:
                m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.AT_POINT;
                distance_to_target = Vector3.Distance(m_target.getCurrentPosition(),m_selfAgent.getCurrentPosition());
                if(distance_to_target < m_explosion.Range/2)
                {
                    m_explosion.explode();
                }
        break;
        case GameEnums.MovmentBehaviorStage.AT_POINT:
                distance_to_target = Vector3.Distance(m_target.getCurrentPosition(),m_selfAgent.getCurrentPosition());
                if(distance_to_target < m_explosion.Range/2)
                {
                    m_explosion.explode();
                }
                else
                {
                    m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT;

                }

        break;     
        }
    }
}
