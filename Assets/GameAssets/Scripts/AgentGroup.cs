using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentGroup : MonoBehaviour
{
    // Start is called before the first frame update
    private List<AgentController> m_agentControllers;

    void Awake()
    {
        m_agentControllers = new List<AgentController>(this.GetComponentsInChildren<AgentController>());
    }
    void Start()
    {
        foreach (var obj in m_agentControllers)
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
        }
    }

    public List<AgentController> getAgents()
    {
        return m_agentControllers;
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
