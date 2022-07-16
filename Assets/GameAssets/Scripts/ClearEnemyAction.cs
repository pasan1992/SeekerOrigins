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
    public GameObject EnemySet;
    public FsmEvent finishEvent;
    public override void OnEnter()
    {
        m_enemyAgents = EnemySet.GetComponentsInChildren<DamagableObject>();
        destroyedEnemyCount = 0;
        foreach (DamagableObject agent in m_enemyAgents)
        {
            agent.setOnDestroyed(onUnitDestory);
            var agent_cont = agent.getTransfrom().GetComponent<AgentController>();
            if(agent_cont!=null)
            {
                agent_cont.RemoveRestrictions();
            }
        }
    }

    public void onUnitDestory()
    {
        destroyedEnemyCount += 1;
        if(destroyedEnemyCount == m_enemyAgents.Length)
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

