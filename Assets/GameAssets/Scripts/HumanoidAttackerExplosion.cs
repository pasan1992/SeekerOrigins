using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAttackerExplosion : AttackerExplosion
{
    // Start is called before the first frame update
    private HumanoidMovingAgent m_movingAgent;

    public override void Start()
    {
        base.Start();
        m_movingAgent = this.GetComponent<HumanoidMovingAgent>();   
        ExplosionCountDown = -1;
        enableTimerFromStart = false;
    }



    public override void explode(ProjectilePool.POOL_OBJECT_TYPE explosionType)
    {
       GameObject explostion = ProjectilePool.getInstance().getPoolObject(explosionType);
       explostion.transform.position = this.transform.position;
       explostion.SetActive(true);
       damgeAround();
    }

    public override void performAttack(float duration,Vector3 direction)
    {
        m_movingAgent.MeleteAttack(duration,direction);
    }

    protected override void hitOnEnemy(Collider other)
    {
        if(!damageSelf)
        {
            DamagableObject damagableObject =  other.GetComponentInParent<DamagableObject>();
            if(damagableObject == m_selfDamagable)
            {
                return;
            }
        }
        base.hitOnEnemy(other);
    }
}
