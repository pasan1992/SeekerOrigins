using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HutongGames.PlayMaker.Actions
{
    public class HumanAgentDo : FsmStateAction
    {
        // Start is called before the first frame update
        public HumanoidMovingAgent HumanAgent;
        private NavMeshAgent m_navmeshAgent;
        public BasicWaypoint WayPoint;
        private Interactable m_interatable;

        public override void OnPreprocess()
        {
            m_navmeshAgent = HumanAgent.GetComponent<NavMeshAgent>();
            m_interatable = WayPoint.GetComponent<Interactable>();

        }

        public override void OnEnter()
        {
            m_navmeshAgent.SetDestination(WayPoint.transform.position);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

    }
}
