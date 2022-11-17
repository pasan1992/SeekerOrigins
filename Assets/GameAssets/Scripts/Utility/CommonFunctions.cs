using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CommonFunctions
{
    [System.Serializable]
    public class ActionAgent
    {
        
        public GameObject agentController;
        public int agentCount;
        public GameObject spawnPoint;
    }
    public class Damage
    {
        public float healthDamage;
        public float energyDamage;

        public Damage(float health, float energy)
        {
            this.healthDamage = health;
            this.energyDamage = energy;
        }
    }
    
    private static LayerMask floor_mask = -1;
    public static LayerMask getFloorLayerMask()
    {
        if(floor_mask == -1)
        {
            floor_mask = LayerMask.GetMask("Floor");
        }
        return floor_mask;
    }
    public static bool isAllies(ICyberAgent detectedAgent, ICyberAgent selfAgent)
    {
        if(detectedAgent == selfAgent)
        {
            return true;
        }
        if(detectedAgent.getFaction()==AgentBasicData.AgentFaction.Player)
        {
            if(selfAgent.getFaction() == AgentBasicData.AgentFaction.Allies)
            {
                return true;
            }
            if(detectedAgent.GetAgentData().behaviorStatus == AgentBasicData.AgentBehaviorStatus.Suspicious)
            {
                return false;
            }
            return true;
        }

        return detectedAgent.getFaction().Equals(selfAgent.getFaction());
    }

    public static bool isTargetVisibleToAgent(ICyberAgent agent, ICyberAgent target)
    {
        return true;
    }

    public static bool checkDestniationReached(NavMeshAgent navMeshAgent)
    {
        if(!navMeshAgent.pathPending && 
          navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && 
          (navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f))
        {
            return true;
        }
        return false;
    }

    public static void place_on_ground(Vector3 position,Transform obj)
    {
        RaycastHit hit;
        if (Physics.Raycast(position+ Vector3.up*5, Vector3.down, out hit, 30, getFloorLayerMask()))
        {
            obj.position = hit.point + Vector3.up * 0.2f;
        }
    }
}
