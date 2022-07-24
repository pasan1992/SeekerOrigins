using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HutongGames.PlayMaker.Actions
{

public class ClearEnemyAction : FsmStateAction
{
    private DamagableObject[] m_enemyAgents;
    private int destroyedEnemyCount = 0;
    private int fullEnemyCount = 0;
    public GameObject EnemySet;
    public FsmEvent finishEvent;

    private bool m_started = false;

    public override void OnEnter()
    {
        m_enemyAgents = EnemySet.GetComponentsInChildren<DamagableObject>();
        destroyedEnemyCount = 0;
        foreach (DamagableObject agent in m_enemyAgents)
        {
            agent.setOnDestroyed(onUnitDestory);
            var agent_cont = agent.getTransfrom().GetComponent<AgentController>();
            fullEnemyCount +=1;
            if(agent_cont!=null)
            {
                
                agent_cont.RemoveRestrictions();
            }
        }
        m_started = true;
    }

    public override void OnUpdate()
    {
        if(m_started)
        {
            bool agent_exist = false;
            foreach(var agent in m_enemyAgents)
            {
                if(!agent.isDestroyed())
                {
                    agent_exist = true;
                    break;
                }
            }

            if(!agent_exist)
            {
                Fsm.Event(finishEvent);
                Finish();                
            }
        }
    }

    public void onUnitDestory()
    {
        destroyedEnemyCount += 1;
        if(destroyedEnemyCount >= fullEnemyCount)
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
        return ((float)destroyedEnemyCount)/m_enemyAgents.Length;
    }

#endif
}

}

