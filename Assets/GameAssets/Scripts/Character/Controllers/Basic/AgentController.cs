﻿
using UnityEngine;

public abstract class AgentController : MonoBehaviour
{
    protected agentOnDestoryEventDelegate m_onDestoryEvent;

    public bool m_inUse = false;
    public float m_timeToReset = 2;
    public enum AGENT_AI_RESTRICTIONS { NO_RESTRICTIONS,NO_COMBAT,DISABLED };
    public AGENT_AI_RESTRICTIONS m_restrictions = AGENT_AI_RESTRICTIONS.NO_RESTRICTIONS;

    protected void intializeAgentCallbacks(ICyberAgent cyberAgent)
    {
        cyberAgent.setOnDestoryCallback(OnAgentDestroy);
        cyberAgent.setOnDisableCallback(onAgentDisable);
        cyberAgent.setOnEnableCallback(onAgentEnable);
        cyberAgent.setOnDamagedCallback(OnDamage);
    }

    //public delegate void agentBasicEventDelegate();
    public delegate void agentOnDestoryEventDelegate(AgentController controller);

    // public enum AgentFaction { Player,Enemy,Neutral};
    // public AgentFaction m_agentFaction;
        
    public abstract void setMovableAgent(ICyberAgent agent);
    public abstract float getSkill();
    public abstract ICyberAgent getICyberAgent();

    public virtual void OnAgentDestroy()
    {
        if(m_onDestoryEvent !=null)
        {
            m_onDestoryEvent(this);
        }
    }

    public void RemoveRestrictions()
    {
        m_restrictions =  AGENT_AI_RESTRICTIONS.NO_RESTRICTIONS;
    }

    public abstract void onAgentDisable();
    public abstract void onAgentEnable();
    public abstract void resetCharacher();
    public abstract void setPosition(Vector3 postion);

    public void addOnDestroyEvent(agentOnDestoryEventDelegate onDestoryCallback)
    {
        m_onDestoryEvent = onDestoryCallback;
    }

    public void resetControlAgent()
    {
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public virtual void MoveToWaypoint(BasicWaypoint[] waypoints, bool enableIterate, GameEvents.BasicNotifactionEvent onEnd)
    {

    }

    public virtual void SetOnMovmentEndEvent()
    {
        
    }

    public bool isInUse()
    {
        return m_inUse;
    }

    public void setInUse(bool used)
    {
        m_inUse = used;
    }

    public virtual void BeAlert()
    {
        Debug.LogError("BE ALERT NOT IMPLEMENTED FOR " + this.gameObject.name);
    }

    public virtual void SwitchToCombat()
    {
        Debug.LogError("SwitchToCombat FOR " + this.gameObject.name);
    }

    public virtual void ForceCombatMode(Transform position)
    {
        Debug.LogError("FORCE COMBAT MODE NOT IMPLEMENTED " + this.gameObject.name);
    }
    public virtual void OnDamage()
    {
    }


}
