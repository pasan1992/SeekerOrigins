using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceAttack : MonoBehaviour
{
    // Start is called before the first frame update
    public List<AgentController> m_agents;
    public Transform m_target;
    public bool m_enableCharacters = false;


    public void StartForceAttack()
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

    IEnumerator waitAndSetCOmbat(AgentController agent, Vector3 target)
    {
        yield return new WaitForSeconds(0.5f);
        agent.ForceCombatMode(m_target);
    }
}
