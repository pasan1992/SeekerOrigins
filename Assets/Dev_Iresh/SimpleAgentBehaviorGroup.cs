using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAgentBehaviorGroup : MonoBehaviour
{
    // Start is called before the first frame update
    private List<HumanoidSimpleBehaviorController> m_agentControllers;
    void Awake()
    {
        m_agentControllers = new List<HumanoidSimpleBehaviorController>(this.GetComponentsInChildren<HumanoidSimpleBehaviorController>());
    }

    public void SetBehavior(HumanoidSimpleBehaviorController.SimpleBehavior behavior)
    {
        foreach(var agen in m_agentControllers)
        {
            agen.Behavor = behavior;
            agen.START_ACTION();
        }
    }

    public void SetTarget(GameObject target)
    {
        var cagent = target.GetComponent<ICyberAgent>();
        Debug.Log(cagent);
        foreach(var agen in m_agentControllers)
        {
            
            
            agen.SetTarget(cagent);
        }        
    }
}
