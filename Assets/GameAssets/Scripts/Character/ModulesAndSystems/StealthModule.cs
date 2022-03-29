using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthModule : MonoBehaviour
{
    // Start is called before the first frame update
    private HumanoidMovingAgent m_agent;
    private bool is_equiped = false;

    public float WaitTimeToNeutral = 2;
    public bool PermenentStatus = false;
    void Start()
    {
        m_agent = GetComponent<HumanoidMovingAgent>();
        m_agent.setOnDamagedCallback(onDamaged);
        m_agent.setOnUnEquipCallback(onUnequip);
        m_agent.setOnEquipCallback(onEquip);
    }

    

    public void onEquip()
    {
        if(PermenentStatus)
        {
            return;
        }

        m_agent.GetAgentData().behaviorStatus = AgentBasicData.AgentBehaviorStatus.Suspicious;
        is_equiped = true;
    }

    public void onUnequip()
    {
        if (PermenentStatus)
        {
            return;
        }

        m_agent.GetAgentData().behaviorStatus = AgentBasicData.AgentBehaviorStatus.Neutral;
        is_equiped = false;
    }

    public void onDamaged()
    {
        if (PermenentStatus)
        {
            return;
        }

        m_agent.GetAgentData().behaviorStatus = AgentBasicData.AgentBehaviorStatus.Suspicious;

        if(!is_equiped)
        {
            StartCoroutine(WaitAndNeutral());
        }
        
    }

    IEnumerator WaitAndNeutral()
    {
        yield return new WaitForSeconds(WaitTimeToNeutral);
        m_agent.GetAgentData().behaviorStatus = AgentBasicData.AgentBehaviorStatus.Neutral;
    }

    public void MakePermenentSus()
    {
        PermenentStatus = true;
        m_agent.GetAgentData().behaviorStatus = AgentBasicData.AgentBehaviorStatus.Suspicious;
    }
}
