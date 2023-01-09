using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace HutongGames.PlayMaker.Actions
{

public class ActionMoveToPoint : FsmStateAction
{
    public AgentController agentControl;

    private ICyberAgent agent;
    public Transform location;
    public Transform lookatTarget;

    public bool m_enableRun;
    public bool m_enableHide;
    private NavMeshAgent m_navMeshAgent;
    private bool startMoving = false;
    public bool disableController = false;

    public override void OnEnter()
    {
        agent = agentControl.getICyberAgent();
        m_navMeshAgent = agentControl.GetComponent<NavMeshAgent>();
        m_navMeshAgent.SetDestination(location.position);
        m_navMeshAgent.isStopped = false;

        if(!agent.isHidden() && m_enableHide)
        {
            agent.toggleHide();
        }

        var humanAgent = (HumanoidMovingAgent)agent;
        if(humanAgent !=null)
        {
            humanAgent.hosterWeapon();
        }

        if(disableController)
        {
            agentControl.enabled = false;
        }
    }   
    public override void OnUpdate()
    {
        if (!m_navMeshAgent.pathPending)
        {
            startMoving = true;

            if(!agent.isHidden() && m_enableHide)
            {
                agent.toggleHide();
            }

            Vector3 velocity = m_navMeshAgent.desiredVelocity;

            if (!m_enableRun)
            {
                if(velocity.magnitude > 1 )
                {
                    velocity = velocity.normalized;
                }
                
            }
            else
            {
                velocity = velocity * 2.2f;
            }

            velocity = new Vector3(velocity.x, 0, velocity.z);
            agent.moveCharacter(velocity);
        }

        if(startMoving && checkDestniationReached())
        {
            m_navMeshAgent.isStopped = true;
            if(lookatTarget !=null)
            {
                agent.getGameObject().transform.LookAt(lookatTarget);
            }
            
            m_navMeshAgent.velocity = Vector3.zero;
            agent.moveCharacter(Vector3.zero);

            if(disableController)
            {
                agentControl.enabled = true;
            }

            Finish();
        }
    }

    protected bool checkDestniationReached()
    {
        if(!m_navMeshAgent.pathPending && 
          m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance && 
          (m_navMeshAgent.hasPath || m_navMeshAgent.velocity.sqrMagnitude == 0f))
        {
            return true;
        }
        return false;
    }

}

}

