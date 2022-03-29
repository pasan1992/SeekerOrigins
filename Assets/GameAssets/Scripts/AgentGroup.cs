using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentGroup : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> agents;
    private List<AgentController> m_agentControllers;
    void Start()
    {
        m_agentControllers = new List<AgentController>();
        foreach (GameObject obj in agents)
        {
            ICyberAgent agent = obj.GetComponent<ICyberAgent>();
            if(agent !=null)
            {
                agent.setOnDamagedCallback(OnDamage);
            }
            else
            {
                Debug.LogError("GIVEN AGENT HAS NO ICYBERAGENT");
            }


            AgentController agentcont = obj.GetComponent<AgentController>();
            if (agent != null)
            {
                m_agentControllers.Add(agentcont);
            }
            else
            {
                Debug.LogError("GIVEN AGENT HAS NO AGENT CONTROLLER");
            }
        }
    }

    public void OnDamage()
    {
        foreach(AgentController agentCont in m_agentControllers)
        {
            agentCont.SwitchToCombat();
            agentCont.BeAlert();         
        }
    }
}
