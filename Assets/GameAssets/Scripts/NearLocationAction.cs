using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace HutongGames.PlayMaker.Actions
{
public class NearLocationAction : FsmStateAction
{
    public Transform target;
    public Transform location;
    public bool enable_pointer = false;
    public InGameInidactor.IndicatorTypes indicatorType;
    public float distance;
    public FsmEvent finishEvent;

    private GameObject m_indicator;

    public override void OnEnter()
    {
        if(enable_pointer)
        {
            var m_indicator = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.Obj_Indicator);
            m_indicator.SetActive(false);
            m_indicator.GetComponent<InGameInidactor>().IndicatorType = indicatorType;
        }
    }   
    public override void OnUpdate()
    {
        if(Vector3.Distance(target.transform.position,location.transform.position) < distance)
        {
            if(finishEvent!=null)
                Fsm.Event(finishEvent);
            if(m_indicator !=null)
            {
                m_indicator.SetActive(false);
            }
            Finish();
        }
    }



}

}

