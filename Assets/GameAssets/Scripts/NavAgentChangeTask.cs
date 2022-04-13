using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgentChangeTask : MonoBehaviour
{
    public void ChangeAreaMask(string MaskName,NavMeshAgent agent)
    {
        int areaMask = agent.areaMask;
        areaMask = 1 << NavMesh.GetAreaFromName("Nothing");//turn on all
        areaMask += 1 << NavMesh.GetAreaFromName(MaskName);
        //areaMask += 1 << NavMesh.GetAreaFromName(MaskName);
        // areaMask |= (1 << LayerMask.NameToLayer("Nothing"));
        // areaMask |= (1 << LayerMask.NameToLayer(MaskName));
        agent.areaMask=areaMask;
    }
}
