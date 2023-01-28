using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IteractionStage : WaypontMovementStage
{
    // Start is called before the first frame update
    ICyberAgent  m_agent;

    enum IterationState {MovintToPoint,OnPoint,Interaction,InteractionOver};

    IterationState m_currentIteractionState = IterationState.InteractionOver;
    public bool Iterate = true;

    public GameEvents.BasicNotifactionEvent onMovmentEnd;


    public IteractionStage(ICyberAgent selfAgent,NavMeshAgent navMeshAgent, BasicWaypoint[] wayPoints):base(selfAgent,navMeshAgent,wayPoints)
    {
        m_agent = selfAgent;
        m_currentIteractionState = IterationState.InteractionOver;
    }
    void Start()
    {
        
    }

    public override void setTargets(ICyberAgent target)
    {
        m_agent = target;
    }

    public override void initalizeStage()
    {
        m_currentIteractionState = IterationState.MovintToPoint;
        m_navMeshAgent.isStopped = false;
        MoveToWaypoint(getNextWaypoint());         
    }

    protected override void stepUpdate()
    {
        switch (m_currentIteractionState)
        {
            case IterationState.Interaction:

                if(!m_agent.isInteracting())
                {
                    m_currentIteractionState = IterationState.InteractionOver;
                }

            break;
            case IterationState.InteractionOver:
                 m_currentIteractionState = IterationState.MovintToPoint; 
                 m_navMeshAgent.enabled = true;   
                 m_navMeshAgent.isStopped = false;  
                MoveToWaypoint(getNextWaypoint());            
            break;
            case IterationState.MovintToPoint:
                if(m_navMeshAgent.isOnNavMesh)
                {
                    if(!m_navMeshAgent.pathPending && 
                    m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance && 
                    (m_navMeshAgent.hasPath || m_navMeshAgent.velocity.sqrMagnitude == 0f))
                    {
                        m_navMeshAgent.isStopped = true;
                        m_navMeshAgent.velocity = Vector3.zero;
                        m_currentIteractionState = IterationState.OnPoint;  
                    }
                }       
            break;
            case IterationState.OnPoint:
                Interactable interactableObject = m_wayPoints[m_currentWayPointID].GetComponent<Interactable>();

                if(interactableObject && !interactableObject.isInteracting())
                {
                    m_currentIteractionState = IterationState.Interaction;
                    m_navMeshAgent.enabled = false;
                    StartInteraction(interactableObject,Interactable.InteractableProperties.InteractableType.TimedInteraction);
                }
                else
                {
                    m_currentIteractionState = IterationState.MovintToPoint;
                    if(m_navMeshAgent.isOnNavMesh)
                    {
                        m_navMeshAgent.isStopped = false;
                    }
                    
                    MoveToWaypoint(getNextWaypoint());   
                }
            break;
        }
    }

    private void StartInteraction(Interactable interactableObj,Interactable.InteractableProperties.InteractableType type)
    {
        m_agent.interactWith(interactableObj,type);
    }

    protected override BasicWaypoint getNextWaypoint()
    {
        if(m_wayPoints.Length == 0)
        {
            m_wayPoints = new BasicWaypoint[1];
            m_wayPoints[0] =  createBasicWaypoint();
            return m_wayPoints[0];
        }

        m_currentWayPointID++;

        if(m_currentWayPointID == m_wayPoints.Length)
        {
            if (Iterate)
            {
                m_currentWayPointID = 0;  
            }
            else
            {
                endMovment();
                if(onMovmentEnd != null)
                {
                    onMovmentEnd();
                }
                m_currentWayPointID--;
            }
     
        }
        return m_wayPoints[m_currentWayPointID];
    }

    public void setMovmentEndEvent(GameEvents.BasicNotifactionEvent onEnd) {
        if (onEnd == null)
        {
            Debug.LogError("On End event is null");
            return;
            
        }
        onMovmentEnd += onEnd;
    }

    public  void moveToWayPoint(BasicWaypoint[] waypoints, bool enableIterate=true)
    {
        Iterate = enableIterate;
        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints are given");
            return;
        }
        m_currentWayPointID = -1;
        this.m_wayPoints = waypoints;
        startStage();   
    }

    private BasicWaypoint createBasicWaypoint()
    {
        GameObject waypont = new GameObject();
        waypont.transform.position = m_agent.getTransfrom().position;
        waypont.AddComponent<BasicWaypoint>();
        return waypont.GetComponent<BasicWaypoint>();
    }

    private void endMovment()
    {
        stopStage();
    }

    public void setIterationEnabled(bool enabled)
    {
        if(enabled)
        {          
            startStage();
        }
        Iterate = enabled;
    }

    public void ResetIteration()
    {
        m_currentWayPointID = 0;
    }
}
