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

    private IEnumerator waitAndSpawn()
    {
            foreach(CommonFunctions.ActionAgent agent in agents) 
            {
                for(int i=0; i< agent.agentCount; i++) 
                {
                    yield return new WaitForSeconds(agent.wait);
                    var enemey = GameObject.Instantiate(agent.agentController);
                    var navMesh = enemey.GetComponent<NavMeshAgent>();

                    navMesh.Warp(agent.spawnPoint.transform.position);
                    navMesh.enabled = false;
                    navMesh.enabled = true;

                    AgentController agentCont = enemey.GetComponent<AgentController>();
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

