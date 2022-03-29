using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterLocationObjective : BaseObjective
{
    public Transform location;
    public Transform Target;
    public float MaxDistanceToActivate;
    private bool m_objectiveStarted = false;

    public override void StartObjective()
    {
        base.StartObjective();
        m_objectiveStarted = true;
    }

    public void Update()
    {
        if(m_objectiveStarted)
        {
            var distance = Vector3.Distance(location.transform.position, Target.transform.position);
            if (distance <= MaxDistanceToActivate)
            {
                onObjectiveComplete();
            }
        }
    }

}
