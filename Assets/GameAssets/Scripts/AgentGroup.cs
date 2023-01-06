using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentGroup : MonoBehaviour
{
    // Start is called before the first frame update
    private List<AgentController> m_agentControllers;
    public bool setGroupBehavior = false;
    public float start_visual_distance = 0;
    public float start_visual_angle = 0;
    public float start_sound_distance = 0;
    public float end_visual_angle = 0;
    public float end_visual_distance = 0;
    public float end_sound_distace = 0;
    private bool alert_done = false;

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
                agent.setOnDestoryCallback(OnDestoryed);
                obj.setOnSwitchToCombat(OnDamage);

                if(setGroupBehavior)
                {
                agent.GetAgentData().VisualAngle = start_visual_angle;
                agent.GetAgentData().VisualDistance = start_visual_distance;
                agent.GetAgentData().max_Sound_hearing_distance = start_sound_distance;
                }

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

    public void OnDestoryed()
    {

    }

    public void setAgentRestriction(AgentController.AGENT_AI_RESTRICTIONS restriction)
    {
        foreach(var agent in m_agentControllers)
        {
            agent.m_restrictions = restriction;
        }
    }

    public void OnDamage()
    {
        if(alert_done)
        {
            return;
        }
        alert_done = true;
        foreach(AgentController agentCont in m_agentControllers)
        {
            if(agentCont !=null)
            {
                agentCont.SwitchToCombat();
                agentCont.BeAlert(); 

                if(setGroupBehavior)
                {
                    agentCont.getICyberAgent().GetAgentData().VisualAngle = end_visual_angle;
                    agentCont.getICyberAgent().GetAgentData().VisualDistance = end_visual_distance;
                    agentCont.getICyberAgent().GetAgentData().max_Sound_hearing_distance = end_sound_distace;
                }
            }
        
        }
    }
}
