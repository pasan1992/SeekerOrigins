using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentRewardSystem: MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] rewards;
    private LayerMask floorHitLayerMask;
    private AgentController m_agent_contoller;

    void Start()
    {
        foreach(GameObject obj in rewards)
        {
            obj.SetActive(false);
        }
        floorHitLayerMask = LayerMask.GetMask("Floor");
        m_agent_contoller = this.gameObject.GetComponent<AgentController>();
        m_agent_contoller.addOnDestroyEvent(onAgentDestory);
    }

    public void Spawn_Reward(Vector3 position)
    {
       foreach(GameObject obj in rewards)
        {
            obj.SetActive(true);
            obj.transform.position = Random.insideUnitSphere + Vector3.up + position;
            place_on_ground(obj);
        }
    }

    public void place_on_ground(GameObject obj)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(obj.transform.position, Vector3.down, out hit, 10, floorHitLayerMask))
        {
            obj.transform.position = hit.point + Vector3.up * 0.2f;
        }
    }

    public void onAgentDestory(AgentController controller)
    {
        Spawn_Reward(controller.transform.position);
    }


}
