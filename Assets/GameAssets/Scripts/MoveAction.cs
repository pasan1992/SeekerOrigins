using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HutongGames.PlayMaker.Actions
{
    public class MoveAction : FsmStateAction
    {
        // Start is called before the first frame update

        public AgentController m_agent;
        public BasicWaypoint[] m_waypoints;
        public FsmEvent finishEvent;
        public FsmEvent onStartEvent;
        public bool enableIterate = true;
        public bool waitTillEnd = false;

        public override void OnEnter()
        {
            if(waitTillEnd && enableIterate)
            {
                Debug.LogError("Both wait till end and enable iterate in the movment action is enabled please disable one of variables");
            }

            m_agent.MoveToWaypoint(m_waypoints,enableIterate,OnMovmentEnd);

            if(!waitTillEnd)
            {
                if(finishEvent!=null)
                    Fsm.Event(finishEvent);
                Finish();
            }
        }

        public void OnMovmentEnd()
        {
            if(finishEvent!=null)
                Fsm.Event(finishEvent);
            Finish();
        }
    }
}

