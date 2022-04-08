using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearEnemyObjective : BaseObjective
{
    // Start is called before the first frame update

    private DamagableObject[] m_enemyAgents;
    private int m_enemyCount = 0;
    public GameObject EnemySet;

    public override void StartObjective()
    {
        m_enemyAgents = EnemySet.GetComponentsInChildren<DamagableObject>();
        m_enemyCount = 0;
        foreach (DamagableObject agent in m_enemyAgents)
        {
            agent.setOnDestroyed(onUnitDestory);
        }
        if(m_ObjectiveText !=null)
        {
            m_ObjectiveText.text = Objective.Replace("#", m_enemyCount.ToString()).Replace("*", m_enemyAgents.Length.ToString());
        }
    }

    public void onUnitDestory()
    {
        m_enemyCount += 1;

        if(m_ObjectiveText !=null)
        {
            m_ObjectiveText.text = Objective.Replace("#", m_enemyCount.ToString()).Replace("*", m_enemyAgents.Length.ToString());
        }
        

        if(m_enemyCount == m_enemyAgents.Length)
        {
            onObjectiveComplete();
        }
    }
}
