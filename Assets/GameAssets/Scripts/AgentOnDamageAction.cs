using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HutongGames.PlayMaker.Actions
{
    public class AgentOnDamageAction : FsmStateAction
    {
        // Start is called before the first frame update

        public AgentController m_agent;
        public float triggerHealthValueAt = -1;
        public float triggerShieldValueAt = -1;

        public FsmEvent triggerenvet;

        public override void OnEnter()
        {
            if(m_agent == null)
            {
                triggerEvent();
            }
        }

        public override void OnUpdate()
        {
            if(m_agent !=null)
            {
                var health = m_agent.getICyberAgent().GetAgentData().Health;
                var shield = m_agent.getICyberAgent().GetAgentData().Sheild;

                if(health < triggerHealthValueAt)
                {
                    triggerEvent();
                }

                if(shield < triggerShieldValueAt)
                {
                    triggerEvent();
                }
            }
            else
            {
                triggerEvent();
            }

        }

        private void triggerEvent()
        {
            if(triggerenvet!=null)
                Fsm.Event(triggerenvet);
            Finish();
        }
    }
}

