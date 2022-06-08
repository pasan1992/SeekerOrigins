using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class SpawnEnemy : BaseObjective
{
    // Start is called before the first frame update
    public GameObject AgentPrefab;

    private int agent_count = 0;

    public override void StartObjective()
    {
        Debug.LogError("This is not a startable objective, use SpawnPrefabEnemy");
    }

    public void SpawnPrefabEnemy(int count,GameObject spawnPoint,Transform target,string FireEvent)
    {
        if(agent_count > 0)
        {
            Debug.LogError("You can't spawn more nemeies when previous spawn enemis are active");
        }
        OnObjectiveCompelteEvent = FireEvent;
        agent_count = count;

        for (int i =0; i< count ; i++)
        {
            var enemey = GameObject.Instantiate(AgentPrefab);
            var agent =  enemey.GetComponent<NavMeshAgent>();

            agent.Warp(spawnPoint.transform.position);
            agent.enabled = false;
            agent.enabled = true;

            AgentController agentCont = enemey.GetComponent<AgentController>();
            enemey.GetComponent<DamagableObject>().setOnDestroyed(OnDestroyed);
            StartCoroutine(waitAndAttack(agentCont,target));
        }
    }

    public void SpawnGivenRandomLocation(GameObject spawnAgent, int count,GameObject[] spawnPoint,Transform target,string FireEvent)
    {
        if(agent_count > 0)
        {
            Debug.LogError("You can't spawn more nemeies when previous spawn enemis are active");
        }
        OnObjectiveCompelteEvent = FireEvent;
        agent_count = count;

        for (int i =0; i< count ; i++)
        {
            var enemey = GameObject.Instantiate(spawnAgent);
            var agent =  enemey.GetComponent<NavMeshAgent>();
            var sp_id = i % spawnPoint.Length;
            var sp = spawnPoint[sp_id];

            agent.Warp(sp.transform.position);
            agent.enabled = false;
            agent.enabled = true;

            AgentController agentCont = enemey.GetComponent<AgentController>();
            enemey.GetComponent<DamagableObject>().setOnDestroyed(OnDestroyed);
            StartCoroutine(waitAndAttack(agentCont,target));
        }
    }

    public void SpawnEnemyRandomLocation(int count,GameObject[] spawnPoint,Transform target,string FireEvent)
    {

    }

    IEnumerator waitAndAttack(AgentController agent,Transform target)
    {
        yield return new WaitForSeconds(1);
        agent.ForceCombatMode(target);
    }

    public void OnDestroyed()
    {
        agent_count -=1;
        if(agent_count == 0)
        {
            onObjectiveComplete();
        }
    }
}
