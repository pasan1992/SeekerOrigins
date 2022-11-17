using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HutongGames.PlayMaker.Actions
{

    public class SpawnGroupEnemyAction2 : FsmStateAction
    {

        public GameObject AgentPrefab1;
        public GameObject AgentPrefab2;
        public GameObject[] SpawnPoint;
        public Transform Target;
        public FsmEvent finishEvent;
        public int AgetnCount;
        private int agent_count = 0;

        public bool waitTillEnd = true;



        public override void OnEnter()
        {
            agent_count = AgetnCount;
            for (int i = 1; i < agent_count*2; i++)
            {
                var enemey1 = GameObject.Instantiate(AgentPrefab1);
                var enemey2 = GameObject.Instantiate(AgentPrefab2);
                var agent1 = enemey1.GetComponent<NavMeshAgent>();
                var agent2 = enemey2.GetComponent<NavMeshAgent>();
                var sp_id = i % SpawnPoint.Length;
                var sp = SpawnPoint[sp_id];

                agent1.Warp(sp.transform.position);
                agent2.Warp(sp.transform.position);
                agent1.enabled = false;
                agent2.enabled = false;
                agent1.enabled = true;
                agent2.enabled = true;




                AgentController agentCont1 = enemey1.GetComponent<AgentController>();
                AgentController agentCont2 = enemey2.GetComponent<AgentController>();
                enemey1.GetComponent<DamagableObject>().setOnDestroyed(OnDestroyed);
                enemey2.GetComponent<DamagableObject>().setOnDestroyed(OnDestroyed);
                StartCoroutine(waitAndAttack(agentCont1, Target));
                StartCoroutine(waitAndAttack(agentCont2, Target));
            }

            if (!waitTillEnd)
            {
                Finish();
            }
        }

        public void OnDestroyed()
        {
            var allagents = 2 * agent_count;
            allagents -= 1;
            if (allagents == 0)
            {
                Fsm.Event(finishEvent);
                Finish();
            }
        }


        IEnumerator waitAndAttack(AgentController agent, Transform target)
        {
            yield return new WaitForSeconds(2);
            agent.ForceCombatMode(target);
        }

#if UNITY_EDITOR

        public override float GetProgress()
        {
            return (AgetnCount - (float)agent_count) / AgetnCount;
        }

#endif
    }

}

