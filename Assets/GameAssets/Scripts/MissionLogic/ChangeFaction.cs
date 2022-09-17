using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFaction : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Change faction")]
    public List<HumanoidMovingAgent> AgentList;
    public AgentBasicData.AgentFaction NewFaction;

    [Header("Add Waypoints")]
    public List<BasicWaypoint> waypoints;
    public WaypointRutine waypointRutine;
    public AutoHumanoidAgentController autoAgent;

    public void SwitchFaction()
    {
        foreach (HumanoidMovingAgent agent in AgentList)
        {
            agent.setFaction(NewFaction);
        }
    }

    public void AddWaypoints()
    {
        waypointRutine.addNewWaypoints(waypoints);
        autoAgent.SetNewWaypoints(waypoints.ToArray());
        autoAgent.ResetWaypointIteration();
        autoAgent.getICyberAgent().cancleInteraction();
    }
}
