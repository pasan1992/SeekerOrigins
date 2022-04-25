using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceAttack : MonoBehaviour
{
    // Start is called before the first frame update
    public List<AgentController> m_agents;

    public AgentGroup m_agentGroup;
    public Transform m_target;
    public bool m_enableCharacters = false;
    private int agent_count = 0;

    public string OnObjectiveCompelteEvent = "";

    public PlayMakerFSM externalFSM;

    public void Start() 
    {
        if(m_agents !=null & m_agents.Count > 0)
        {
            agent_count = m_agents.Count;
            foreach(var agent in m_agents)
            {
                agent.GetComponent<DamagableObject>().setOnDestroyed(OnDestroyed);
            }
        }
        else if (m_agentGroup !=null)
        {
            agent_count = m_agentGroup.agents.Count;
            foreach(var agent in m_agentGroup.agents)
            {
                agent.GetComponent<DamagableObject>().setOnDestroyed(OnDestroyed);
            }
        }
        else
        {
            Debug.LogError("No agent list or agent group found");
        }

    }

    public void StartForceAttack()
    {
      
        if(m_agents !=null & m_agents.Count > 0)
        {
            foreach (AgentController agent in m_agents)
            {
                if (m_enableCharacters)
                {
                    agent.gameObject.SetActive(true);
                }
                StartCoroutine(waitAndSetCOmbat(agent,m_target.position));
            }
        }
        else if(m_agentGroup !=null)
        {
            foreach (GameObject agentobj in m_agentGroup.agents)
            {
                var agent = agentobj.GetComponent<AgentController>();
                if (m_enableCharacters)
                {
                    agent.gameObject.SetActive(true);
                }
                StartCoroutine(waitAndSetCOmbat(agent,m_target.position));
            }           
        }

    }

    IEnumerator waitAndSetCOmbat(AgentController agent, Vector3 target)
    {
        yield return new WaitForSeconds(0.5f);
        agent.ForceCombatMode(m_target);
    }

    public void OnDestroyed()
    {
        agent_count -=1;
        if(agent_count == 0)
        {
            if (OnObjectiveCompelteEvent != "")
            {
                if (externalFSM == null)
                {
                    PlayMakerFSM.BroadcastEvent(OnObjectiveCompelteEvent);
                }
                else
                {
                    externalFSM.Fsm.Event(OnObjectiveCompelteEvent);
                }
            }

        }
    }
}
