using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HutongGames.PlayMaker.Actions
{

public class SpawnEnemyAction : FsmStateAction
{

    public CommonFunctions.ActionAgent[] agents;
    public Transform Target;
    public FsmEvent finishEvent;
  
    public bool waitTillEnd = true;

    private int agentCount = 0;
    public GameObject distance_target;

    private List<AgentController> m_agents = new List<AgentController>();

    public bool random_spawn = false;

    public override void OnEnter()
    {
     
            foreach(CommonFunctions.ActionAgent agent in agents) 
            {
                for(int i=0; i< agent.agentCount; i++) 
                {
                    agentCount += 1;
                }
            }



            StartCoroutine(waitAndSpawn());



        if (!waitTillEnd)
        {
            Finish();
        }

    }

    public override void OnUpdate()
    {
        foreach(var agent in m_agents)
        {
            if(agent !=null)
            {
                agent.ForceCombatMode(Target);
            }
        }
    }

    private IEnumerator waitAndSpawn()
    {
            foreach(CommonFunctions.ActionAgent agent in agents) 
            {
                for(int i=0; i< agent.agentCount; i++) 
                {

                    var sp = agent.spawnPoint;

                    if(distance_target != null && agent.alternateSpawnPoint !=null)
                    {
                        var distance = Vector3.Distance(distance_target.transform.position,agent.spawnPoint.transform.position);
                        var alt_distance = Vector3.Distance(distance_target.transform.position,agent.alternateSpawnPoint.transform.position);
                        if (random_spawn)
                        {
                            if (i % 2 != 0)
                            {
                                sp = agent.spawnPoint;
                            }
                            else
                            {
                                sp = agent.alternateSpawnPoint;
                            }
                        }
                        else if(alt_distance > distance)
                        {
                            sp = agent.alternateSpawnPoint;
                        }
                    }

                    yield return new WaitForSeconds(agent.wait);
                    var enemey = GameObject.Instantiate(agent.agentController);
                    var navMesh = enemey.GetComponent<NavMeshAgent>();

                    navMesh.Warp(sp.transform.position);
                    navMesh.enabled = false;
                    navMesh.enabled = true;

                    AgentController agentCont = enemey.GetComponent<AgentController>();
                    m_agents.Add(agentCont);
                    enemey.GetComponent<DamagableObject>().setOnDestroyed(OnDestroyed);
                    StartCoroutine(waitAndAttack(agentCont, Target));
                    
                }
                
                
            }
    }

    public void OnDestroyed()
    {
            agentCount -= 1;
        if(agentCount == 0)
        {
            Fsm.Event(finishEvent);
            Finish();
        }
    }

    
    IEnumerator waitAndAttack(AgentController agent,Transform target)
    {
        yield return new WaitForSeconds(2);
        agent.ForceCombatMode(target);
    }

    #if UNITY_EDITOR

   

#endif
}

}

