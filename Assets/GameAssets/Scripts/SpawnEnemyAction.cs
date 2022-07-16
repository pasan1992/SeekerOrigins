using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HutongGames.PlayMaker.Actions
{

public class SpawnEnemyAction : FsmStateAction
{

    public GameObject AgentPrefab;
    public GameObject[] SpawnPoint;
    public Transform Target;
    public FsmEvent finishEvent;
    public int AgetnCount;
    private int agent_count = 0;

    public bool waitTillEnd = true;

    

    public override void OnEnter()
    {
        agent_count = AgetnCount;
        for (int i =0; i< agent_count ; i++)
        {
            var enemey = GameObject.Instantiate(AgentPrefab);
            var agent =  enemey.GetComponent<NavMeshAgent>();
            var sp_id = i % SpawnPoint.Length;
            var sp = SpawnPoint[sp_id];

            agent.Warp(sp.transform.position);
            agent.enabled = false;
            agent.enabled = true;

            AgentController agentCont = enemey.GetComponent<AgentController>();
            enemey.GetComponent<DamagableObject>().setOnDestroyed(OnDestroyed);
            StartCoroutine(waitAndAttack(agentCont,Target));
        }

        if (!waitTillEnd)
        {
            Finish();
        }
    }

    public void OnDestroyed()
    {
        agent_count -=1;
        if(agent_count == 0)
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

    public override float GetProgress()
    {
        return (AgetnCount - (float)agent_count)/AgetnCount;
    }

#endif
}

}

